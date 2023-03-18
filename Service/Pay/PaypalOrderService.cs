using Newtonsoft.Json;
using NLog;
using PayPal.Api;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly ExchangeRateService exchangeRateService;

    public PaypalOrderService(
        ISession session,
        PayService payService,
        ExchangeRateService exchangeRateService
    ) {
        this.session = session;
        this.payService = payService;
        this.exchangeRateService = exchangeRateService;
    }

    public string Create(PaypalOrderCreateParam param) {
        int orderId = param.OrderId;
        int couponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }

        CurrencyType currency = order.Country.Currency;
        decimal exchangeRate = exchangeRateService.GetRate(currency);
        if (exchangeRate <= 0) {
            throw new ValidationException($"{currency.Name}汇率值不合法!");
        }

        decimal totalAmount = order.ArrearsAmountFormula;
        string totalAmountCurrency = (totalAmount / exchangeRate).ToString("#0.00");

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo {
            Order = order,
            Customer = order.Customer,
            PayType = PayType.PayPal,
            OutTradeNo = outTradeNo,
            PayPalPaymentId = string.Empty, // 后面更新
            Amount = totalAmount,
            Status = PayStatus.NotPaid
        };
        session.Save(orderPay);
        session.Flush();

        Payment payment = new Payment() {
            intent = "sale",
            payer = new Payer() { payment_method = param.PaymentMethod.Code },
            redirect_urls = new RedirectUrls() {
                return_url = param.ReturnUrl,
                cancel_url = param.CancelUrl,
            },
            transactions = new List<Transaction>() {
                new Transaction() {
                    description = $"订单{order.Code}_{orderPay.Id}",
                    invoice_number = outTradeNo,
                    amount = new Amount() {
                        currency = currency.Code,
                        total = totalAmountCurrency, // Total must be equal to sum of shipping, tax and subtotal.
                        details = new Details() {
                            tax = "0",
                            shipping = "0",
                            subtotal = totalAmountCurrency
                        }
                    },
                    item_list = new ItemList() {
                        items = new List<Item>() {
                            new Item() {
                                name = $"订单{order.Code}_{orderPay.Id}",
                                currency = currency.Code,
                                price = totalAmountCurrency,
                                quantity = "1",
                                sku = $"{order.Code}_{orderPay.Id}",
                            }
                        }
                    }
                }
            }
        };

        Payment createdPayment = payment.Create(PaypalUtil.GetApiContext());

        orderPay.PayPalPaymentId = createdPayment.id;
        session.Update(orderPay);
        session.Flush();

        try {
            // 记录生成的 Payment 用于调试和改进
            log.Warn("Payment Created 数据:");
            log.Warn(JsonConvert.SerializeObject(createdPayment));
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        return createdPayment.GetApprovalUrl();
    }

    public void Process() {
    }
}

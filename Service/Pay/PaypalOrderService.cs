using Newtonsoft.Json;
using NLog;
using PayPal.Api;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly ExchangeRateService exchangeRateService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderFeeSumService logisticsOrderFeeSumService;
    private readonly CustomerScoreDeductService customerScoreDeductService;

    public PaypalOrderService(
        ISession session,
        PayService payService,
        ExchangeRateService exchangeRateService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        CouponService couponService,
        LogisticsOrderFeeSumService logisticsOrderFeeSumService,
        CustomerScoreDeductService customerScoreDeductService
    ) {
        this.session = session;
        this.payService = payService;
        this.exchangeRateService = exchangeRateService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.couponService = couponService;
        this.logisticsOrderFeeSumService = logisticsOrderFeeSumService;
        this.customerScoreDeductService = customerScoreDeductService;
    }

    public string Create(PaypalOrderCreateParam param) {
        ValidationUtil.Validate<PaypalOrderCreateParamValidator, PaypalOrderCreateParam>(param);

        int orderId = param.OrderId;
        int couponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        CouponInfo? coupon = null;
        if (couponId > 0) {
            coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null) {
                throw new ValidationException($"指定的优惠券{couponId}不存在！");
            }
        }

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }

        CurrencyType currency = order.Country.Currency;
        decimal exchangeRate = exchangeRateService.GetRate(currency);
        if (exchangeRate <= 0) {
            throw new ValidationException($"{currency.Name}汇率值不合法!");
        }

        var orderFeeSumData = logisticsOrderFeeSumService.GetSumData(orderId);

        decimal discountAmount = 0;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, orderFeeSumData.FirstModeFeeNoPromotion);
            if (discountResult.Success == false) {
                discountAmount = 0;
            } else {
                discountAmount = discountResult.Discount;

                // 保存优惠券到订单，并生成响应的订单费用，负的费用
                int customerId = order.Customer.Id;
                couponService.SaveToOrder(customerId: customerId, orderId: orderId, couponId: couponId);
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        if (param.ScoreDeduct) {
            var scoreDeductResult = customerScoreDeductService.Deduct(orderId, Math.Min(arrearsAmount, orderFeeSumData.FirstModeFeeNoPromotion));
            arrearsAmount -= scoreDeductResult.Amount;
        }

        if (arrearsAmount <= 0) {
            // 如果欠费金额是0，就不用生成微信支付了，直接成功支付就可以了
            var orderPayZeroAmount = new LogisticsOrderPayInfo {
                Order = order,
                OrderCode = order.Code,
                Customer = order.Customer,
                PayType = PayType.Paypal,
                OutTradeNo = string.Empty,
                Amount = 0, // 特殊情况的时候：订单用户原来支付过了一些，第二次支付的时候获得了优惠券折扣，这个折扣正好完全抵掉了欠款，这时就会生成一个支付金额为0的记录，同时把订单标记为已支付。
                Status = PayStatus.Paid
            };

            session.Save(orderPayZeroAmount);
            session.Flush();

            logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId); // 这个这里可以不调用，因为 couponService.SaveToOrder()里面已经调用过一次了

            return string.Empty; // 无需支付返回空的url
        }

        string arrearsAmountCurrency = (arrearsAmount / exchangeRate).ToString("#0.00");

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo {
            Order = order,
            OrderCode = order.Code,
            Customer = order.Customer,
            PayType = PayType.Paypal,
            OutTradeNo = outTradeNo,
            PayPalPaymentId = string.Empty, // 后面更新
            Amount = arrearsAmount,
            Status = PayStatus.NotPaid,
            Description = $"币种:{currency.Name},金额:{arrearsAmountCurrency}{currency.Code}",
        };
        session.Save(orderPay);
        session.Flush();

        Payment payment = new Payment {
            intent = "sale",
            payer = new Payer {
                payment_method = param.PaymentMethod.Code
            },
            redirect_urls = new RedirectUrls {
                return_url = param.ReturnUrl,
                cancel_url = param.CancelUrl,
            },
            transactions = new List<Transaction> {
                new Transaction {
                    description = $"订单{order.Code}_{orderPay.Id}",
                    invoice_number = outTradeNo,
                    amount = new Amount {
                        currency = currency.Code,
                        total = arrearsAmountCurrency, // Total must be equal to sum of shipping, tax and subtotal.
                        details = new Details {
                            tax = "0",
                            shipping = "0",
                            subtotal = arrearsAmountCurrency
                        }
                    },
                    item_list = new ItemList {
                        items = new List<Item> {
                            new Item {
                                name = $"订单{order.Code}_{orderPay.Id}",
                                currency = currency.Code,
                                price = arrearsAmountCurrency,
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <returns>payment id</returns>
    /// <exception cref="Exception"></exception>
    public string Process(PaypalOrderProcessParam param) {
        ValidationUtil.Validate<PaypalOrderProcessParamValidator, PaypalOrderProcessParam>(param);

        string payerId = param.PayerId;
        string outTradeNo = param.OutTradeNo;

        var orderPay = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();

        if (orderPay == null) {
            throw new Exception($"指定的待支付记录不存在, out trade no: {outTradeNo}!");
        }

        if (orderPay.Status != PayStatus.NotPaid) {
            throw new Exception($"指定的待支付记录不是未支付状态，或已经支付成功, out trade no: {outTradeNo}!");
        }

        string paymentId = orderPay.PayPalPaymentId;

        PaymentExecution paymentExecution = new PaymentExecution {
            payer_id = payerId
        };

        Payment payment = new Payment {
            id = paymentId
        };

        // Execute the payment.
        Payment executedPayment = payment.Execute(PaypalUtil.GetApiContext(), paymentExecution);

        log.Warn("PayPal订单支付已执行，executedPayment.ConvertToJson()");
        log.Warn(executedPayment.ConvertToJson());

        // 参考 https://stackoverflow.com/questions/51742413/paypal-api-payment-getstate
        if (executedPayment.transactions.Count == 0
            || executedPayment.transactions[0].related_resources.Count == 0
            || executedPayment.transactions[0].related_resources[0].sale.state != "completed") {
            throw new Exception($"PayPal支付已执行，舜达回调判断出错，状态判断失败! {executedPayment.ConvertToJson()}");
        }

        orderPay.Status = PayStatus.Paid;
        session.Update(orderPay);
        session.Flush();

        var order = orderPay.Order;
        int orderId = order.Id;

        PayStatus orderPayStatusBefore = order.PayStatus;

        // 合计标记支付状态
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        PayStatus orderPayStatusAfter = order.PayStatus;

        // 订单日志
        logisticsOrderActionService.NewActionWithoutUserCustomer(orderId, LogisticsOrderActionType.Pay, orderPayStatusBefore, orderPayStatusAfter, $"金额:{orderPay.Amount:#0.00)}, 支付单号:{orderPay.OutTradeNo}");

        // 会员积分
        customerScoreService.AddScore(orderPay.Id);

        return paymentId;
    }
}
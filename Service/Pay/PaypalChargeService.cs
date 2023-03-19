using Newtonsoft.Json;
using NLog;
using PayPal.Api;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly CustomerChargeService customerChargeService;
    private readonly ExchangeRateService exchangeRateService;

    public PaypalChargeService(
        ISession session,
        PayService payService,
        CustomerChargeService customerChargeService,
        ExchangeRateService exchangeRateService
    ) {
        this.session = session;
        this.payService = payService;
        this.customerChargeService = customerChargeService;
        this.exchangeRateService = exchangeRateService;
    }

    public string Create(PaypalChargeCreateParam param) {
        ValidationUtil.Validate<PaypalChargeCreateParamValidator, PaypalChargeCreateParam>(param);

        int customerId = param.CustomerId;
        decimal amount = param.Amount;

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的充值客户#{customerId}不存在！");
        }

        CurrencyType currency = param.Currency;
        decimal exchangeRate = exchangeRateService.GetRate(currency);
        if (exchangeRate <= 0) {
            throw new ValidationException($"{currency.Name}汇率值不合法!");
        }

        string amountCurrency = (amount / exchangeRate).ToString("#0.00");

        string outTradeNo = payService.GenerateOutTradeNo();

        var customerCharge = new CustomerChargeInfo {
            ChargeType = CustomerChargeType.PayPal,
            Customer = customer,
            OutTradeNo = outTradeNo,
            Amount = amount,
            Status = CustomerChargeStatus.New
        };

        session.Save(customerCharge);
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
                    description = $"会员充值_{customerId}_{customerCharge.Id}",
                    invoice_number = outTradeNo,
                    amount = new Amount {
                        currency = currency.Code,
                        total = amountCurrency, // Total must be equal to sum of shipping, tax and subtotal.
                        details = new Details {
                            tax = "0",
                            shipping = "0",
                            subtotal = amountCurrency
                        }
                    },
                    item_list = new ItemList {
                        items = new List<Item> {
                            new Item {
                                name = $"会员充值_{customerId}_{customerCharge.Id}",
                                currency = currency.Code,
                                price = amountCurrency,
                                quantity = "1",
                                sku = $"charge_{customerId}_{customerCharge.Id}",
                            }
                        }
                    }
                }
            }
        };

        Payment createdPayment = payment.Create(PaypalUtil.GetApiContext());

        customerCharge.PaypalPaymentId = createdPayment.id;
        session.Update(customerCharge);
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

    public void Process(PaypalChargeProcessParam param) {
        ValidationUtil.Validate<PaypalChargeProcessParamValidator, PaypalChargeProcessParam>(param);
    }
}
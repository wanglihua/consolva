using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Newtonsoft.Json;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class AlipayChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly CustomerChargeService customerChargeService;

    public AlipayChargeService(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        CustomerChargeService customerChargeService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
        this.customerChargeService = customerChargeService;
    }

    /// <summary>
    /// 创建支付宝支付返回 alipay request url
    /// </summary>
    /// <param name="param">参数对象</param>
    /// <returns>alipay request url 用于跳转支付宝支付</returns>
    public string Create(AlipayChargeCreateParam param) {
        int customerId = param.CustomerId;
        decimal amount = param.Amount;

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的充值客户#{customerId}不存在！");
        }

        if (amount <= 0) {
            throw new ValidationException("充值金额不能为负或0！");
        }

        string outTradeNo = payService.GenerateOutTradeNo();

        var customerCharge = new CustomerChargeInfo {
            ChargeType = CustomerChargeType.Wechat,
            Customer = customer,
            OutTradeNo = outTradeNo,
            Amount = amount,
            Status = CustomerChargeStatus.New
        };

        session.Save(customerCharge);
        session.Flush();

        // 订单名称
        string subject = $"会员充值_{customerId}_{customerCharge.Id}";

        // 商品描述
        string itemBody = $"会员充值_{customerId}_{customerCharge.Id}";

        if (param.IsMobile == false) {
            // 组装业务参数model
            AlipayTradePagePayModel payModel = new AlipayTradePagePayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = amount.ToString("#0.00"),
                OutTradeNo = outTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest payRequest = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.ReturnUrl);
            // 设置异步通知接收地址
            payRequest.SetNotifyUrl(param.NotifyUrl);
            // 将业务model载入到request
            payRequest.SetBizModel(payModel);

            var response = alipayService.SdkExecute(payRequest);

            log.Warn("支付宝支付发起 end");

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return alipayRequestUrl;
        } else {
            // 组装业务参数model
            AlipayTradeWapPayModel payModel = new AlipayTradeWapPayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = amount.ToString("#0.00"),
                OutTradeNo = outTradeNo,
                ProductCode = "QUICK_WAP_WAY"
            };

            AlipayTradeWapPayRequest payRequest = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.ReturnUrl);
            // 设置异步通知接收地址
            payRequest.SetNotifyUrl(param.NotifyUrl);
            // 将业务model载入到request
            payRequest.SetBizModel(payModel);

            var response = alipayService.SdkExecute(payRequest);

            log.Warn("支付宝支付发起 end");

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return alipayRequestUrl;
        }
    }

    public void Callback(Dictionary<string, string> callbackDataDict) {
        try {
            log.Error("支付宝支付回调 数据: " + JsonConvert.SerializeObject(callbackDataDict));
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        if (callbackDataDict.Count == 0) {
            throw new Exception("支付宝支付回调 end fail_001");
        }

        bool checkSuccess = alipayService.RSACheckV1(callbackDataDict);
        if (checkSuccess == false) {
            throw new Exception("支付宝支付回调 end fail_003");
        }

        string outTradeNo = callbackDataDict["out_trade_no"];
        if (string.IsNullOrWhiteSpace(outTradeNo)) {
            throw new Exception("支付宝支付回调 out_trade_no 为空！");
        }

        var customerCharge = session.Query<CustomerChargeInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (customerCharge == null) {
            throw new Exception($"out_trade_no 本地系统客户充值支付记录中不存在。out_trade_no：{outTradeNo}");
        }

        if (callbackDataDict.ContainsKey("total_amount") == false) {
            throw new Exception($"total_amount 参数未提供，out_trade_no: {outTradeNo}");
        }

        if (decimal.TryParse(callbackDataDict["total_amount"], out decimal payAmount) == false) {
            throw new Exception($"total_amount 参数格式错误，out_trade_no: {outTradeNo}");
        }

        if (customerCharge.Status == CustomerChargeStatus.New) {
            // 完成充值
            customerChargeService.CompleteCharge(customerCharge, payAmount, CustomerChargeType.Wechat);
        }
    }
}
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 国际快递单号提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageInternationalExpressTrackingNoService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageInternationalExpressTrackingNoService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.InternationalExpressTrackingNo;

    public static void InitSetting(ISession session) {
        var setting = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            .Where(e => e.Type == messageType)
            .Take(1)
            .SingleOrDefault();
        if (setting == null) {
            setting = new CustomerMessageRealTimeSendSettingInfo {
                Type = CustomerMessageType.OrderDelivered,
                Status = CustomerMessageSettingStatus.Disabled,
                WechatSend = true,
                SmsSend = false,
                WechatMessageTemplate1 = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}",
                SmsMessageTemplate = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}",
                Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，国际快递公司：{express_company}，国际快递跟踪网址：{tracking_url}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }
}
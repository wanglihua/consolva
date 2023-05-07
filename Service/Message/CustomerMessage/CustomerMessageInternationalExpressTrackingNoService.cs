using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
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
                Type = messageType,
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

    private string TransferMessage(string message, string orderCode, string trackingNo, string expressCompany, string trackingUrl) {
        if (string.IsNullOrWhiteSpace(message)) {
            return message;
        }

        message = message.Replace("{order_code}", orderCode);
        message = message.Replace("{tracking_no}", trackingNo);
        message = message.Replace("{express_company}", expressCompany);
        message = message.Replace("{tracking_url}", trackingUrl);

        return message;
    }

    public string GetUrl(int orderId) {
        return $"pages/order/order_detail?id={orderId}";
    }

    public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string expressCompany, string trackingUrl) {
        if (customer.Status == CustomerStatus.Frozen) {
            return;
        }

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled) {
            if (customer == null) {
                throw new ValidationException("消息发送指定的会员不能为空！");
            }

            if (order == null) {
                throw new ValidationException("消息发送指定的物流订单不能为空！");
            }

            string customerCode = customerCodeService.GetCustomerCode(customer.Id);
            string orderCode = order.Code;

            string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode, trackingNo, expressCompany, trackingUrl);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode, trackingNo, expressCompany, trackingUrl);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode, trackingNo, expressCompany, trackingUrl);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }
            
            if (clientShouldSend && (setting.WechatSend || setting.SmsSend)) {
                messageStatus = CustomerMessageStatus.Success;
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    string url = GetUrl(order.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageType.Name, orderCode, customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageType.Name, orderCode, customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageType.Name, orderCode, customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, trackingNo, expressCompany, trackingUrl));
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message2) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message3) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, messageStatus, setting, processUser);
            }

            session.Flush();
        }
    }
}
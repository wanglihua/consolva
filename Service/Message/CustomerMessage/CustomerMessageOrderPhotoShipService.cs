using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 订单发货图片推送
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageOrderPhotoShipService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageOrderPhotoShipService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.OrderPhotoShip;

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
                WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单详情查看！",
                SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单详情查看！",
                Placeholders = "订单编码：{order_code}，图片类型：{photo_type}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string orderCode) {
        if (string.IsNullOrWhiteSpace(message)) {
            return string.Empty;
        }

        message = message.Replace("{order_code}", orderCode);
        message = message.Replace("{photo_type}", ExpressPhotoType.Ship.Name);

        return message;
    }

    public string GetUrl(int orderId) {
        return $"pages/order/order_detail?id={orderId}";
    }

    public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order) {
        if (customer == null) {
            throw new ValidationException("消息发送指定的会员不能为空！");
        }

        if (order == null) {
            throw new ValidationException("消息发送指定的订单不能为空！");
        }

        int orderId = order.Id;
        string orderCode = order.Code;

        if (customer.Status == CustomerStatus.Frozen) {
            return;
        }

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled) {
            string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            if (clientShouldSend && (setting.WechatSend || setting.SmsSend)) {
                messageStatus = CustomerMessageStatus.Success;
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            int message1Id = 0;
            if (string.IsNullOrWhiteSpace(message1) == false) {
                message1Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, orderId, message1, messageStatus, setting, processUser);
            }

            int message2Id = 0;
            if (string.IsNullOrWhiteSpace(message2) == false) {
                message2Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, orderId, message2, messageStatus, setting, processUser);
            }

            int message3Id = 0;
            if (string.IsNullOrWhiteSpace(message3) == false) {
                message3Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, orderId, message3, messageStatus, setting, processUser);
            }

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message1Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message2Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message3Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }
        }
    }
}
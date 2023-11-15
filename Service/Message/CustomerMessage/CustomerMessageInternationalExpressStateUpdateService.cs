using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 国际快递更新提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageInternationalExpressStateUpdateService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageInternationalExpressStateUpdateService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.InternationalExpressStateUpdate;


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
                WechatMessageTemplate1 = "您的 订单{order_code} 跟踪单号{tracking_no}，有新的状态更新：{tracking_content}",
                SmsMessageTemplate = "您的 订单{order_code} 跟踪单号{tracking_no}，有新的状态更新：{tracking_content}",
                Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，状态更新内容：{tracking_content}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string orderCode, string trackingNo, string trackingContent) {
        if (string.IsNullOrWhiteSpace(message)) {
            return message;
        }

        message = message.Replace("{order_code}", orderCode);
        message = message.Replace("{tracking_no}", trackingNo);
        message = message.Replace("{tracking_content}", trackingContent);

        return message;
    }

    public string GetUrl(int orderId) {
        return $"/pages/order/order_detail?id={orderId}";
    }

    public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string trackingContent) {
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

            int orderId = order.Id;
            string orderCode = order.Code;

            string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode, trackingNo, trackingContent);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode, trackingNo, trackingContent);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode, trackingNo, trackingContent);

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
                message1Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, orderId, message1, messageStatus, setting, processUser);
            }

            int message2Id = 0;
            if (string.IsNullOrWhiteSpace(message2) == false) {
                message2Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, orderId, message2, messageStatus, setting, processUser);
            }

            int message3Id = 0;
            if (string.IsNullOrWhiteSpace(message3) == false) {
                message3Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, orderId, message3, messageStatus, setting, processUser);
            }

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    string messageTitle = "订单跟踪状态更新提醒";
                    if (order.OrderCate == LogisticsOrderCate.Air) {
                        messageTitle = "空运订单跟踪状态更新提醒";
                    }

                    if (order.OrderCate == LogisticsOrderCate.Sea) {
                        messageTitle = "海运订单跟踪状态更新提醒";
                    }

                    messageTitle = customerMessageCommonService.GetMessageTitle(messageType, messageTitle);
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message1Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, orderCode, customerCode, messageTitle, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message2Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, orderCode, customerCode, messageTitle, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message3Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, orderCode, customerCode, messageTitle, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, trackingNo, trackingContent));
                }
            }
        }
    }
}
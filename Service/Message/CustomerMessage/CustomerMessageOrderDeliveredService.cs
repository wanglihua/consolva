using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 空运包裹签收提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageOrderDeliveredService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageOrderDeliveredService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.OrderDelivered;


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
                WechatMessageTemplate1 = "亲，您的空运订单{order_code}，显示已妥投签收，如果有问题请联系空运客服。另外，分享微信朋友圈积攒10个以上，联系空运客服可得20RMB空运优惠券！",
                SmsMessageTemplate = "亲，您的空运订单{order_code}，显示已妥投签收，如果有问题请联系空运客服。另外，分享微信朋友圈积攒10个以上，联系空运客服可得20RMB空运优惠券！",
                Placeholders = "订单编码：{order_code}",
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

        // setting.Placeholders = "订单编码：{order_code}";
        message = message.Replace("{order_code}", orderCode);

        return message;
    }

    public string GetUrl(int orderId) {
        return $"pages/order/order_detail?id={orderId}";
    }

    /// <summary>
    /// 
    /// 空运包裹签收提醒
    /// 
    /// <param name="order">物流订单</param>
    /// 
    /// </summary>
    public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order) {
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
                    customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode));
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
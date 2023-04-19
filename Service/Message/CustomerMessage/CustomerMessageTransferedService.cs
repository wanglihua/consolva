using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 已转运提醒
/// todo 还没调用
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageTransferedService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageTransferedService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.Transfered;

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
                WechatMessageTemplate1 = "亲，您的{order_code}订单现已转运！",
                SmsMessageTemplate = "亲，您的{order_code}订单现已转运！",
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

    private string GetUrl(int orderId) {
        return $"pages/order/order_detail?id={orderId}";
    }

    /// <summary>
    /// 已转运提醒
    /// <param name="order">物流订单</param>
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

            string orderCode = order.Code;

            string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);
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
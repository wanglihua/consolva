using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 账户余额变动提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageAccountChangeService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageAccountChangeService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.AccountChange;

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
                WechatMessageTemplate1 = "您好，您的账户余额有变动，变动原因：{change_reason}，变动金额{account_change_amount}，变动前余额{account_before_amount}，变动后余额{account_after_amount}！",
                SmsMessageTemplate = "您好，您的账户余额有变动，变动原因：{change_reason}，变动金额{account_change_amount}，变动前余额{account_before_amount}，变动后余额{account_after_amount}！",
                Placeholders = "变动原因：{change_reason}，账户余额变动金额：{account_change_amount}，账户变动前余额：{account_before_amount}，账户变动后余额：{account_after_amount}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };


            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string changeReason, decimal accountChangeAmount, decimal accountBeforeAmount, decimal accountAfterAmount) {
        if (string.IsNullOrWhiteSpace(message)) {
            return message;
        }

        message = message.Replace("{change_reason}", changeReason);
        message = message.Replace("{account_change_amount}", accountChangeAmount.ToString("#0.00"));
        message = message.Replace("{account_before_amount}", accountBeforeAmount.ToString("#0.00"));
        message = message.Replace("{account_after_amount}", accountAfterAmount.ToString("#0.00"));

        return message;
    }

    private string GetUrl() {
        return "pages/customer/customer_account/customer_account";
    }

    public void SendMessage(CustomerInfo customer, string changeReason, decimal accountChangeAmount, decimal accountBeforeAmount, decimal accountAfterAmount) {
        if (customer == null) {
            throw new ValidationException("消息发送指定的会员不能为空！");
        }

        if (customer.Status == CustomerStatus.Frozen) {
            return;
        }

        string customerCode = customerCodeService.GetCustomerCode(customer.Id);

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled) {
            string message1 = TransferMessage(setting.WechatMessageTemplate1, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    string url = GetUrl();
                    string messageTitle = "账户余额变动提醒";

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message2) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message3) == false) {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, messageStatus, setting, processUser);
            }

            session.Flush();
        }
    }
}
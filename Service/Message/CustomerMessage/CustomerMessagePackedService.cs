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
/// 空运已打包提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessagePackedService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerMessageClientUrlService customerMessageClientUrlService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessagePackedService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerMessageClientUrlService customerMessageClientUrlService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerMessageClientUrlService = customerMessageClientUrlService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.Packed;

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
                WechatMessageTemplate1 = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。你可以通过订单详情查看打包数据及计费明细。友情提醒：如您对费用有异议，请联系您的专属客服！如无异议，请及时支付费用！",
                SmsMessageTemplate = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。如对费用有异议，请联系客服！并请及时支付费用！",
                Placeholders = "订单编码：{order_code}，总费用：{total_fee}，费用计算描述：{fee_cal_desc}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string orderCode, string totalFee, string feeCalDesc) {
        if (string.IsNullOrWhiteSpace(message)) {
            return message;
        }

        message = message.Replace("{order_code}", orderCode);
        message = message.Replace("{total_fee}", totalFee);
        message = message.Replace("{fee_cal_desc}", feeCalDesc);

        return message;
    }

    /// <summary>
    /// 空运已打包提醒
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
            string totalFee = order.OrderAmount.ToString("#0.00");
            string feeCalDesc = order.FirstMode.FeeCalDesc ?? string.Empty;

            string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode, totalFee, feeCalDesc);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode, totalFee, feeCalDesc);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode, totalFee, feeCalDesc);

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
                    string url = customerMessageClientUrlService.GetUrl(TerminalType.WechatApp, messageType);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    messageStatus = CustomerMessageStatus.Success;
                    customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, totalFee, feeCalDesc));
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
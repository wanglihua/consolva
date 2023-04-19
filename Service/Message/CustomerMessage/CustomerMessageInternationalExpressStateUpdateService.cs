using NHibernate;
using shunda.Core.Service;
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
                Type = CustomerMessageType.OrderDelivered,
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
}

/*


        #region 国际快递状态更新提醒

        /// <summary>
        /// 
        /// 国际快递状态更新提醒
        /// 
        /// </summary>
        public void SendMessageInternationalExpressStateUpdate(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string trackingContent) {
            if (customer?.Status == CustomerStatus.Frozen) {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.InternationalExpressStateUpdateTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled) {
                if (customer == null) {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (order == null) {
                    throw new ValidationException("消息发送指定的物流订单不能为空！");
                }

                string orderCode = order?.Code;

                string message1 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate1, orderCode, trackingNo, trackingContent);
                string message2 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate2, orderCode, trackingNo, trackingContent);
                string message3 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate3, orderCode, trackingNo, trackingContent);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend)) {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.InternationalExpressStateUpdateTiXing);
                }

                if (setting.WeiXinSend) {
                    if (clientShouldSend) {
                        isSent = true;

                        string orderDetailUrl = customerClientUrlService.GetTransportingOrderDetailUrl(order.Customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false) {
                            weiXinMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message1, defaultProcessUser, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false) {
                            weiXinMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message2, defaultProcessUser, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false) {
                            weiXinMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message3, defaultProcessUser, orderDetailUrl);
                        }
                    }
                }

                if (setting.SmsSend) {
                    if (clientShouldSend) {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageInternationalExpressStateUpdate(setting.SmsMessageTemplate, orderCode, trackingNo, trackingContent));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false) {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false) {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false) {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageInternationalExpressStateUpdate(string message, string orderCode, string trackingNo, string trackingContent) {
            if (string.IsNullOrWhiteSpace(message)) {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，状态更新内容：{tracking_content}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{tracking_content}", trackingContent);

            return message;
        }

        #endregion
        
*/
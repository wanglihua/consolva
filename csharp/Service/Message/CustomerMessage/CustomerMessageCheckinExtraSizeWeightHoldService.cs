using Core;
using Domain.Customer;
using Domain.Message;
using NHibernate;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 入库超大超重HOLD通知
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageCheckinExtraSizeWeightHoldService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;

        public CustomerMessageCheckinExtraSizeWeightHoldService(
            ISession session,
            CustomerMessageCommonService customerMessageCommonService)
        {
            this.session = session;
            this.customerMessageCommonService = customerMessageCommonService;
        }

        /// <summary>
        /// 
        /// 入库超大超重HOLD通知
        /// 
        /// <param name="trackingNo">快递单号</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.CheckinExtraSizeWeightHoldTongZhi;
            CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo) == true)
                {
                    throw new ValidationException("消息发送指定的快递单号不能为空！");
                }



                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer?.Id ?? 0, messageType);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = customerMessageCommonService.GetProcessUser();

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", trackingNo, customer.CustomerCode, message1, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", trackingNo, customer.CustomerCode, message2, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", trackingNo, customer.CustomerCode, message3, processUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessage(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}";
            message = message.Replace("{tracking_no}", trackingNo);

            return message;
        }
    }
}
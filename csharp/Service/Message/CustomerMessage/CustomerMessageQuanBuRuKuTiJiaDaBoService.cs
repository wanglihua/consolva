using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 全部入库提交打包提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageQuanBuRuKuTiJiaDaBoService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;

        public CustomerMessageQuanBuRuKuTiJiaDaBoService(
            ISession session,
            CustomerMessageCommonService customerMessageCommonService)
        {
            this.session = session;
            this.customerMessageCommonService = customerMessageCommonService;
        }

        /// <summary>
        /// 
        /// 全部入库提交打包提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.QuanBuRuKuTiJiaDaBoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (order == null)
                {
                    throw new ValidationException("消息发送指定的物流订单不能为空！");
                }

                string orderCode = order.Code;

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode);

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
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessage(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}";
            message = message.Replace("{order_code}", orderCode);

            return message;
        }
    }
}
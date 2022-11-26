using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 已打包提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageYiDaBaoService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageYiDaBaoService(
            ISession session,
            CustomerMessageCommonService customerMessageCommonService,
            CustomerClientUrlService customerClientUrlService)
        {
            this.session = session;
            this.customerMessageCommonService = customerMessageCommonService;
            this.customerClientUrlService = customerClientUrlService;
        }
        
        /// <summary>
        /// 
        /// 已打包提醒
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

            CustomerMessageType messageType = CustomerMessageType.YiDaBaoTiXing;
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
                string totalFee = order.OrderAmount.ToString("#0.00");

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, totalFee);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, totalFee);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, totalFee);

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
                        string url = customerClientUrlService.GetWaitTransportOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, totalFee));
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

        private string TransferMessage(string message, string orderCode, string totalFee)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，总费用：{total_fee}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{total_fee}", totalFee);

            return message;
        }        
    }
}
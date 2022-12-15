using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 已转运提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageYiZhuanYunService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerMessageClientUrlService customerMessageClientUrlService = null;

        public CustomerMessageYiZhuanYunService(
            ISession session,
            CustomerMessageCommonService customerMessageCommonService,
            CustomerMessageClientUrlService customerMessageClientUrlService)
        {
            this.session = session;
            this.customerMessageCommonService = customerMessageCommonService;
            this.customerMessageClientUrlService = customerMessageClientUrlService;
        }
        
        /// <summary>
        /// 
        /// 已转运提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode, string arriveTime)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YiZhuanYunTiXing;
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

                string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCode, customerShipContainerCode, arriveTime);
                string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCode, customerShipContainerCode, arriveTime);
                string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCode, customerShipContainerCode, arriveTime);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer?.Id ?? 0, messageType);
                }

                if (setting.WechatSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = customerMessageCommonService.GetProcessUser();
                        string url = customerMessageClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, arriveTime));
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

        private string TransferMessage(string message, string orderCode, string customerShipContainerCode, string arriveTime)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，到港时间：{arrive_time}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{arrive_time}", arriveTime);

            return message;
        }        
    }
}
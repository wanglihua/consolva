using Core;
using Domain.Customer;
using Domain.Deliver;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 取货点交付评价推送
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessagePickupStationDeliverRateService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessagePickupStationDeliverRateService(
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
        /// 取货点交付评价推送
        ///
        /// <param name="customer"></param>
        /// <param name="order">订单</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.PickupStationDeliverRateTuiSong;
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
                string pickupStationName = order.SecondLogisticsMode?.PickupStation?.Name;

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, pickupStationName);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, pickupStationName);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, pickupStationName);

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
                        string url = customerClientUrlService.GetPickupStationDeliverRateUrl(customer, order);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, pickupStationName));
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

        private string TransferMessage(string message, string orderCode, string pickupStationName)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，取货点：{pickup_station_name}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{pickup_station_name}", pickupStationName);

            return message;
        }
    }
}
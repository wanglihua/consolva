using System.Collections.Generic;
using System.Linq;
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
    /// 送货交付图片/评价推送
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageHomeDeliverPhotoRateService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageHomeDeliverPhotoRateService(
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
        /// 送货交付图片/评价推送
        ///
        /// <param name="customer">客户s</param>
        /// <param name="batch">送货批次</param> 
        /// <param name="orderList">物流订单列表</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, HomeDeliverBatchInfo batch, IList<LogisticsOrderInfo> orderList)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverPhotoRateTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (batch == null)
                {
                    throw new ValidationException("消息发送送货批次不能为空！");
                }

                if (orderList == null || orderList.Count == 0)
                {
                    throw new ValidationException("消息发送指定的物流订单不能为空！");
                }

                var firstOrder = orderList.First();
                string firstOrderCode = firstOrder.Code;

                // string orderCodes = string.Join(",", orderList.Select(e => e.Code)); 用下面的方式，消息显示时显得更美观

                string orderCodes = null;
                if (orderList.Count > 1)
                {
                    orderCodes = $"{firstOrderCode}...共{orderList.Count}个订单";
                }
                else
                {
                    orderCodes = $"{firstOrderCode}共1个订单";
                }

                string driverName = batch.DriverName;

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCodes, driverName);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCodes, driverName);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCodes, driverName);

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
                        string url = customerClientUrlService.GetHomeDeliverBatchRateUrl(customer, batch);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", firstOrderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", firstOrderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", firstOrderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCodes, driverName));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessage(string message, string orderCode, string driverName)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，司机姓名：{driver_name}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{driver_name}", driverName);

            return message;
        }
    }
}
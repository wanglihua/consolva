using System.Collections.Generic;
using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 送货上门时间推送
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageHomeDeliverTimeService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageHomeDeliverTimeService(
            ISession session,
            CustomerMessageCommonService customerMessageCommonService,
            CustomerClientUrlService customerClientUrlService)
        {
            this.session = session;
            this.customerMessageCommonService = customerMessageCommonService;
            this.customerClientUrlService = customerClientUrlService;
        }

        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo firstOrder, IList<string> orderCodeList,
            string timeBegin, string timeEnd, string driverName, string driverPhone)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverTimeTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status != CustomerMessageSettingStatus.Enabled)
            {
                return;
            }

            if (customer == null)
            {
                throw new ValidationException("消息发送指定的会员不能为空！");
            }

            if (firstOrder == null)
            {
                throw new ValidationException("送货上门时间推送 订单不能为空！");
            }

            if (orderCodeList == null || orderCodeList.Count == 0)
            {
                throw new ValidationException("送货上门时间推送 订单编号列表不能为空！");
            }

            string orderCode = firstOrder.Code; // string.Join(", ", orderCodeList);

            if (string.IsNullOrWhiteSpace(timeBegin))
            {
                throw new ValidationException("送货上门时间推送 送货时间段开始时间不能为空！");
            }

            if (string.IsNullOrWhiteSpace(timeEnd))
            {
                throw new ValidationException("送货上门时间推送 送货时间段结束时间不能为空！");
            }              

            if (string.IsNullOrWhiteSpace(driverName))
            {
                throw new ValidationException("送货上门时间推送 司机姓名不能为空！");
            }

            if (string.IsNullOrWhiteSpace(driverPhone))
            {
                throw new ValidationException("送货上门时间推送 司机电话不能为空！");
            }

            string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, orderCodeList.Count, timeBegin, timeEnd, driverName, driverPhone);
            string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, orderCodeList.Count, timeBegin, timeEnd, driverName, driverPhone);
            string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, orderCodeList.Count, timeBegin, timeEnd, driverName, driverPhone);

            bool isSent = false;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer?.Id ?? 0, CustomerMessageType.HomeDeliverTimeTuiSong);
            }

            if (setting.WeiXinSend)
            {
                if (clientShouldSend)
                {
                    isSent = true;

                    string processUser = customerMessageCommonService.GetProcessUser();
                    string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, firstOrder.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "送货上门时间推送", orderCode, customer.CustomerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "送货上门时间推送", orderCode, customer.CustomerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "送货上门时间推送", orderCode, customer.CustomerCode, message3, processUser, url);
                    }
                }
            }

            if (setting.SmsSend)
            {
                if (clientShouldSend)
                {
                    isSent = true;

                    customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, orderCodeList.Count, timeBegin, timeEnd, driverName, driverPhone));
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

        private string TransferMessage(string message, string orderCode, int orderCount,
            string timeBegin, string timeEnd, string driverName, string driverPhone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // 订单：{order_code}，订单数量：{order_count}，
            // 送货时间段开始：{time_begin}，送货时间段结束：{time_end}，
            // 司机姓名：{driver_name}，司机电话：{driver_phone}
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{order_count}", orderCount.ToString());
            message = message.Replace("{time_begin}", timeBegin);
            message = message.Replace("{time_end}", timeEnd);
            message = message.Replace("{driver_name}", driverName);
            message = message.Replace("{driver_phone}", driverPhone);
            return message;
        }
    }
}
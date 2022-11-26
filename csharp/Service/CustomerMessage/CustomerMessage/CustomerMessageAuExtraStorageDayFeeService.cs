using System;
using System.Linq;
using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 待支付仓储费通知
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageAuExtraStorageDayFeeService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageAuExtraStorageDayFeeService(
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
        /// 待支付仓储费通知
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

            CustomerMessageType messageType = CustomerMessageType.AuExtraStorageDayFeeTongZhi;
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
                string feeDate = DateTime.Now.AddDays(-1).ToString("M月d日"); // 发送消息时的上一天

                int orderId = order.Id;
                var extraStorageFeeList = session.Query<LogisticsOrderFeeInfo>()
                    .Where(e => e.Order.Id == orderId
                                && e.Type == LogisticsOrderFeeType.SecondLogisticsFeeExtraStorageDay)
                    .ToList();

                decimal storageFee = 0m;
                if (extraStorageFeeList != null && extraStorageFeeList.Count != 0)
                {
                    storageFee = extraStorageFeeList.Sum(e => e.Fee);
                }

                if (storageFee <= 0)
                {
                    return; // 如果在这里发现实际没产生费用，就不发送消息了
                }

                string storageFeeStr = storageFee.ToString("0.00");

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, feeDate, storageFeeStr);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, feeDate, storageFeeStr);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, feeDate, storageFeeStr);

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
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, feeDate, storageFeeStr));
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

        private string TransferMessage(string message, string orderCode, string feeDate, string storageFee)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，计费日期（推送消息的前1天）：{fee_date}，仓储费(RMB)：{storage_fee}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{fee_date}", feeDate);
            message = message.Replace("{storage_fee}", storageFee);

            return message;
        }
    }
}
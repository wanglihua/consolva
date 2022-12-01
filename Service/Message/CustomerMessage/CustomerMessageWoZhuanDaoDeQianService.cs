using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 我赚到的钱 提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageWoZhuanDaoDeQianService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageWoZhuanDaoDeQianService(
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
        /// 我赚到的钱
        ///
        /// <param name="orderCate">订单类别，海运，空运</param>
        /// <param name="score">积分</param>
        /// <param name="gap">代差</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderCate orderCate, decimal score, int gap)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.WoZhuanDaoDeQianTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessage(setting.WechatMessageTemplate1, orderCate, score, gap);
                string message2 = TransferMessage(setting.WechatMessageTemplate2, orderCate, score, gap);
                string message3 = TransferMessage(setting.WechatMessageTemplate3, orderCate, score, gap);

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
                        string templateOrderCode = "积分变动通知";

                        if (orderCate == LogisticsOrderCate.Sea)
                        {
                            templateOrderCode = "海运积分变动通知";
                        }

                        if (orderCate == LogisticsOrderCate.Air)
                        {
                            templateOrderCode = "空运积分变动通知";
                        }

                        string defaultProcessUser = CustomerMessageCommonService.DefaultProcessUser;

                        string messageUrl = customerClientUrlService.GetCustomerScoreUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", templateOrderCode, customer.CustomerCode, message1, defaultProcessUser, messageUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", templateOrderCode, customer.CustomerCode, message2, defaultProcessUser, messageUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", templateOrderCode, customer.CustomerCode, message3, defaultProcessUser, messageUrl);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCate, score, gap));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessage(string message, LogisticsOrderCate orderCate, decimal score, int gap)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单类别(海运或空运)：{order_cate}，积分：{score}，代差：{gap}";

            string orderCateStr = "";

            if (orderCate == LogisticsOrderCate.Sea)
            {
                orderCateStr = "海运";
            }

            if (orderCate == LogisticsOrderCate.Air)
            {
                orderCateStr = "空运";
            }

            message = message.Replace("{order_cate}", orderCateStr);
            message = message.Replace("{score}", score.ToString("#0.00"));
            message = message.Replace("{gap}", gap.ToString());

            return message;
        }
    }
}
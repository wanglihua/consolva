using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 空运已打包提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageYiDaBaoAirService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageYiDaBaoAirService(
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
        /// 空运已打包提醒
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

            CustomerMessageType messageType = CustomerMessageType.OrderAirYiDaBaoTiXing;
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
                string feeCalDesc = order.FirstLogisticsMode?.FeeCalDesc ?? string.Empty;

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, totalFee, feeCalDesc);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, totalFee, feeCalDesc);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, totalFee, feeCalDesc);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, totalFee, feeCalDesc));
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

        private string TransferMessage(string message, string orderCode, string totalFee, string feeCalDesc)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，总费用：{total_fee}，费用计算描述：{fee_cal_desc}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{total_fee}", totalFee);
            message = message.Replace("{fee_cal_desc}", feeCalDesc);

            return message;
        }
    }
}
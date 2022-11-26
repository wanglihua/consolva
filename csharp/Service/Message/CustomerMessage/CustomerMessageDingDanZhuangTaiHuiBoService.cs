using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 订单状态回拨提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageDingDanZhuangTaiHuiBoService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageDingDanZhuangTaiHuiBoService(
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
        /// 订单状态回拨提醒
        ///
        /// <param name="customer"></param>
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// <param name="orderOldStatus">订单状态</param>
        /// <param name="orderPreStatus">订单上一个状态</param>
        /// <param name="orderStatusNextTime">订单下次状态时间</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode, string orderOldStatus, string orderPreStatus, string orderStatusNextTime)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.DingDanZhuangTaiHuiBoTiXing;
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

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime));
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

        private string TransferMessage(string message, string orderCode, string customerShipContainerCode, string orderOldStatus, string orderPreStatus, string orderStatusNextTime)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，订单状态：{order_old_status}，订单上一个状态：{order_pre_status}，订单下次状态时间：{order_status_next_time}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{order_old_status}", orderOldStatus);
            message = message.Replace("{order_pre_status}", orderPreStatus);
            message = message.Replace("{order_status_next_time}", orderStatusNextTime);

            return message;
        }
    }
}
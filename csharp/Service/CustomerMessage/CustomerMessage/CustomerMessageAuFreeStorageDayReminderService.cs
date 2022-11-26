using System.Linq;
using Core;
using Domain.Customer;
using Domain.Logistics;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using Service.Customer;

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 仓储费率提醒
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageAuFreeStorageDayReminderService
    {
        private ISession session = null;
        private CustomerMessageCommonService customerMessageCommonService = null;
        private CustomerClientUrlService customerClientUrlService = null;

        public CustomerMessageAuFreeStorageDayReminderService(
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
        /// 仓储费率提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessage(CustomerInfo customer, LogisticsOrderInfo order, int daysRemaining)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.AuFreeStorageDayReminderTiXing;
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

                // 二程类型不是送货上门，且二程方式为空的，不发送这个消息
                if (order.SecondLogisticsModeType != LogisticsModeType.HomeDelivery
                    && order.SecondLogisticsMode == null)
                {
                    return;
                }

                LogisticsModeInfo secondMode = order.SecondLogisticsMode;
                if (secondMode == null && order.SecondLogisticsModeType == LogisticsModeType.HomeDelivery)
                {
                    // 假设为最新的启用的送货上门物流方式
                    var homeDeliveryType = LogisticsModeType.HomeDelivery;
                    int targetWarehouseId = order.TargetWarehouse?.Id ?? 0;
                    var modeStatusEnabled = LogisticsModeStatus.Enabled;
                    secondMode = session.Query<LogisticsModeInfo>()
                        .Where(e => e.Type == homeDeliveryType
                                    && e.WarehouseTo.Id == targetWarehouseId
                                    && e.Status == modeStatusEnabled)
                        .OrderByDescending(e => e.Id)
                        .Take(1)
                        .SingleOrDefault();
                }

                if (secondMode == null)
                {
                    return;
                }

                string targetWarehouseOrPickupStation = string.Empty;
                if (order.SecondLogisticsModeType == LogisticsModeType.WarehousePickup
                    || order.SecondLogisticsModeType == LogisticsModeType.HomeDelivery)
                {
                    // 订单的目的仓库，二程的发货仓库
                    targetWarehouseOrPickupStation = order.TargetWarehouse?.Name ?? string.Empty; // 目的仓本身名称中就已经有仓库字样，所以后面就不用加上 "澳仓"或"仓库"资源。但取货点需要。 
                }
                else if (order.SecondLogisticsModeType == LogisticsModeType.TerminalPickup)
                {
                    targetWarehouseOrPickupStation = $"{secondMode.PickupStation?.Name ?? string.Empty} 取货点";
                }
                else
                {
                    return;
                }

                string storageFeeRate = $"{secondMode.FeeRate?.ExtraStorageDayFee ?? 0m} RMB/立方米/天"; // 实际计算费用的时候，也是取这里二程物流方式里面的值
                string minFeePerDay = $"{secondMode.FeeRate?.ExtraStorageDayMinFeePerDay ?? 0m} RMB/天"; // 实际计算费用的时候，也是取这里二程物流方式里面的值

                string orderCode = order.Code;

                string message1 = TransferMessage(setting.WeiXinMessageTemplate1, orderCode, targetWarehouseOrPickupStation, daysRemaining, storageFeeRate, minFeePerDay);
                string message2 = TransferMessage(setting.WeiXinMessageTemplate2, orderCode, targetWarehouseOrPickupStation, daysRemaining, storageFeeRate, minFeePerDay);
                string message3 = TransferMessage(setting.WeiXinMessageTemplate3, orderCode, targetWarehouseOrPickupStation, daysRemaining, storageFeeRate, minFeePerDay);

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
                        string url = customerClientUrlService.GetDeliveredOrderDetailUrl(customer, order.Id);

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
                        customerMessageCommonService.SendSms(customer.Mobile, TransferMessage(setting.SmsMessageTemplate, orderCode, targetWarehouseOrPickupStation, daysRemaining, storageFeeRate, minFeePerDay));
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

        private string TransferMessage(string message, string orderCode, string targetWarehouseOrPickupStation, int daysRemaining, string storageFeeRate, string minFeePerDay)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，澳仓或取货点名称：{target_warehouse_or_pickup_station}，剩余免费仓储天数：{days_remaining}，额外仓储费率（RMB/立方米/天）：{storage_fee_rate}, 最低仓储收费（RMB/天）：{min_fee_per_day}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{target_warehouse_or_pickup_station}", targetWarehouseOrPickupStation);
            message = message.Replace("{days_remaining}", daysRemaining.ToString());
            message = message.Replace("{storage_fee_rate}", storageFeeRate);
            message = message.Replace("{min_fee_per_day}", minFeePerDay);

            return message;
        }
    }
}
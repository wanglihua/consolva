using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain;
using Domain.Customer;
using Domain.CustomerChat;
using Domain.Deliver;
using Domain.LogisticsOrder;
using Domain.Message;
using Domain.Pass;
using NHibernate;
using Service.Customer;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Domain.Pass;


// todo a 这个文件将来会被移除

namespace shunda.Service.Message;

    [Service(ServiceType.Scoped)]
    public class CustomerMessageService
    {
        /// <summary>
        /// 已到港 消息发送延迟
        /// </summary>
        public static readonly TimeSpan DelayYiDaoGangTiXing = new TimeSpan(2, 0, 0, 0); // TimeSpan(int days, int hours, int minutes, int seconds)

        /// <summary>
        /// 已到仓消息发送延迟
        /// </summary>
        public static readonly TimeSpan DelayYiDaoMuDiCangTiXing = new TimeSpan(2, 0, 0, 0); // TimeSpan(int days, int hours, int minutes, int seconds)


        private static readonly string defaultProcessUser = "澳拼小秘3.0";

        private static readonly string photoMessageSender = "澳拼3.0智能拍照机器人";

        private ISession session = null;
        private SmsService smsService = null;
        private WechatMessageService wechatMessageService = null;
        private LoginUserService loginUserService = null;
        private CustomerClientUrlService customerClientUrlService = null;
        private CustomerMessageClientCustomizeService customerMessageClientCustomizeService = null;

        private string userName = string.Empty; // 初始是空的

        public CustomerMessageService(ISession session,
            SmsService smsService,
            WechatMessageService wechatMessageService,
            LoginUserService loginUserService,
            CustomerClientUrlService customerClientUrlService,
            CustomerMessageClientCustomizeService customerMessageClientCustomizeService)
        {
            this.session = session;
            this.smsService = smsService;
            this.wechatMessageService = wechatMessageService;
            this.loginUserService = loginUserService;
            this.customerClientUrlService = customerClientUrlService;
            this.customerMessageClientCustomizeService = customerMessageClientCustomizeService;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        /// <summary>
        /// 发送客户消息
        /// </summary>
        public void SendCustomerMessage(CustomerMessageSendType sendType, CustomerMessageType messageType, CustomerInfo customer, params object[] messageData)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            SendCustomerMessageWithOrder(sendType, messageType, customer, null, messageData);
        }

        /// <summary>
        /// 发送客户消息，带有订单参数
        /// </summary>
        public void SendCustomerMessageWithOrder(CustomerMessageSendType sendType, CustomerMessageType messageType, CustomerInfo customer, LogisticsOrderInfo order, params object[] messageData)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            SendMessageWithOrder(sendType, messageType, customer, null, order, messageData);
        }

        /// <summary>
        /// 发送公众号订阅者消息
        /// </summary>
        public void SendWeiXinFollowerMessage(WeChatOfficialAccount account, CustomerMessageSendType sendType, CustomerMessageType messageType, WeiXinFollowerInfo weiXinFollower, params object[] messageData)
        {
            // todo 这里要修改
            SendMessageWithOrder(sendType, messageType, null, weiXinFollower, null, messageData);
        }

        #region 快递单号入库

        /// <summary>
        /// 
        /// 快递单号入库
        /// 这个消息类型，现在3.0里面实际已经没有用到了
        /// 
        /// <param name="warehouseName">仓库名称</param>
        /// <param name="goodsExpress">快递单</param>
        /// 
        /// </summary>
        public void SendMessageKuaiDiDanHaoYiRuKu(CustomerInfo customer, string warehouseName, LogisticsOrderGoodsExpressInfo goodsExpress)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.KuaiDiDanHaoYiRuKuTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (goodsExpress == null)
                {
                    throw new ValidationException("消息发送指定的快递单不能为空！");
                }

                string orderCode = goodsExpress.Order?.Code;
                string trackingNo = goodsExpress.ExpressTrackingNo;
                string goodsType = goodsExpress.GoodsType?.Name;

                string message1 = TransferMessageKuaiDiDanHaoYiRuKu(setting.WeiXinMessageTemplate1, orderCode, trackingNo, warehouseName, goodsType);
                string message2 = TransferMessageKuaiDiDanHaoYiRuKu(setting.WeiXinMessageTemplate2, orderCode, trackingNo, warehouseName, goodsType);
                string message3 = TransferMessageKuaiDiDanHaoYiRuKu(setting.WeiXinMessageTemplate3, orderCode, trackingNo, warehouseName, goodsType);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.KuaiDiDanHaoYiRuKuTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageKuaiDiDanHaoYiRuKu(setting.SmsMessageTemplate, orderCode, trackingNo, warehouseName, goodsType));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, goodsExpress.Order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, goodsExpress.Order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, goodsExpress.Order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageKuaiDiDanHaoYiRuKu(string message, string orderCode, string trackingNo, string warehouseName, string goodsType)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，快递单号：{tracking_no}，仓库名称：{warehouse_name}，物品类型：{goods_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{warehouse_name}", warehouseName);
            message = message.Replace("{goods_type}", goodsType);

            return message;
        }

        #endregion

        #region 我的申领件提醒

        /// <summary>
        /// 
        /// 我的申领件提醒
        /// 
        /// </summary>
        public void SendMessageWoDeShenLingJian(CustomerInfo customer)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.WoDeShenLingJianTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageWoDeShenLingJian(setting.WeiXinMessageTemplate1);
                string message2 = TransferMessageWoDeShenLingJian(setting.WeiXinMessageTemplate2);
                string message3 = TransferMessageWoDeShenLingJian(setting.WeiXinMessageTemplate3);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.WoDeShenLingJianTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string abnormalClause = "【申领件】通知";

                        string url = customerClientUrlService.GetExpressClaimListUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageWoDeShenLingJian(setting.SmsMessageTemplate));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageWoDeShenLingJian(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "";

            return message;
        }

        #endregion

        #region 预报货值提醒

        /// <summary>
        /// 预报货值提醒
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="trackingNo"></param>
        /// <exception cref="ValidationException"></exception>
        public void SendMessageYuBaoHuoZhi(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YuBaoHuoZhiTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageYuBaoHuoZhi(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageYuBaoHuoZhi(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageYuBaoHuoZhi(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.YuBaoHuoZhiTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string abnormalClause = "【预报货值】通知";
                        string url = customerClientUrlService.GetExpressExpectListUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", abnormalClause, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageYuBaoHuoZhi(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageYuBaoHuoZhi(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}";
            message = message.Replace("{tracking_no}", trackingNo);

            return message;
        }

        #endregion

        #region 我的问题件提醒

        /// <summary>
        /// 
        /// 我的问题件提醒
        /// 
        /// </summary>
        public void SendMessageWoDeWeiTiJian(CustomerInfo customer)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.WoDeWeiTiJianTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageWoDeWeiTiJian(setting.WeiXinMessageTemplate1);
                string message2 = TransferMessageWoDeWeiTiJian(setting.WeiXinMessageTemplate2);
                string message3 = TransferMessageWoDeWeiTiJian(setting.WeiXinMessageTemplate3);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.WoDeWeiTiJianTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string abnormalClause = "【问题件】通知";

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderAbnormalNoticeTemplateMessage(customer, "", abnormalClause, message1, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderAbnormalNoticeTemplateMessage(customer, "", abnormalClause, message2, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderAbnormalNoticeTemplateMessage(customer, "", abnormalClause, message3, processUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageWoDeWeiTiJian(setting.SmsMessageTemplate));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageWoDeWeiTiJian(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "";

            return message;
        }

        #endregion

        #region 全部入库提交打包提醒

        /// <summary>
        /// 
        /// 全部入库提交打包提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageQuanBuRuKuTiJiaDaBo(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.QuanBuRuKuTiJiaDaBoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageQuanBuRuKuTiJiaDaBo(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageQuanBuRuKuTiJiaDaBo(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageQuanBuRuKuTiJiaDaBo(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.QuanBuRuKuTiJiaDaBoTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageQuanBuRuKuTiJiaDaBo(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageQuanBuRuKuTiJiaDaBo(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}";
            message = message.Replace("{order_code}", orderCode);

            return message;
        }

        #endregion

        #region 自动提交打包预告

        /// <summary>
        /// 
        /// 自动提交打包预告
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageZiDongDaBaoYuGao(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.ZiDongDaBaoYuGao;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageZiDongDaBaoYuGao(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageZiDongDaBaoYuGao(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageZiDongDaBaoYuGao(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.ZiDongDaBaoYuGao);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, defaultProcessUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageZiDongDaBaoYuGao(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageZiDongDaBaoYuGao(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}";
            message = message.Replace("{order_code}", orderCode);

            return message;
        }

        #endregion

        #region 已打包提醒

        /// <summary>
        /// 
        /// 已打包提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageYiDaBao(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YiDaBaoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageYiDaBao(setting.WeiXinMessageTemplate1, orderCode, totalFee);
                string message2 = TransferMessageYiDaBao(setting.WeiXinMessageTemplate2, orderCode, totalFee);
                string message3 = TransferMessageYiDaBao(setting.WeiXinMessageTemplate3, orderCode, totalFee);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.YiDaBaoTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetWaitTransportOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageYiDaBao(setting.SmsMessageTemplate, orderCode, totalFee));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageYiDaBao(string message, string orderCode, string totalFee)
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

        #endregion

        #region 已转运提醒

        /// <summary>
        /// 
        /// 已转运提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageYiZhuanYun(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode, string arriveTime)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YiZhuanYunTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageYiZhuanYun(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, arriveTime);
                string message2 = TransferMessageYiZhuanYun(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, arriveTime);
                string message3 = TransferMessageYiZhuanYun(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, arriveTime);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.YiZhuanYunTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageYiZhuanYun(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, arriveTime));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageYiZhuanYun(string message, string orderCode, string customerShipContainerCode, string arriveTime)
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

        #endregion

        #region 支付二程方式变更费用提醒

        /// <summary>
        /// 
        /// 支付二程方式变更费用提醒
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageZhiFuErChengFangShiBianGengFeiYong(CustomerInfo customer, LogisticsOrderInfo order, string shipContainerCustomerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.ZhiFuErChengFangShiBianGengFeiYongTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageZhiFuErChengFangShiBianGengFeiYong(setting.WeiXinMessageTemplate1, orderCode, shipContainerCustomerCode);
                string message2 = TransferMessageZhiFuErChengFangShiBianGengFeiYong(setting.WeiXinMessageTemplate2, orderCode, shipContainerCustomerCode);
                string message3 = TransferMessageZhiFuErChengFangShiBianGengFeiYong(setting.WeiXinMessageTemplate3, orderCode, shipContainerCustomerCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.ZhiFuErChengFangShiBianGengFeiYongTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageZhiFuErChengFangShiBianGengFeiYong(setting.SmsMessageTemplate, orderCode, shipContainerCustomerCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageZhiFuErChengFangShiBianGengFeiYong(string message, string orderCode, string shipContainerCustomerCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", shipContainerCustomerCode);

            return message;
        }

        #endregion

        #region 已到港提醒

        /// <summary>
        /// 
        /// 已到港提醒
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageYiDaoGang(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YiDaoGangTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageYiDaoGang(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode);
                string message2 = TransferMessageYiDaoGang(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode);
                string message3 = TransferMessageYiDaoGang(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.YiDaoGangTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageYiDaoGang(setting.SmsMessageTemplate, orderCode, customerShipContainerCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageYiDaoGang(string message, string orderCode, string customerShipContainerCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);

            return message;
        }

        #endregion

        #region 已到目的仓提醒

        /// <summary>
        /// 
        /// 已到目的仓提醒
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageYiDaoMuDiCang(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.YiDaoMuDiCangTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageYiDaoMuDiCang(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode);
                string message2 = TransferMessageYiDaoMuDiCang(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode);
                string message3 = TransferMessageYiDaoMuDiCang(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.YiDaoMuDiCangTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageYiDaoMuDiCang(setting.SmsMessageTemplate, orderCode, customerShipContainerCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageYiDaoMuDiCang(string message, string orderCode, string customerShipContainerCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);

            return message;
        }

        #endregion

        #region 订单状态回拨提醒

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
        public void SendMessageDingDanZhuangTaiHuiBo(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode, string orderOldStatus, string orderPreStatus, string orderStatusNextTime)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.DingDanZhuangTaiHuiBoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageDingDanZhuangTaiHuiBo(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);
                string message2 = TransferMessageDingDanZhuangTaiHuiBo(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);
                string message3 = TransferMessageDingDanZhuangTaiHuiBo(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.DingDanZhuangTaiHuiBoTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageDingDanZhuangTaiHuiBo(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, orderOldStatus, orderPreStatus, orderStatusNextTime));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageDingDanZhuangTaiHuiBo(string message, string orderCode, string customerShipContainerCode, string orderOldStatus, string orderPreStatus, string orderStatusNextTime)
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

        #endregion

        #region 目的仓批次可自提推送

        /// <summary>
        /// 
        /// 目的仓批次可自提推送
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageMuDiCangPiCiKeZiTi(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.MuDiCangPiCiKeZiTiTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                string warehouseName = order.FirstLogisticsMode?.WarehouseTo?.Name;
                string warehouseAddress = order.FirstLogisticsMode?.WarehouseTo?.Address;

                string message1 = TransferMessageMuDiCangPiCiKeZiTi(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);
                string message2 = TransferMessageMuDiCangPiCiKeZiTi(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);
                string message3 = TransferMessageMuDiCangPiCiKeZiTi(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.MuDiCangPiCiKeZiTiTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageMuDiCangPiCiKeZiTi(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, warehouseName, warehouseAddress));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageMuDiCangPiCiKeZiTi(string message, string orderCode, string customerShipContainerCode, string warehouseName, string warehouseAddress)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，仓库名称：{warehouse_name}，仓库地址：{warehouse_address}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{warehouse_name}", warehouseName);
            message = message.Replace("{warehouse_address}", warehouseAddress);

            return message;
        }

        #endregion

        #region 目的仓批次扫码异常推送

        /// <summary>
        /// 
        /// 目的仓批次扫码异常推送
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageMuDiCangPiCiSaoMaYiChang(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.MuDiCangPiCiSaoMaYiChangTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                string warehouseName = order.FirstLogisticsMode?.WarehouseTo?.Name;
                string warehouseAddress = order.FirstLogisticsMode?.WarehouseTo?.Address;

                string message1 = TransferMessageMuDiCangPiCiSaoMaYiChang(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);
                string message2 = TransferMessageMuDiCangPiCiSaoMaYiChang(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);
                string message3 = TransferMessageMuDiCangPiCiSaoMaYiChang(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, warehouseName, warehouseAddress);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.MuDiCangPiCiSaoMaYiChangTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageMuDiCangPiCiSaoMaYiChang(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, warehouseName, warehouseAddress));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageMuDiCangPiCiSaoMaYiChang(string message, string orderCode, string customerShipContainerCode, string warehouseName, string warehouseAddress)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，仓库名称：{warehouse_name}，仓库地址：{warehouse_address}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{warehouse_name}", warehouseName);
            message = message.Replace("{warehouse_address}", warehouseAddress);

            return message;
        }

        #endregion

        #region 即将送货上门推送

        /// <summary>
        /// 
        /// 即将送货上门推送
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageJiJiangSongHuoShangMen(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.JiJiangSongHuoShangMenTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageJiJiangSongHuoShangMen(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode);
                string message2 = TransferMessageJiJiangSongHuoShangMen(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode);
                string message3 = TransferMessageJiJiangSongHuoShangMen(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.JiJiangSongHuoShangMenTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageJiJiangSongHuoShangMen(setting.SmsMessageTemplate, orderCode, customerShipContainerCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageJiJiangSongHuoShangMen(string message, string orderCode, string customerShipContainerCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);

            return message;
        }

        #endregion

        #region 取货点可取货推送

        /// <summary>
        /// 
        /// 取货点可取货推送
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageQuHuoDianKeQuHuo(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.QuHuoDianKeQuHuoTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                string pickupStationPhone = order.SecondLogisticsMode?.PickupStation?.Phone;
                string pickupStationAddress = order.SecondLogisticsMode?.PickupStation?.Address;

                string message1 = TransferMessageQuHuoDianKeQuHuo(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);
                string message2 = TransferMessageQuHuoDianKeQuHuo(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);
                string message3 = TransferMessageQuHuoDianKeQuHuo(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.QuHuoDianKeQuHuoTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageQuHuoDianKeQuHuo(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageQuHuoDianKeQuHuo(string message, string orderCode, string customerShipContainerCode, string pickupStationName, string pickupStationPhone, string pickupStationAddress)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，取货点名称：{pickup_station_name}，取货点电话：{pickup_station_phone}，取货点地址：{pickup_station_address}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{pickup_station_name}", pickupStationName);
            message = message.Replace("{pickup_station_phone}", pickupStationPhone);
            message = message.Replace("{pickup_station_address}", pickupStationAddress);

            return message;
        }

        #endregion

        #region 取货点扫码异常推送

        /// <summary>
        /// 
        /// 取货点扫码异常推送
        /// 
        /// <param name="order">物流订单</param>
        /// <param name="customerShipContainerCode">客户端船航批次</param>
        /// 
        /// </summary>
        public void SendMessageQuHuoDianSaoMaYiChang(CustomerInfo customer, LogisticsOrderInfo order, string customerShipContainerCode, string pickupStationName, string pickupStationPhone, string pickupStationAddress)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.QuHuoDianSaoMaYiChangTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageQuHuoDianSaoMaYiChang(setting.WeiXinMessageTemplate1, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);
                string message2 = TransferMessageQuHuoDianSaoMaYiChang(setting.WeiXinMessageTemplate2, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);
                string message3 = TransferMessageQuHuoDianSaoMaYiChang(setting.WeiXinMessageTemplate3, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.QuHuoDianSaoMaYiChangTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageQuHuoDianSaoMaYiChang(setting.SmsMessageTemplate, orderCode, customerShipContainerCode, pickupStationName, pickupStationPhone, pickupStationAddress));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageQuHuoDianSaoMaYiChang(string message, string orderCode, string customerShipContainerCode, string pickupStationName, string pickupStationPhone, string pickupStationAddress)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，取货点名称：{pickup_station_name}，取货点电话：{pickup_station_phone}，取货点地址：{pickup_station_address}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerShipContainerCode);
            message = message.Replace("{pickup_station_name}", pickupStationName);
            message = message.Replace("{pickup_station_phone}", pickupStationPhone);
            message = message.Replace("{pickup_station_address}", pickupStationAddress);

            return message;
        }

        #endregion

        #region 取货盘点库存再次推送

        /// <summary>
        /// 
        /// 取货盘点库存再次推送
        /// 
        /// </summary>
        public void SendMessageQuHuoDianPanDianKuCun(CustomerInfo customer, LogisticsOrderInfo order, int stayDays)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.QuHuoDianPanDianKuCunZaiCiTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code ?? "";

                string message1 = TransferMessageQuHuoDianPanDianKuCun(setting.WeiXinMessageTemplate1, orderCode, stayDays);
                string message2 = TransferMessageQuHuoDianPanDianKuCun(setting.WeiXinMessageTemplate2, orderCode, stayDays);
                string message3 = TransferMessageQuHuoDianPanDianKuCun(setting.WeiXinMessageTemplate3, orderCode, stayDays);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.QuHuoDianPanDianKuCunZaiCiTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", orderCode, customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageQuHuoDianPanDianKuCun(setting.SmsMessageTemplate, orderCode, stayDays));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageQuHuoDianPanDianKuCun(string message, string orderCode, int stayDays)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编号：{order_code}，滞留目的仓天数：{stay_days}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{stay_days}", stayDays.ToString());

            return message;
        }

        #endregion

        #region 收取GST推送

        /// <summary>
        /// 
        /// 收取GST推送
        /// 这个消息 2.0 里面使用较多，3.0 里面基本上没用起来
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageGst(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.GstTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageGst(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageGst(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageGst(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.GstTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "收取GST推送", customer.CustomerCode, message1, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "收取GST推送", customer.CustomerCode, message2, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "收取GST推送", customer.CustomerCode, message3, defaultProcessUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageGst(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageGst(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}";
            message = message.Replace("{order_code}", orderCode);

            return message;
        }

        #endregion

        #region 快递单号签收图片推送

        public void SendMessageOrderPhotoSignIn(CustomerInfo customer, string trackingNo, string senderName)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoSignInTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo))
                {
                    throw new ValidationException("消息发送指定的快递单号不能为空！");
                }

                string message1 = TransferMessageOrderPhotoSignIn(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageOrderPhotoSignIn(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageOrderPhotoSignIn(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoSignInTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string sender = photoMessageSender;
                        if (string.IsNullOrWhiteSpace(senderName) == false)
                        {
                            sender = senderName;
                        }

                        string url = null;
                        var claim = session.Query<LogisticsClaimGoodsInfo>()
                            .Where(e => e.TrackingNo == trackingNo)
                            .Take(1)
                            .SingleOrDefault();
                        if (claim != null)
                        {
                            url = customerClientUrlService.GetExpressCheckinDetailUrl(customer, claim.Id);
                        }

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单签收图片推送", customer.CustomerCode, message1, sender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单签收图片推送", customer.CustomerCode, message2, sender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单签收图片推送", customer.CustomerCode, message3, sender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoSignIn(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        /// <summary>
        /// 
        /// 快递单签收图片推送
        /// 
        /// <param name="trackingNo">快递单号</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoSignIn(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            SendMessageOrderPhotoSignIn(customer, trackingNo, string.Empty);
        }

        private string TransferMessageOrderPhotoSignIn(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}，图片类型：{photo_type}";
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.SignIn.Name);

            return message;
        }

        #endregion

        #region 订单入库图片推送

        /// <summary>
        /// 
        /// 快递单入库图片推送
        /// 
        /// <param name="trackingNo">快递单号</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoCheckIn(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoCheckInTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo))
                {
                    throw new ValidationException("消息发送指定的快递单号不能为空！");
                }

                string message1 = TransferMessageOrderPhotoCheckIn(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageOrderPhotoCheckIn(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageOrderPhotoCheckIn(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoCheckInTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = null;
                        var claim = session.Query<LogisticsClaimGoodsInfo>()
                            .Where(e => e.TrackingNo == trackingNo)
                            .Take(1)
                            .SingleOrDefault();
                        if (claim != null)
                        {
                            url = customerClientUrlService.GetExpressCheckinDetailUrl(customer, claim.Id);
                        }

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoCheckIn(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoCheckIn(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}，图片类型：{photo_type}";
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.CheckIn.Name);

            return message;
        }

        #endregion

        #region 订单入库图片推送

        /// <summary>
        /// 
        /// 快递单入库图片推送
        /// 
        /// <param name="trackingNo">快递单号</param>
        /// 
        /// </summary>
        public void SendMessageOrderVideoCheckIn(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderVideoCheckInTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo))
                {
                    throw new ValidationException("消息发送指定的快递单号不能为空！");
                }

                string message1 = TransferMessageOrderVideoCheckIn(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageOrderVideoCheckIn(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageOrderVideoCheckIn(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderVideoCheckInTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string url = null;
                        var claim = session.Query<LogisticsClaimGoodsInfo>()
                            .Where(e => e.TrackingNo == trackingNo)
                            .Take(1)
                            .SingleOrDefault();
                        if (claim != null)
                        {
                            url = customerClientUrlService.GetExpressCheckinDetailUrl(customer, claim.Id);
                        }

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库视频推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库视频推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单入库视频推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderVideoCheckIn(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderVideoCheckIn(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}，视频类型：{video_type}";
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{video_type}", LogisticsOrderVideoType.CheckIn.Name);

            return message;
        }

        #endregion

        #region 订单问题图片推送

        /// <summary>
        /// 
        /// 订单问题图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoIssue(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoIssueTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoIssue(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoIssue(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoIssue(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoIssueTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单问题图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单问题图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单问题图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoIssue(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoIssue(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.Issue.Name);

            return message;
        }

        #endregion

        #region 订单中国损毁图片推送

        /// <summary>
        /// 
        /// 订单中国损毁图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoCnDamage(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoCnDamageTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoCnDamage(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoCnDamage(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoCnDamage(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoCnDamageTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string url = customerClientUrlService.GetWaitTransportOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单中国损毁图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单中国损毁图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单中国损毁图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoCnDamage(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoCnDamage(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.CnDamage.Name);

            return message;
        }

        #endregion

        #region 订单退运图片推送

        /// <summary>
        /// 
        /// 订单退运图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoReturn(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoReturnTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoReturn(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoReturn(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoReturn(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoReturnTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string url = customerClientUrlService.GetWaitTransportOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单退运图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单退运图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单退运图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoReturn(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoReturn(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.Return.Name);

            return message;
        }

        #endregion

        #region 订单发货图片推送

        /// <summary>
        /// 
        /// 订单发货图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoShip(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoShipTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoShip(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoShip(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoShip(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoShipTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string url = customerClientUrlService.GetWaitTransportOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单发货图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoShip(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoShip(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.Ship.Name);

            return message;
        }

        #endregion


        #region 订单澳洲损毁图片推送

        /// <summary>
        /// 
        /// 订单澳洲损毁图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoAuDamage(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoAuDamageTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoAuDamage(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoAuDamage(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoAuDamage(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoAuDamageTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单澳洲损毁图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单澳洲损毁图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单澳洲损毁图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoAuDamage(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoAuDamage(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.AuDamage.Name);

            return message;
        }

        #endregion

        #region 订单交付图片推送

        /// <summary>
        /// 
        /// 订单交付图片推送
        /// 
        /// <param name="order">物流订单</param>
        /// 
        /// </summary>
        public void SendMessageOrderPhotoDeliver(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderPhotoDeliverTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string message1 = TransferMessageOrderPhotoDeliver(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderPhotoDeliver(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderPhotoDeliver(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderPhotoDeliverTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetDeliveredOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单交付图片推送", customer.CustomerCode, message1, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单交付图片推送", customer.CustomerCode, message2, photoMessageSender, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单交付图片推送", customer.CustomerCode, message3, photoMessageSender, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderPhotoDeliver(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderPhotoDeliver(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{photo_type}", LogisticsOrderPhotoType.Deliver.Name);

            return message;
        }

        #endregion

        #region VIP升级推送

        /// <summary>
        /// 
        /// VIP升级推送
        /// 
        /// </summary>
        public void SendMessageVipShengJi(CustomerInfo customer, string newVipLevel, string logisticsFirstDiscount, string logisticsSecondDiscount, string mallDiscount, string nextVipLevel)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.VipShengJiTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageVipShengJi(setting.WeiXinMessageTemplate1, newVipLevel, logisticsFirstDiscount, logisticsSecondDiscount, mallDiscount, nextVipLevel);
                string message2 = TransferMessageVipShengJi(setting.WeiXinMessageTemplate2, newVipLevel, logisticsFirstDiscount, logisticsSecondDiscount, mallDiscount, nextVipLevel);
                string message3 = TransferMessageVipShengJi(setting.WeiXinMessageTemplate3, newVipLevel, logisticsFirstDiscount, logisticsSecondDiscount, mallDiscount, nextVipLevel);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.VipShengJiTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetCustomerScoreUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "VIP升级", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "VIP升级", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "VIP升级", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageVipShengJi(setting.SmsMessageTemplate, newVipLevel, logisticsFirstDiscount, logisticsSecondDiscount, mallDiscount, nextVipLevel));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageVipShengJi(string message, string newVipLevel, string logisticsFirstDiscount, string logisticsSecondDiscount, string mallDiscount, string nextVipLevel)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "最新VIP等级：{new_vip_level}，集运头程折扣：{logistics_first_discount}，集运二程折扣：{logistics_second_discount}，澳拼商城折扣：{mall_discount}，下一级VIP：{next_vip_level}";
            message = message.Replace("{new_vip_level}", newVipLevel);
            message = message.Replace("{logistics_first_discount}", logisticsFirstDiscount);
            message = message.Replace("{logistics_second_discount}", logisticsSecondDiscount);
            message = message.Replace("{mall_discount}", mallDiscount);
            message = message.Replace("{next_vip_level}", nextVipLevel);

            return message;
        }

        #endregion

        #region 在线客服留言提醒

        /// <summary>
        /// 
        /// 在线客服留言提醒
        /// 
        /// </summary>
        public void SendMessageZaiXianKeFu(CustomerInfo customer)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.ZaiXianKeFuTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageZaiXianKeFu(setting.WeiXinMessageTemplate1);
                string message2 = TransferMessageZaiXianKeFu(setting.WeiXinMessageTemplate2);
                string message3 = TransferMessageZaiXianKeFu(setting.WeiXinMessageTemplate3);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.ZaiXianKeFuTuiSong);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string processUser = GetProcessUser();
                        string url = customerClientUrlService.GetChatUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "在线客服留言提醒", customer.CustomerCode, message1, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "在线客服留言提醒", customer.CustomerCode, message2, processUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "在线客服留言提醒", customer.CustomerCode, message3, processUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageZaiXianKeFu(setting.SmsMessageTemplate));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageZaiXianKeFu(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "";

            return message;
        }

        #endregion

        #region 账户余额变动提醒

        /// <summary>
        /// 
        /// 账户余额变动提醒
        /// 
        /// </summary>
        public void SendMessageAccountChange(CustomerInfo customer, LogisticsOrderCate orderCate, string changeReason, decimal accountChangeAmount, decimal accountBeforeAmount, decimal accountAfterAmount)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.AccountChangeTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageAccountChange(setting.WeiXinMessageTemplate1, orderCate, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);
                string message2 = TransferMessageAccountChange(setting.WeiXinMessageTemplate2, orderCate, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);
                string message3 = TransferMessageAccountChange(setting.WeiXinMessageTemplate3, orderCate, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.AccountChangeTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetCustomerAccountUrl(customer);

                        string messageTitle = "账户余额变动提醒";

                        if (orderCate == LogisticsOrderCate.Sea)
                        {
                            messageTitle = "海运余额变动提醒";
                        }

                        if (orderCate == LogisticsOrderCate.Air)
                        {
                            messageTitle = "空运余额变动提醒";
                        }

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageTitle, customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageAccountChange(setting.SmsMessageTemplate, orderCate, changeReason, accountChangeAmount, accountBeforeAmount, accountAfterAmount));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageAccountChange(string message, LogisticsOrderCate orderCate, string changeReason, decimal accountChangeAmount, decimal accountBeforeAmount, decimal accountAfterAmount)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单类别(海运或空运)：{order_cate}，变动原因：{change_reason}，账户余额变动金额：{account_change_amount}，账户变动前余额：{account_before_amount}，账户变动后余额：{account_after_amount}";

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
            message = message.Replace("{change_reason}", changeReason);
            message = message.Replace("{account_change_amount}", accountChangeAmount.ToString("#0.00"));
            message = message.Replace("{account_before_amount}", accountBeforeAmount.ToString("#0.00"));
            message = message.Replace("{account_after_amount}", accountAfterAmount.ToString("#0.00"));

            return message;
        }

        #endregion

        #region 船航时间更改提醒

        /// <summary>
        /// 
        /// 船航时间更改提醒
        ///
        /// 
        /// </summary>
        public void SendMessageShipContainerTimeChange(CustomerInfo customer, LogisticsOrderInfo order, string customerContainerCode, string changeTimeType, string timeAfterChange, string changeReason)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.ShipContainerTimeChangeTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code;

                string message1 = TransferMessageShipContainerTimeChange(setting.WeiXinMessageTemplate1, orderCode, customerContainerCode, changeTimeType, timeAfterChange, changeReason);
                string message2 = TransferMessageShipContainerTimeChange(setting.WeiXinMessageTemplate2, orderCode, customerContainerCode, changeTimeType, timeAfterChange, changeReason);
                string message3 = TransferMessageShipContainerTimeChange(setting.WeiXinMessageTemplate3, orderCode, customerContainerCode, changeTimeType, timeAfterChange, changeReason);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.ShipContainerTimeChangeTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "船航时间更改提醒", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "船航时间更改提醒", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "船航时间更改提醒", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageShipContainerTimeChange(setting.SmsMessageTemplate, orderCode, customerContainerCode, changeTimeType, timeAfterChange, changeReason));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageShipContainerTimeChange(string message, string orderCode, string customerContainerCode, string changeTimeType, string timeAfterChange, string changeReason)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，变动时间类型：{change_time_type}，变动后时间：{time_after_change}，更新原因：{change_reason}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{customer_container_code}", customerContainerCode);
            message = message.Replace("{change_time_type}", changeTimeType);
            message = message.Replace("{time_after_change}", timeAfterChange);
            message = message.Replace("{change_reason}", changeReason);

            return message;
        }

        #endregion

        #region 超期未入库快递单提醒

        /// <summary>
        /// 
        /// 超期未入库快递单提醒
        ///
        /// 
        /// </summary>
        public void SendMessageExpressOverdueCheckin(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.ExpressOverdueCheckinTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo))
                {
                    throw new ValidationException("快递单号不能为空！");
                }

                string message1 = TransferMessageExpressOverdueCheckin(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageExpressOverdueCheckin(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageExpressOverdueCheckin(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.ExpressOverdueCheckinTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetExpressExpectListUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单超期未入库提醒", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单超期未入库提醒", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "快递单超期未入库提醒", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageExpressOverdueCheckin(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageExpressOverdueCheckin(string message, string expressTrackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}";
            message = message.Replace("{tracking_no}", expressTrackingNo);

            return message;
        }

        #endregion

        #region 澳洲快递单号提醒

        /// <summary>
        /// 
        /// 澳洲快递单号提醒
        /// 
        /// </summary>
        public void SendMessageAuExpressTrackingNo(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string expressCompany, string trackingUrl)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.AuExpressTrackingNoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code;

                string message1 = TransferMessageAuExpressTrackingNo(setting.WeiXinMessageTemplate1, orderCode, trackingNo, expressCompany, trackingUrl);
                string message2 = TransferMessageAuExpressTrackingNo(setting.WeiXinMessageTemplate2, orderCode, trackingNo, expressCompany, trackingUrl);
                string message3 = TransferMessageAuExpressTrackingNo(setting.WeiXinMessageTemplate3, orderCode, trackingNo, expressCompany, trackingUrl);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.AuExpressTrackingNoTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "澳洲快递单号提醒", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "澳洲快递单号提醒", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "澳洲快递单号提醒", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageAuExpressTrackingNo(setting.SmsMessageTemplate, orderCode, trackingNo, expressCompany, trackingUrl));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageAuExpressTrackingNo(string message, string orderCode, string trackingNo, string expressCompany, string trackingUrl)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，澳洲快递单号：{tracking_no}，澳洲快递公司：{express_company}，澳洲快递跟踪网址：{tracking_url}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{express_company}", expressCompany);
            message = message.Replace("{tracking_url}", trackingUrl);

            return message;
        }

        #endregion

        #region 国际快递单号提醒

        /// <summary>
        /// 
        /// 国际快递单号提醒
        /// 
        /// </summary>
        public void SendMessageInternationalExpressTrackingNo(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string expressCompany, string trackingUrl)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.InternationalExpressTrackingNoTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code;

                string message1 = TransferMessageInternationalExpressTrackingNo(setting.WeiXinMessageTemplate1, orderCode, trackingNo, expressCompany, trackingUrl);
                string message2 = TransferMessageInternationalExpressTrackingNo(setting.WeiXinMessageTemplate2, orderCode, trackingNo, expressCompany, trackingUrl);
                string message3 = TransferMessageInternationalExpressTrackingNo(setting.WeiXinMessageTemplate3, orderCode, trackingNo, expressCompany, trackingUrl);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.InternationalExpressTrackingNoTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string sender = "空运客服";

                        string orderDetailUrl = customerClientUrlService.GetTransportingOrderDetailUrl(customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageType.Name, customer.CustomerCode, message1, sender, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageType.Name, customer.CustomerCode, message2, sender, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageType.Name, customer.CustomerCode, message3, sender, orderDetailUrl);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageInternationalExpressTrackingNo(setting.SmsMessageTemplate, orderCode, trackingNo, expressCompany, trackingUrl));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageInternationalExpressTrackingNo(string message, string orderCode, string trackingNo, string expressCompany, string trackingUrl)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，国际快递公司：{express_company}，国际快递跟踪网址：{tracking_url}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{express_company}", expressCompany);
            message = message.Replace("{tracking_url}", trackingUrl);

            return message;
        }

        #endregion

        #region 国际快递状态更新提醒

        /// <summary>
        /// 
        /// 国际快递状态更新提醒
        /// 
        /// </summary>
        public void SendMessageInternationalExpressStateUpdate(CustomerInfo customer, LogisticsOrderInfo order, string trackingNo, string trackingContent)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.InternationalExpressStateUpdateTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code;

                string message1 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate1, orderCode, trackingNo, trackingContent);
                string message2 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate2, orderCode, trackingNo, trackingContent);
                string message3 = TransferMessageInternationalExpressStateUpdate(setting.WeiXinMessageTemplate3, orderCode, trackingNo, trackingContent);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.InternationalExpressStateUpdateTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        string orderDetailUrl = customerClientUrlService.GetTransportingOrderDetailUrl(order.Customer, order.Id);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message1, defaultProcessUser, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message2, defaultProcessUser, orderDetailUrl);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国际快递状态更新提醒", customer.CustomerCode, message3, defaultProcessUser, orderDetailUrl);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageInternationalExpressStateUpdate(setting.SmsMessageTemplate, orderCode, trackingNo, trackingContent));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageInternationalExpressStateUpdate(string message, string orderCode, string trackingNo, string trackingContent)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，状态更新内容：{tracking_content}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{tracking_no}", trackingNo);
            message = message.Replace("{tracking_content}", trackingContent);

            return message;
        }

        #endregion

        #region 订单未支付提醒

        /// <summary>
        /// 
        /// 订单未支付提醒
        /// 
        /// </summary>
        public void SendMessageOrderNotPay(CustomerInfo customer, LogisticsOrderInfo order)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderNotPayTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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

                string orderCode = order?.Code;

                string message1 = TransferMessageOrderNotPay(setting.WeiXinMessageTemplate1, orderCode);
                string message2 = TransferMessageOrderNotPay(setting.WeiXinMessageTemplate2, orderCode);
                string message3 = TransferMessageOrderNotPay(setting.WeiXinMessageTemplate3, orderCode);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderNotPayTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单未支付提醒", customer.CustomerCode, message1, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单未支付提醒", customer.CustomerCode, message2, defaultProcessUser);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "订单未支付提醒", customer.CustomerCode, message3, defaultProcessUser);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderNotPay(setting.SmsMessageTemplate, orderCode));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, order, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderNotPay(string message, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单编码：{order_code}";
            message = message.Replace("{order_code}", orderCode);

            return message;
        }

        #endregion

        #region 未创建订单提醒

        /// <summary>
        /// 
        /// 未创建订单提醒
        /// 
        /// </summary>
        public void SendMessageOrderNotCreate(CustomerInfo customer)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.OrderNotCreateTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string message1 = TransferMessageOrderNotCreate(setting.WeiXinMessageTemplate1);
                string message2 = TransferMessageOrderNotCreate(setting.WeiXinMessageTemplate2);
                string message3 = TransferMessageOrderNotCreate(setting.WeiXinMessageTemplate3);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.OrderNotCreateTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetExpressCheckinListUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "未创建订单提醒", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "未创建订单提醒", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "未创建订单提醒", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageOrderNotCreate(setting.SmsMessageTemplate));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageOrderNotCreate(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            return message;
        }

        #endregion

        #region 客服评价消息

        /// <summary>
        /// 
        /// 客服评价消息
        /// 
        /// </summary>
        public void SendMessageCustomerChatRate(CustomerInfo customer, UserInfo user, CustomerChatRateMessageInfo chatRateMessage)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.CustomerChatRateMessage;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                string customerCode = customer.CustomerCode;
                string serviceUserName = user?.UserName;

                string message1 = TransferMessageCustomerChatRate(setting.WeiXinMessageTemplate1, customerCode, serviceUserName);
                string message2 = TransferMessageCustomerChatRate(setting.WeiXinMessageTemplate2, customerCode, serviceUserName);
                string message3 = TransferMessageCustomerChatRate(setting.WeiXinMessageTemplate3, customerCode, serviceUserName);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.CustomerChatRateMessage);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetChatRateUrl(customer, chatRateMessage);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "客服评价消息", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "客服评价消息", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "客服评价消息", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageCustomerChatRate(setting.SmsMessageTemplate, customerCode, serviceUserName));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageCustomerChatRate(string message, string customerCode, string serviceUserName)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "会员ID：{customer_code}，客服：{user_name}";
            message = message.Replace("{customer_code}", customerCode);
            message = message.Replace("{user_name}", serviceUserName);

            return message;
        }

        #endregion

        #region 国内仓储即将超期提醒

        /// <summary>
        /// 
        /// 国内仓储即将超期提醒
        /// 
        /// </summary>
        public void SendMessageCnStorageOverdue(CustomerInfo customer, string trackingNo)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.CnStorageOverdueTiXing;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
            if (setting.Status == CustomerMessageSettingStatus.Enabled)
            {
                if (customer == null)
                {
                    throw new ValidationException("消息发送指定的会员不能为空！");
                }

                if (string.IsNullOrWhiteSpace(trackingNo))
                {
                    throw new ValidationException("快递单号不能为空！");
                }

                string message1 = TransferMessageCnStorageOverdue(setting.WeiXinMessageTemplate1, trackingNo);
                string message2 = TransferMessageCnStorageOverdue(setting.WeiXinMessageTemplate2, trackingNo);
                string message3 = TransferMessageCnStorageOverdue(setting.WeiXinMessageTemplate3, trackingNo);

                bool isSent = false;

                bool clientShouldSend = true;
                if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
                {
                    clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.CnStorageOverdueTiXing);
                }

                if (setting.WeiXinSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        string url = customerClientUrlService.GetExpressCheckinListUrl(customer);

                        if (string.IsNullOrWhiteSpace(message1) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国内仓储即将超期提醒", customer.CustomerCode, message1, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message2) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国内仓储即将超期提醒", customer.CustomerCode, message2, defaultProcessUser, url);
                        }

                        if (string.IsNullOrWhiteSpace(message3) == false)
                        {
                            wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "国内仓储即将超期提醒", customer.CustomerCode, message3, defaultProcessUser, url);
                        }
                    }
                }

                if (setting.SmsSend)
                {
                    if (clientShouldSend)
                    {
                        isSent = true;
                        smsService.SendSms(customer.Mobile, TransferMessageCnStorageOverdue(setting.SmsMessageTemplate, trackingNo));
                    }
                }

                if (string.IsNullOrWhiteSpace(message1) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message2) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, isSent, setting);
                }

                if (string.IsNullOrWhiteSpace(message3) == false)
                {
                    SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, isSent, setting);
                }

                session.Flush();
            }
        }

        private string TransferMessageCnStorageOverdue(string message, string trackingNo)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "快递单号：{tracking_no}";
            message = message.Replace("{tracking_no}", trackingNo);

            return message;
        }

        #endregion

        #region 送货上门第一次推送

        /// <summary>
        /// 送货上门第一次推送
        /// </summary>
        /// <param name="firstEntry">送货上门明细, 同一个送货批次中的同一个客户相同地址的第一个送货上门明细</param>
        /// <param name="orderCodeList">同一个送货批次中的同一个客户相同地址的的所有订单</param>
        /// <param name="planDateBegin">1月1日</param>
        /// <param name="planDateEnd">1月2日</param>
        /// <param name="phone"></param>
        /// <exception cref="ValidationException"></exception>
        public void SendMessageHomeDeliverFirstTuiSong(HomeDeliverBatchEntryInfo firstEntry, IList<string> orderCodeList, string planDateBegin, string planDateEnd, string phone)
        {
            CustomerInfo customer = firstEntry.Customer;
            LogisticsOrderInfo firstOrder = firstEntry.Order;

            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverFirstTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                throw new ValidationException("送货上门第一次推送 订单不能为空！");
            }

            if (orderCodeList == null || orderCodeList.Count == 0)
            {
                throw new ValidationException("送货上门第一次推送 订单编号列表不能为空！");
            }

            string orderCode = firstOrder.Code; // string.Join(", ", orderCodeList);

            if (string.IsNullOrWhiteSpace(planDateBegin))
            {
                throw new ValidationException("送货上门第一次推送 计划送货日期开始不能为空！");
            }

            if (string.IsNullOrWhiteSpace(planDateEnd))
            {
                throw new ValidationException("送货上门第一次推送 计划送货日期结束不能为空！");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ValidationException("送货上门第一次推送 联系电话不能为空！");
            }

            string message1 = TransferMessageHomeDeliverFirstTuiSong(setting.WeiXinMessageTemplate1, orderCode, orderCodeList.Count, planDateBegin, planDateEnd, phone);
            string message2 = TransferMessageHomeDeliverFirstTuiSong(setting.WeiXinMessageTemplate2, orderCode, orderCodeList.Count, planDateBegin, planDateEnd, phone);
            string message3 = TransferMessageHomeDeliverFirstTuiSong(setting.WeiXinMessageTemplate3, orderCode, orderCodeList.Count, planDateBegin, planDateEnd, phone);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.HomeDeliverFirstTuiSong);
            }

            if (setting.WeiXinSend)
            {
                if (clientShouldSend)
                {
                    string processUser = defaultProcessUser; // GetProcessUser();
                    var url = customerClientUrlService.GetHomeDeliverBatchConfirmUrl(firstEntry.Customer, firstEntry.Id); // 客户端Url

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第一次推送", orderCode, customer.CustomerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第一次推送", orderCode, customer.CustomerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第一次推送", orderCode, customer.CustomerCode, message3, processUser, url);
                    }
                }
            }

            if (setting.SmsSend)
            {
                if (clientShouldSend)
                {
                    messageStatus = smsService.SendSmsMessage(customer.Mobile, TransferMessageHomeDeliverFirstTuiSong(setting.SmsMessageTemplate, orderCode, orderCodeList.Count, planDateBegin, planDateEnd, phone));
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message1, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message2, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message3, messageStatus, setting);
            }

            session.Flush();
        }

        private string TransferMessageHomeDeliverFirstTuiSong(string message, string orderCode, int orderCount, string planDateBegin, string planDateEnd, string phone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，订单数量：{order_count}，计划送货日期开始：{plan_date_begin}，计划送货日期结束：{plan_date_end}，联系电话：{phone}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{order_count}", orderCount.ToString());
            message = message.Replace("{plan_date_begin}", planDateBegin);
            message = message.Replace("{plan_date_end}", planDateEnd);
            message = message.Replace("{phone}", phone);

            return message;
        }

        #endregion

        #region 送货上门第二次推送

        /// <summary>
        /// 送货上门第二次推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="firstOrder">同一个送货批次中的其中第一个订单</param>
        /// <param name="orderCodeList">同一个送货批次中的同一个客户同一个地址的所有订单</param>
        /// <param name="deliverDate">1月1日</param>
        /// <param name="deliverTimeBegin">2时</param>
        /// <param name="deliverTimeEnd">3时</param>
        /// <param name="phone"></param>
        /// <exception cref="ValidationException"></exception>
        public void SendMessageHomeDeliverSecondTuiSong(CustomerInfo customer, LogisticsOrderInfo firstOrder, IList<string> orderCodeList, string deliverDate, string deliverTimeBegin, string deliverTimeEnd, string phone)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverSecondTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                throw new ValidationException("送货上门第二次推送 订单不能为空！");
            }

            if (orderCodeList == null || orderCodeList.Count == 0)
            {
                throw new ValidationException("送货上门第四次推送 订单编号列表不能为空！");
            }

            string orderCode = firstOrder.Code; // string.Join(", ", orderCodeList);

            if (string.IsNullOrWhiteSpace(deliverDate))
            {
                throw new ValidationException("送货上门第二次推送 送货日期不能为空！");
            }

            if (string.IsNullOrWhiteSpace(deliverTimeBegin))
            {
                throw new ValidationException("送货上门第二次推送 送货时间开始不能为空！");
            }

            if (string.IsNullOrWhiteSpace(deliverTimeEnd))
            {
                throw new ValidationException("送货上门第二次推送 送货时间结束不能为空！");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ValidationException("送货上门第二次推送 联系电话不能为空！");
            }

            string message1 = TransferMessageHomeDeliverSecondTuiSong(setting.WeiXinMessageTemplate1, orderCode, orderCodeList.Count, deliverDate, deliverTimeBegin, deliverTimeEnd, phone);
            string message2 = TransferMessageHomeDeliverSecondTuiSong(setting.WeiXinMessageTemplate2, orderCode, orderCodeList.Count, deliverDate, deliverTimeBegin, deliverTimeEnd, phone);
            string message3 = TransferMessageHomeDeliverSecondTuiSong(setting.WeiXinMessageTemplate3, orderCode, orderCodeList.Count, deliverDate, deliverTimeBegin, deliverTimeEnd, phone);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.HomeDeliverSecondTuiSong);
            }

            if (setting.WeiXinSend)
            {
                if (clientShouldSend)
                {
                    string processUser = defaultProcessUser; // GetProcessUser();
                    string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, firstOrder.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第二次推送", orderCode, customer.CustomerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第二次推送", orderCode, customer.CustomerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第二次推送", orderCode, customer.CustomerCode, message3, processUser, url);
                    }
                }
            }

            if (setting.SmsSend)
            {
                if (clientShouldSend)
                {
                    messageStatus = smsService.SendSmsMessage(customer.Mobile, TransferMessageHomeDeliverSecondTuiSong(setting.SmsMessageTemplate, orderCode, orderCodeList.Count, deliverDate, deliverTimeBegin, deliverTimeEnd, phone));
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message1, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message2, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message3, messageStatus, setting);
            }

            session.Flush();
        }

        private string TransferMessageHomeDeliverSecondTuiSong(string message, string orderCode, int orderCount, string deliverDate, string deliverTimeBegin, string deliverTimeEnd, string phone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，订单数量：{order_count}，送货日期：{deliver_date}，送货时间开始：{deliver_time_begin}，送货时间结束：{deliver_time_end}，联系电话：{phone}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{order_count}", orderCount.ToString());
            message = message.Replace("{deliver_date}", deliverDate);
            message = message.Replace("{deliver_time_begin}", deliverTimeBegin);
            message = message.Replace("{deliver_time_end}", deliverTimeEnd);
            message = message.Replace("{phone}", phone);

            return message;
        }

        #endregion

        #region 送货上门第三次推送

        /// <summary>
        /// 送货上门第三次推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="firstOrder">同一个送货批次中的其中第一个订单</param>
        /// <param name="orderCodeList">同一个送货批次中的同一个客户同一个地址的所有订单</param>
        /// <param name="deliverTimeBegin">1时</param>
        /// <param name="deliverTimeEnd">2时</param>
        /// <param name="driverName"></param>
        /// <param name="driverPhone"></param>
        /// <param name="url"></param>
        /// <exception cref="ValidationException"></exception>
        public void SendMessageHomeDeliverThirdTuiSong(CustomerInfo customer, LogisticsOrderInfo firstOrder, IList<string> orderCodeList, string deliverTimeBegin, string deliverTimeEnd, string driverName, string driverPhone)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverThirdTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                throw new ValidationException("送货上门第三次推送 订单不能为空！");
            }

            if (orderCodeList == null || orderCodeList.Count == 0)
            {
                throw new ValidationException("送货上门第四次推送 订单编号列表不能为空！");
            }

            string orderCode = firstOrder.Code; // string.Join(", ", orderCodeList);

            if (string.IsNullOrWhiteSpace(deliverTimeBegin))
            {
                throw new ValidationException("送货上门第三次推送 送货时间开始不能为空！");
            }

            if (string.IsNullOrWhiteSpace(deliverTimeEnd))
            {
                throw new ValidationException("送货上门第三次推送 送货时间结束不能为空！");
            }

            if (string.IsNullOrWhiteSpace(driverName))
            {
                throw new ValidationException("送货上门第三次推送 司机姓名不能为空！");
            }

            if (string.IsNullOrWhiteSpace(driverPhone))
            {
                throw new ValidationException("送货上门第三次推送 司机电话不能为空！");
            }

            string message1 = TransferMessageHomeDeliverThirdTuiSong(setting.WeiXinMessageTemplate1, orderCode, orderCodeList.Count, deliverTimeBegin, deliverTimeEnd, driverName, driverPhone);
            string message2 = TransferMessageHomeDeliverThirdTuiSong(setting.WeiXinMessageTemplate2, orderCode, orderCodeList.Count, deliverTimeBegin, deliverTimeEnd, driverName, driverPhone);
            string message3 = TransferMessageHomeDeliverThirdTuiSong(setting.WeiXinMessageTemplate3, orderCode, orderCodeList.Count, deliverTimeBegin, deliverTimeEnd, driverName, driverPhone);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.HomeDeliverThirdTuiSong);
            }

            if (setting.WeiXinSend)
            {
                if (clientShouldSend)
                {
                    string processUser = defaultProcessUser; // GetProcessUser();
                    string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, firstOrder.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第三次推送", orderCode, customer.CustomerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第三次推送", orderCode, customer.CustomerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第三次推送", orderCode, customer.CustomerCode, message3, processUser, url);
                    }
                }
            }

            if (setting.SmsSend)
            {
                if (clientShouldSend)
                {
                    messageStatus = smsService.SendSmsMessage(customer.Mobile, TransferMessageHomeDeliverThirdTuiSong(setting.SmsMessageTemplate, orderCode, orderCodeList.Count, deliverTimeBegin, deliverTimeEnd, driverName, driverPhone));
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message1, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message2, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message3, messageStatus, setting);
            }

            session.Flush();
        }

        private string TransferMessageHomeDeliverThirdTuiSong(string message, string orderCode, int orderCount, string deliverTimeBegin, string deliverTimeEnd, string driverName, string driverPhone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，订单数量：{order_count}，送货时间开始：{deliver_time_begin}，送货时间结束：{deliver_time_end}，司机姓名：{driver_name}，司机电话：{driver_phone}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{order_count}", orderCount.ToString());
            message = message.Replace("{deliver_time_begin}", deliverTimeBegin);
            message = message.Replace("{deliver_time_end}", deliverTimeEnd);
            message = message.Replace("{driver_name}", driverName);
            message = message.Replace("{driver_phone}", driverPhone);

            return message;
        }

        #endregion

        #region 送货上门第四次推送

        /// <summary>
        /// 送货上门第四次推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="firstOrder">同一个送货批次中的其中第一个订单</param>
        /// <param name="orderCodeList">同一个送货批次中的同一个客户的同一个地址的所有订单</param>
        /// <param name="driverName"></param>
        /// <param name="driverPhone"></param>
        /// <exception cref="ValidationException"></exception>
        public void SendMessageHomeDeliverFourthTuiSong(CustomerInfo customer, LogisticsOrderInfo firstOrder, IList<string> orderCodeList, string driverName, string driverPhone)
        {
            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            CustomerMessageType messageType = CustomerMessageType.HomeDeliverFourthTuiSong;
            CustomerMessageRealTimeSendSettingInfo setting = GetCustomerMessageRealTimeSendSetting(messageType);
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
                throw new ValidationException("送货上门第四次推送 订单不能为空！");
            }

            if (orderCodeList == null || orderCodeList.Count == 0)
            {
                throw new ValidationException("送货上门第四次推送 订单编号列表不能为空！");
            }

            string orderCode = firstOrder.Code; // string.Join(", ", orderCodeList);

            if (string.IsNullOrWhiteSpace(driverName))
            {
                throw new ValidationException("送货上门第四次推送 司机姓名不能为空！");
            }

            if (string.IsNullOrWhiteSpace(driverPhone))
            {
                throw new ValidationException("送货上门第四次推送 司机电话不能为空！");
            }

            string message1 = TransferMessageHomeDeliverFourthTuiSong(setting.WeiXinMessageTemplate1, orderCode, orderCodeList.Count, driverName, driverPhone);
            string message2 = TransferMessageHomeDeliverFourthTuiSong(setting.WeiXinMessageTemplate2, orderCode, orderCodeList.Count, driverName, driverPhone);
            string message3 = TransferMessageHomeDeliverFourthTuiSong(setting.WeiXinMessageTemplate3, orderCode, orderCodeList.Count, driverName, driverPhone);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WeiXinSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customer?.Id ?? 0, CustomerMessageType.HomeDeliverFourthTuiSong);
            }

            if (setting.WeiXinSend)
            {
                if (clientShouldSend)
                {
                    string processUser = defaultProcessUser; // GetProcessUser();
                    string url = customerClientUrlService.GetTransportingOrderDetailUrl(customer, firstOrder.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第四次推送", orderCode, customer.CustomerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第四次推送", orderCode, customer.CustomerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        messageStatus = wechatMessageService.SendOrderNoticeTemplateMessage(customer, "送货上门第四次推送", orderCode, customer.CustomerCode, message3, processUser, url);
                    }
                }
            }

            if (setting.SmsSend)
            {
                if (clientShouldSend)
                {
                    messageStatus = smsService.SendSmsMessage(customer.Mobile, TransferMessageHomeDeliverFourthTuiSong(setting.SmsMessageTemplate, orderCode, orderCodeList.Count, driverName, driverPhone));
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message1, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message2, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, firstOrder, message3, messageStatus, setting);
            }

            session.Flush();
        }

        private string TransferMessageHomeDeliverFourthTuiSong(string message, string orderCode, int orderCount, string driverName, string driverPhone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // setting.Placeholders = "订单：{order_code}，订单数量：{order_count}，司机姓名：{driver_name}，司机电话：{driver_phone}";
            message = message.Replace("{order_code}", orderCode);
            message = message.Replace("{order_count}", orderCount.ToString());
            message = message.Replace("{driver_name}", driverName);
            message = message.Replace("{driver_phone}", driverPhone);
            return message;
        }

        #endregion

        /// <summary>
        /// 发送客户消息，带有订单参数
        /// </summary>
        private void SendMessageWithOrder(CustomerMessageSendType sendType, CustomerMessageType messageType, CustomerInfo customer, WeiXinFollowerInfo weiXinFollower, LogisticsOrderInfo order, params object[] messageData)
        {
            // 这里的 customer 是有可能为 null 的

            if (customer?.Status == CustomerStatus.Frozen)
            {
                return;
            }

            int customerId = customer?.Id ?? 0;

            string processUser = GetProcessUser();

            bool isSent = false;
            bool smsSend = false;
            bool weiXinSend = false;
            string message = String.Empty; // 记录的消息内容，一般取取微信的详细

            if (sendType == CustomerMessageSendType.CycleTiming)
            {
                CustomerMessageCycleTimingSendSettingInfo setting = session
                    .Query<CustomerMessageCycleTimingSendSettingInfo>()
                    .Where(e => e.Type == messageType)
                    .Take(1)
                    .SingleOrDefault();

                if (setting == null)
                {
                    throw new Exception("指定的消息发送配置不存在！");
                }

                smsSend = setting.SmsSend;
                weiXinSend = setting.WeiXinSend;

                #region 帮我赚钱提醒

                if (messageType == CustomerMessageType.HelpMeMakeMoney)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("帮我赚钱提醒 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "嗨，老板，今天澳拼帮您赚钱了吗？动动手指，将【帮我赚钱】里您的专属海报转发微信朋友圈，让澳拼马不停蹄帮您赚钱哦！";
                    setting.SmsMessageTemplate = "嗨，老板，今天澳拼帮您赚钱了吗？将【帮我赚钱】里您的专属海报转发朋友圈就可以赚钱哦！";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.HelpMeMakeMoney); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "帮我赚钱提醒", customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                            }
                        }
                    }
                }

                #endregion

                #region 未注册会员推送

                if (messageType == CustomerMessageType.NotRegister)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("未注册会员推送 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "亲，澳拼集运发红包了。现在注册会员，即送￥100红包哦。该红包即可以在澳拼消费，也可以提现。进入公众号【费率|下单】里面的【在线下单】即可注册会员呢，别错过机会哦！";
                    setting.SmsMessageTemplate = "亲，澳拼集运发红包了。现在注册会员，即送￥100红包哦！";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        /*
                        这个不可能定制，还没会员
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.NotRegister); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                        */
                        if (setting.WeiXinSend)
                        {
                            isSent = true;
                            wechatMessageService.SendOrderNoticeTemplateMessage(weiXinFollower, "", "会员注册提醒", "澳拼客户", message, processUser);
                        }

                        /*
                         这个是不可能发送的，发送的话还会出错，因为会员还没注册，电话都取不到
                        if (setting.SmsSend)
                        {
                            isSent = true;
                            smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                        }
                        */
                    }
                }

                #endregion

                #region 已注册未下单推送

                if (messageType == CustomerMessageType.YiZhuCeXiaDanTuiSong)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("已注册未下单推送 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "嗨，老板，澳拼很久没有见您下单了。下个订单，赏口饭吃呗。现在下单，不仅费用低，还有优惠活动哦！";
                    setting.SmsMessageTemplate = "嗨，老板，澳拼很久没有见您下单了。下个订单，赏口饭吃呗。现在下单，不仅费用低，还有优惠活动哦！";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.YiZhuCeXiaDanTuiSong); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "下单提醒", customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                            }
                        }
                    }
                }

                #endregion

                #region 会员未关注公众号推送

                if (messageType == CustomerMessageType.HuiYuanWeiGuanZhuGongZhongHaoTuiSong)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("会员未关注公众号推送 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "亲，发现您未关注澳拼公众号哦。关注公众号，登录系统免密码，且会收到更丰富的实时通知哦！澳拼公众号：iepoch-au";
                    setting.SmsMessageTemplate = "亲，发现您未关注澳拼公众号哦。关注公众号，登录系统免密码，且会收到更丰富的实时通知哦！澳拼公众号：iepoch-au";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.HuiYuanWeiGuanZhuGongZhongHaoTuiSong); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "公众号关注提醒", customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                            }
                        }
                    }
                }

                #endregion

                #region 取消关注公众号推送

                if (messageType == CustomerMessageType.QuXiaoGuanZhuGongZhongHaoTuiSong)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("取消关注公众号推送 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "亲，您取消关注澳拼公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！澳拼公众号：iepoch-au";
                    setting.SmsMessageTemplate = "亲，您取消关注澳拼公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！澳拼公众号：iepoch-au";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.QuXiaoGuanZhuGongZhongHaoTuiSong); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "已取消关注提醒", customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                            }
                        }
                    }
                }

                #endregion

                #region 定时提交打包提醒

                if (messageType == CustomerMessageType.DingShiTiJiaoDaBaoTiXing)
                {
                    if (messageData == null || messageData.Length != 1)
                    {
                        throw new Exception("定时提交打包提醒 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 0: 订单编号
                     *
                    setting.WeiXinMessageTemplate = "亲，您的{0}订单目前已是全部入库状态，如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。早打包，早收到宝贝呢！";
                    setting.SmsMessageTemplate = "亲，您的{0}订单目前已是全部入库状态，如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。";
                     *
                     */

                    message = string.Format(setting.WeiXinMessageTemplate1, messageData[0]);

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.DingShiTiJiaoDaBaoTiXing); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", messageData[0].ToString(), customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, string.Format(setting.SmsMessageTemplate, messageData[0]));
                            }
                        }
                    }
                }

                #endregion

                #region 公共申领件处理提醒

                if (messageType == CustomerMessageType.GongGongShenLingJianChuLiTiXing)
                {
                    if (messageData != null && messageData.Length != 0)
                    {
                        throw new Exception("公共申领件处理提醒 消息发送参数数量不对！");
                    }

                    /*
                     *
                     * 消息无参数
                     *
                    setting.WeiXinMessageTemplate = "亲，客服发现您有【提交打包】前状态的订单，不知公共申领区有没有您的包裹。方便的话，去看一下吧，万一有呢！";
                    setting.SmsMessageTemplate = "亲，您有【提交打包】前状态的订单，不知公共申领区有没有您的包裹。方便的话，去看一下吧，万一有呢！";
                     *
                     */

                    message = setting.WeiXinMessageTemplate1;

                    if (setting.Status == CustomerMessageSettingStatus.Enabled)
                    {
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.GongGongShenLingJianChuLiTiXing); // 实际周期发送的还不是可定制的
                        if (clientShouldSend)
                        {
                            if (setting.WeiXinSend)
                            {
                                isSent = true;
                                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "公共申领区寻宝提醒", customer?.CustomerCode, message, processUser);
                            }

                            if (setting.SmsSend)
                            {
                                isSent = true;
                                smsService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                            }
                        }
                    }
                }

                #endregion
            }

            CustomerMessageInfo customerMessage = new CustomerMessageInfo();
            customerMessage.SendType = sendType;
            customerMessage.Type = messageType;
            customerMessage.Customer = customer;

            // 其实可以合成一句 customerMessage.Order = order;

            if (order != null)
            {
                customerMessage.Order = order;
            }
            else
            {
                customerMessage.Order = null;
            }

            customerMessage.Content = message;
            customerMessage.SmsSend = smsSend;
            customerMessage.WeiXinSend = weiXinSend;
            customerMessage.PlanSendTime = DateTime.Now;
            customerMessage.SendTime = DateTime.Now;

            if (isSent)
            {
                customerMessage.Status = CustomerMessageStatus.Sent;
            }
            else
            {
                customerMessage.Status = CustomerMessageStatus.NotSend;
            }

            session.Save(customerMessage);

            session.Flush();
        }

        private CustomerMessageRealTimeSendSettingInfo GetCustomerMessageRealTimeSendSetting(CustomerMessageType messageType)
        {
            CustomerMessageRealTimeSendSettingInfo setting = session
                .Query<CustomerMessageRealTimeSendSettingInfo>()
                .Where(e => e.Type == messageType)
                .Take(1)
                .SingleOrDefault();

            if (setting == null)
            {
                // throw new Exception("指定的消息发送配置不存在！");
                return new CustomerMessageRealTimeSendSettingInfo() {
                    // 这样，新添加一个消息类型的时候，设置条目数据库还没生成，也不会在这里报错
                    Status = CustomerMessageSettingStatus.Disabled
                };
            }

            return setting;
        }

        private string GetProcessUser()
        {
            if (string.IsNullOrWhiteSpace(userName) == false)
            {
                return userName;
            }

            string processUser = defaultProcessUser;
            if (loginUserService != null) // 如果是定时任务的调用，这个值可能是null值
            {
                int loginUserId = loginUserService.GetLoginUserId();
                if (loginUserId != 0)
                {
                    string loginUserName = session.Get<UserInfo>(loginUserId)?.UserName;
                    if (string.IsNullOrWhiteSpace(loginUserName) == false)
                    {
                        processUser = loginUserName;
                    }
                }
            }

            return processUser;
        }

        private int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo order, string message, CustomerMessageStatus messageStatus, CustomerMessageRealTimeSendSettingInfo setting)
        {
            CustomerMessageInfo customerMessage = new CustomerMessageInfo();
            customerMessage.SendType = sendType;
            customerMessage.Type = setting.Type;
            customerMessage.Customer = customer;

            // 其实可以合成一句 customerMessage.Order = order;

            if (order != null)
            {
                customerMessage.Order = order;
            }
            else
            {
                customerMessage.Order = null;
            }

            customerMessage.Content = message;
            customerMessage.SmsSend = setting.SmsSend;
            customerMessage.WeiXinSend = setting.WeiXinSend;
            customerMessage.PlanSendTime = DateTime.Now;
            customerMessage.SendTime = DateTime.Now;
            customerMessage.Status = messageStatus;

            session.Save(customerMessage);
            session.Flush();

            return customerMessage.Id;
        }

        private int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, string message, CustomerMessageStatus messageStatus, CustomerMessageRealTimeSendSettingInfo setting)
        {
            return SaveCustomerMessage(sendType, customer, null, message, messageStatus, setting);
        }

        private int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo order, string message, bool isSent, CustomerMessageRealTimeSendSettingInfo setting)
        {
            var messageStatus = CustomerMessageStatus.NotSend;
            if (isSent)
            {
                messageStatus = CustomerMessageStatus.Sent;
            }

            return SaveCustomerMessage(sendType, customer, order, message, messageStatus, setting);
        }

        /// <summary>
        /// 一般保存的是订单为空的客户消息
        /// </summary>
        /// <param name="sendType"></param>
        /// <param name="customer"></param>
        /// <param name="message"></param>
        /// <param name="isSent"></param>
        /// <param name="setting"></param>
        private int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, string message, bool isSent, CustomerMessageRealTimeSendSettingInfo setting)
        {
            return SaveCustomerMessage(sendType, customer, null, message, isSent, setting);
        }
    }

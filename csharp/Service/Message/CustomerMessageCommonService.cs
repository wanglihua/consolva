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

namespace shunda.Service.CustomerMessage
{
    /// <summary>
    /// 客户消息共用处理
    /// </summary>
    [Service(ServiceType.Scoped)]
    public class CustomerMessageCommonService
    {
        /// <summary>
        /// 已到港 消息发送延迟
        /// </summary>
        public static readonly TimeSpan DelayYiDaoGangTiXing = new TimeSpan(2, 0, 0, 0); // TimeSpan(int days, int hours, int minutes, int seconds)

        /// <summary>
        /// 已到仓消息发送延迟
        /// </summary>
        public static readonly TimeSpan DelayYiDaoMuDiCangTiXing = new TimeSpan(2, 0, 0, 0); // TimeSpan(int days, int hours, int minutes, int seconds)


        public static readonly string DefaultProcessUser = "舜达小秘";

        public static readonly string PhotoMessageSender = "舜达智能拍照机器人";

        private ISession session = null;
        private SmsService smsService = null;
        private WechatMessageService wechatMessageService = null;
        private LoginUserService loginUserService = null;
        private CustomerMessageClientCustomizeService customerMessageClientCustomizeService = null;

        private string userName = string.Empty; // 初始是空的

        public CustomerMessageCommonService(ISession session,
            SmsService smsService,
            WechatMessageService wechatMessageService,
            LoginUserService loginUserService,
            CustomerMessageClientCustomizeService customerMessageClientCustomizeService)
        {
            this.session = session;
            this.smsService = smsService;
            this.wechatMessageService = wechatMessageService;
            this.loginUserService = loginUserService;
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

        /// <summary>
        /// 发送客户消息，带有订单参数
        /// todo a 这里的 消息发送会被改造
        /// </summary>
        public void SendMessageWithOrder(CustomerMessageSendType sendType, CustomerMessageType messageType, CustomerInfo customer, WeiXinFollowerInfo weiXinFollower, LogisticsOrderInfo order, params object[] messageData)
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

                if (messageType == CustomerMessageType.BangWoZhuanQianTiXing)
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
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.BangWoZhuanQianTiXing); // 实际周期发送的还不是可定制的
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

                if (messageType == CustomerMessageType.WeiZhuCeHuiYuanTuiSong)
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
                        bool clientShouldSend = customerMessageClientCustomizeService.ShouldSend(customerId, CustomerMessageType.WeiZhuCeHuiYuanTuiSong); // 实际周期发送的还不是可定制的
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

        public CustomerMessageRealTimeSendSettingInfo GetCustomerMessageRealTimeSendSetting(CustomerMessageType messageType)
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

        public string GetProcessUser()
        {
            if (string.IsNullOrWhiteSpace(userName) == false)
            {
                return userName;
            }

            string processUser = DefaultProcessUser;
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

        public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo order, string message, CustomerMessageStatus messageStatus, CustomerMessageRealTimeSendSettingInfo setting)
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

        public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, string message, CustomerMessageStatus messageStatus, CustomerMessageRealTimeSendSettingInfo setting)
        {
            return SaveCustomerMessage(sendType, customer, null, message, messageStatus, setting);
        }

        public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo order, string message, bool isSent, CustomerMessageRealTimeSendSettingInfo setting)
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
        public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, string message, bool isSent, CustomerMessageRealTimeSendSettingInfo setting)
        {
            return SaveCustomerMessage(sendType, customer, null, message, isSent, setting);
        }

        /// <summary>
        /// 发送订单通知
        /// </summary>
        /// <param name="customer">customer</param>
        /// <param name="first">first</param>
        /// <param name="orderCode">orderCode</param>
        /// <param name="customerCode">customerCode</param>
        /// <param name="remark">remark</param>
        /// <param name="sender">sender</param>
        /// <param name="url"></param>
        public void SendOrderNoticeTemplateMessage(CustomerInfo customer, string first, string orderCode, string customerCode, string remark, string sender, string url = null)
        {
            wechatMessageService.SendOrderNoticeTemplateMessage(customer, first, orderCode, customerCode, remark, sender, url);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">不加国家代号的手机号码</param>
        /// <param name="message">message</param>
        /// <returns></returns>
        public SendSmsResult SendSms(string mobile, string message)
        {
            return smsService.SendSms(mobile, message);
        }

        /// <summary>
        /// 按客户消息定制，是否发送
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public bool CustomerShouldSend(int customerId, CustomerMessageType messageType)
        {
            return customerMessageClientCustomizeService.ShouldSend(customerId, messageType);
        }
    }
}
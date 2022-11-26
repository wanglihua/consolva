using System;
using Core;
using Core.Config;
using Core.Util;
using Domain.Customer;
using Domain.Message;
using NHibernate;
using NLog;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Service.WeiXin;

namespace shunda.Service.CustomerMessage
{
    [Service(ServiceType.Scoped)]
    public class WeiXinMessageService
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private ISession session = null;
        private WeiXinAccessTokenService weiXinAccessTokenService = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="session"></param>
        /// <param name="weiXinAccessTokenService"></param>
        public WeiXinMessageService(ISession session, WeiXinAccessTokenService weiXinAccessTokenService)
        {
            this.session = session;
            this.weiXinAccessTokenService = weiXinAccessTokenService;
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="openId">openId</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <returns>是否发送成功</returns>
        public CustomerMessageStatus SendTemplateMessage(WeChatOfficialAccount account, string openId, string templateId, object data, string url = null)
        {
            if (string.IsNullOrWhiteSpace(openId) == false)
            {
                try
                {
                    TemplateApi.SendTemplateMessage(weiXinAccessTokenService.GetAccessToken(account), openId, templateId, url, data);
                    return CustomerMessageStatus.Sent;
                }
                catch (Exception exp)
                {
                    log.Error($"发送模板消息失败，openid: ${openId}, templateId: ${templateId}, data: ${data}");
                    log.Error(exp);

                    return CustomerMessageStatus.SendFail;
                }
            }

            return CustomerMessageStatus.SendFail; // 一般这时还没关注公众号，也算发送失败
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
        /// <returns>是否发送成功</returns>
        public CustomerMessageStatus SendOrderNoticeTemplateMessage(CustomerInfo customer, string first, string orderCode, string customerCode, string remark, string sender, string url = null)
        {
            WeChatOfficialAccount account = WeChatOfficialAccount.None;
            string openId = string.Empty;

            if (string.IsNullOrWhiteSpace(customer.OpenId) == false && customer.FollowStatus == WeiXinFollowStatus.Followed)
            {
                account = WeChatOfficialAccount.Account1;
                openId = customer.OpenId;
            }

            // 把 第二个账号的判断放在后面，这样他就优先使用后面的发送了
            if (string.IsNullOrWhiteSpace(customer.OpenId2) == false && customer.FollowStatus2 == WeiXinFollowStatus.Followed)
            {
                account = WeChatOfficialAccount.Account2;
                openId = customer.OpenId2;
            }

            // 尽量按第二个的来
            if (account == null || account == WeChatOfficialAccount.None)
            {
                account = WeChatOfficialAccount.Account2;
                openId = customer.OpenId2;
            }

            if (string.IsNullOrWhiteSpace(openId))
            {
                return CustomerMessageStatus.SendFail; // 一般这时还没关注公众号，也算发送失败
            }

#if DEBUG

            log.Trace("SendOrderNoticeTemplateMessage: ");
            log.Trace($"account: {account.Name}");
            log.Trace($"openId: {openId}");
            log.Trace($"first: {first}");
            log.Trace($"orderCode: {orderCode}");
            log.Trace($"customerCode: {customerCode}");
            log.Trace($"remark: {remark}");
            log.Trace($"sender: {sender}");
            log.Trace($"url: {url}");


            return CustomerMessageStatus.Sent;
#else
            DateTime processTime = DateTime.Now;

            var data = new {
                first = new TemplateDataItem(first),
                keyword1 = new TemplateDataItem(orderCode),
                keyword2 = new TemplateDataItem(customerCode),
                keyword3 = new TemplateDataItem(processTime.ToString("yyyy-MM-dd HH:mm") + " 北京时间"),
                keyword4 = new TemplateDataItem(sender),
                remark = new TemplateDataItem(remark)
            };

            if (account == WeChatOfficialAccount.Account2)
            {
                return SendTemplateMessage(account, openId, WeChatConfig2.OrderNoticeTemplateId, data, url);
            }
            else if (account == WeChatOfficialAccount.Account1
                     || account == WeChatOfficialAccount.None
                     || account == null)
            {
                return SendTemplateMessage(account, openId, WeChatConfig1.OrderNoticeTemplateId, data, url);
            }

            return CustomerMessageStatus.SendFail;
#endif
        }

        /// <summary>
        /// 发送订单通知
        /// </summary>
        /// <param name="weiXinFollower">customer</param>
        /// <param name="first">first</param>
        /// <param name="orderCode">orderCode</param>
        /// <param name="customerCode">customerCode</param>
        /// <param name="remark">remark</param>
        /// <param name="sender">sender</param>
        /// <returns>是否发送成功</returns>
        public CustomerMessageStatus SendOrderNoticeTemplateMessage(WeiXinFollowerInfo weiXinFollower, string first, string orderCode, string customerCode, string remark, string sender)
        {
            // 这个方法也就用来做 未注册会员推送， 所以原公众号的也就不推送了
            if (weiXinFollower.Account == WeChatOfficialAccount.Account1)
            {
                return CustomerMessageStatus.SendFail; // 也算发送失败吧
            }

            WeChatOfficialAccount account = weiXinFollower.Account;
            string openId = weiXinFollower.OpenId;

#if DEBUG

            log.Trace("SendOrderNoticeTemplateMessage: ");
            log.Trace($"account: {account.Name}");
            log.Trace($"openId: {openId}");
            log.Trace($"first: {first}");
            log.Trace($"orderCode: {orderCode}");
            log.Trace($"customerCode: {customerCode}");
            log.Trace($"remark: {remark}");
            log.Trace($"sender: {sender}");


            return CustomerMessageStatus.Sent;
#else
            DateTime processTime = DateTime.Now;

            var data = new {
                first = new TemplateDataItem(first),
                keyword1 = new TemplateDataItem(orderCode),
                keyword2 = new TemplateDataItem(customerCode),
                keyword3 = new TemplateDataItem(processTime.ToString("yyyy-MM-dd HH:mm") + " 北京时间"),
                keyword4 = new TemplateDataItem(sender),
                remark = new TemplateDataItem(remark)
            };

            if (account == WeChatOfficialAccount.Account2)
            {
                return SendTemplateMessage(account, openId, WeChatConfig2.OrderNoticeTemplateId, data);
            }
            else if (account == WeChatOfficialAccount.Account1
                     || account == WeChatOfficialAccount.None
                     || account == null)
            {
                return SendTemplateMessage(account, openId, WeChatConfig1.OrderNoticeTemplateId, data);
            }
            
            return CustomerMessageStatus.SendFail;
#endif
        }

        /// <summary>
        /// 订单处理异常通知
        /// </summary>
        /// <param name="customer">customer</param>
        /// <param name="first">first</param>
        /// <param name="abnormalCause">异常原因</param>
        /// <param name="remark">remark</param>
        /// <param name="sender">sender</param>
        /// <returns>是否发送成功</returns>
        public CustomerMessageStatus SendOrderAbnormalNoticeTemplateMessage(CustomerInfo customer, string first, string abnormalCause, string remark, string sender)
        {
            WeChatOfficialAccount account = WeChatOfficialAccount.None;
            string openId = string.Empty;

            if (string.IsNullOrWhiteSpace(customer.OpenId2) == false && customer.FollowStatus2 == WeiXinFollowStatus.Followed)
            {
                account = WeChatOfficialAccount.Account2;
                openId = customer.OpenId2;
            }

            if (string.IsNullOrWhiteSpace(customer.OpenId) == false && customer.FollowStatus == WeiXinFollowStatus.Followed)
            {
                account = WeChatOfficialAccount.Account1;
                openId = customer.OpenId;
            }

#if DEBUG
            log.Trace("SendOrderAbnormalNoticeTemplateMessage: ");
            log.Trace($"account: {account.Name}");
            log.Trace($"openId: {openId}");
            log.Trace($"first: {first}");
            log.Trace($"abnormalCause: {abnormalCause}");
            log.Trace($"remark: {remark}");
            log.Trace($"sender: {sender}");

            return CustomerMessageStatus.Sent;
#else
            DateTime abnormalTime = DateTime.Now;

            var data = new {
                first = new TemplateDataItem(first),
                keyword1 = new TemplateDataItem(abnormalCause),
                keyword2 = new TemplateDataItem(abnormalTime.ToString("yyyy-MM-dd HH:mm") + " 北京时间"),
                remark = new TemplateDataItem(remark + "【" + sender + "】")
            };

            if (account == WeChatOfficialAccount.Account2)
            {
                return SendTemplateMessage(account, openId, WeChatConfig2.OrderAbnormalTemplateId, data);
            }
            else if (account == WeChatOfficialAccount.Account1
                     || account == WeChatOfficialAccount.None
                     || account == null)
            {
                return SendTemplateMessage(account, openId, WeChatConfig1.OrderAbnormalTemplateId, data);
            }
            
            return CustomerMessageStatus.SendFail;
#endif
        }
    }
}
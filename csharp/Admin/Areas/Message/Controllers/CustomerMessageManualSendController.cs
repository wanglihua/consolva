using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Message.Models.CustomerMessageManualSend;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.CustomerMessage;
using shunda.Service.Message;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Message.Controllers;

[MessageArea]
[AdminAuth(PassRes.CustomerMessage.ManualSend)]
public class CustomerMessageManualSendController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatMessageService wechatMessageService;
    private readonly SmsService smsService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageManualSendController(
        ISession session,
        WechatMessageService wechatMessageService,
        SmsService smsService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.wechatMessageService = wechatMessageService;
        this.smsService = smsService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/message/manual/send")]
    [AdminAuth(PassRes.CustomerMessage.ManualSend)]
    public IActionResult Index()
    {
        CustomerMessageManualSendModel model = new CustomerMessageManualSendModel();
        return View("CustomerMessageManualSendIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/manual/send")]
    [AdminAuth(PassRes.CustomerMessage.ManualSend)]
    public IActionResult Send(CustomerMessageManualSendModel model)
    {
        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);
        string loginUserName = loginUser.UserName;


        if (model.SendByWechat == false && model.SendBySms == false)
        {
            throw new ValidationException("微信发送或短信发送，必须至少选择其中一项！");
        }

        if (model.SendBySms == true)
        {
            int length = StringUtil.GetStringLengthInByte(model.RemarkMessage);
            if (length > 100)
            {
                throw new ValidationException("短信内容，不能超过 50 个汉字， 100个英文字母！");
            }
        }
        else
        {
            int length = StringUtil.GetStringLengthInByte(model.RemarkMessage);
            if (length > 240)
            {
                throw new ValidationException("微信消息内容，不能超过 120 个汉字，240个英文字母！");
            }
        }

        IList<int> customerIdList = new List<int>();

        IList<string> weiXinFailCustomerList = new List<string>();
        IList<string> weiXinSuccessCustomerList = new List<string>();

        IList<string> smsFailCustomerList = new List<string>();
        IList<string> smsSuccessCustomerList = new List<string>();

        string customerIds = model.CustomerIds.Replace("\r\n", ",");

        string[] customerIdStrArray = customerIds.Split(' ', ',');

        if (model.SendByWechat)
        {
            foreach (string customerIdStr in customerIdStrArray)
            {
                if (int.TryParse(customerIdStr, out int customerId) == false)
                {
                    // throw new ValidationException($"{customerIdStr} 不是正确的客户ID数字部分！");

                    weiXinFailCustomerList.Add(customerIdStr);
                    continue;
                }

                CustomerInfo customer = session.Get<CustomerInfo>(customerId);
                if (customer == null)
                {
                    // throw new ValidationException($"指定的客户ID{customerId}不存在！");

                    weiXinFailCustomerList.Add(customerIdStr);
                    continue;
                }

                if (customer.Status == CustomerStatus.Frozen)
                {
                    weiXinFailCustomerList.Add(customerIdStr);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(customer.OpenId))
                {
                    // throw new ValidationException($"指定的客户ID{customerId} open id 不存在，可能是因为没关注公众号！");

                    weiXinFailCustomerList.Add(customerIdStr);
                    continue;
                }

                /*
                WechatFollowerInfo weiXinFollower = session
                    .Query<WechatFollowerInfo>()
                    .Where(e => e.OpenId == customer.OpenId)
                    .Take(1)
                    .SingleOrDefault();

                if (weiXinFollower != null && weiXinFollower.FollowStatus == WechatFollowStatus.Followed)
                {
                    weiXinSuccessCustomerList.Add(customerIdStr);
                }
                else
                {
                    weiXinFailCustomerList.Add(customerIdStr); //这里后面就不放 continue，因为毕竟 open id 是存在的，万一成功了呢？这关注数据可能会有误。
                }
                */
                
                string customerCode = customerCodeService.GetCustomerCode(customer.Id);
                wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "手动推送", customerCode, model.RemarkMessage, loginUserName);

                if (customerIdList.Contains(customer.Id) == false)
                {
                    weiXinSuccessCustomerList.Add(customerIdStr);

                    customerIdList.Add(customer.Id);
                }
            }
        }

        if (model.SendBySms)
        {
            foreach (string customerIdStr in customerIdStrArray)
            {
                if (int.TryParse(customerIdStr, out int customerId) == false)
                {
                    // throw new ValidationException($"{customerIdStr} 不是正确的客户ID数字部分！");

                    smsFailCustomerList.Add(customerIdStr);
                    continue;
                }

                CustomerInfo customer = session.Get<CustomerInfo>(customerId);
                if (customer == null)
                {
                    // throw new ValidationException($"指定的客户ID{customerId}不存在！");

                    smsFailCustomerList.Add(customerIdStr);
                    continue;
                }

                if (customer.Status == CustomerStatus.Frozen)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(customer.Mobile))
                {
                    // throw new ValidationException($"指定的客户ID{customerId} 手机号码 不存在！");

                    smsFailCustomerList.Add(customerIdStr);
                    continue;
                }

                SendSmsResult result = smsService.SendSms(customer.Mobile, model.RemarkMessage);
                if (customerIdList.Contains(customer.Id) == false)
                {
                    customerIdList.Add(customer.Id);
                }

                if (result.Code == SendSmsResult.CodeSuccess)
                {
                    smsSuccessCustomerList.Add(customerIdStr);
                }
                else
                {
                    smsFailCustomerList.Add(customerIdStr);
                }
            }
        }

        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            foreach (int customerId in customerIdList)
            {
                CustomerMessageInfo customerMessage = new CustomerMessageInfo();
                customerMessage.SendType = CustomerMessageSendType.Manual;
                customerMessage.Type = CustomerMessageType.Manual;
                customerMessage.Customer = session.Load<CustomerInfo>(customerId);
                customerMessage.Content = model.RemarkMessage;
                customerMessage.SmsSend = model.SendBySms;
                customerMessage.WechatSend = model.SendByWechat;
                customerMessage.SendTime = DateTime.Now;
                customerMessage.Status = CustomerMessageStatus.Sent;
                customerMessage.Sender = loginUserName;

                session.Save(customerMessage);
            }

            session.Flush();

            transaction.Commit();
        }
        catch (Exception exp)
        {
            transaction?.Rollback();

            log.Error(exp);

            throw;
        }

        /*
        IList<string> weiXinFailCustomerList = new List<string>();
        IList<string> weiXinSuccessCustomerList = new List<string>();

        IList<string> smsFailCustomerList = new List<string>();
        IList<string> smsSuccessCustomerList = new List<string>();
         */

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "发送成功！" },
            { "sendByWechat", model.SendByWechat },
            { "sendBySms", model.SendBySms },
            { "weiXinFailCustomers", string.Join(", ", weiXinFailCustomerList) },
            { "weiXinSuccessCustomers", string.Join(", ", weiXinSuccessCustomerList) },
            { "smsFailCustomers", string.Join(", ", smsFailCustomerList) },
            { "smsSuccessCustomers", string.Join(", ", smsSuccessCustomerList) },
        });
    }

    [HttpGet]
    [Route("/admin/customer/message/manual/send/all")]
    [AdminAuth(PassRes.CustomerMessage.ManualSendAll)]
    public IActionResult SendAll()
    {
        CustomerMessageManualSendAllModel model = new CustomerMessageManualSendAllModel();

        return View("CustomerMessageManualSendAll", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/manual/send/all")]
    [AdminAuth(PassRes.CustomerMessage.ManualSendAll)]
    public IActionResult SendAll(CustomerMessageManualSendAllModel model)
    {
        // 发送一条保存一条。同时这里有个数据库事务，发送一条就是一个事务。现在是整个操作是一个事务，出了状况后，保存的数据（那怕是发一条记录一条），也会被回滚。

        if (model.SendByWechat == false && model.SendBySms == false)
        {
            throw new ValidationException("微信发送或短信发送，必须至少选择其中一项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);
        string loginUserName = loginUser.UserName;

        IList<CustomerInfo> customerList = session
            .Query<CustomerInfo>()
            .Where(e => (e.OpenId != null && e.OpenId != "") || (e.Mobile != null && e.Mobile != ""))
            .OrderBy(e => e.Id)
            .ToList();

        foreach (CustomerInfo customer in customerList)
        {
            if (customer.Status == CustomerStatus.Frozen)
            {
                continue;
            }

            ITransaction? transaction = null;

            try
            {
                transaction = session.BeginTransaction();

                bool sent = false;

                if (model.SendByWechat && (string.IsNullOrWhiteSpace(customer.OpenId) == false))
                {
                    sent = true;
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);
                    wechatMessageService.SendOrderNoticeTemplateMessage(customer, "", "手动推送", customerCode, model.RemarkMessage, loginUserName);
                }

                if (model.SendBySms && string.IsNullOrWhiteSpace(customer.Mobile) == false)
                {
                    sent = true;
                    smsService.SendSms(customer.Mobile, model.RemarkMessage);
                }

                if (sent == true)
                {
                    CustomerMessageInfo customerMessage = new CustomerMessageInfo();
                    customerMessage.SendType = CustomerMessageSendType.Manual;
                    customerMessage.Type = CustomerMessageType.AllMember;
                    customerMessage.Customer = session.Load<CustomerInfo>(customer.Id);
                    customerMessage.Content = model.RemarkMessage;
                    customerMessage.SmsSend = model.SendBySms;
                    customerMessage.WechatSend = model.SendByWechat;
                    customerMessage.SendTime = DateTime.Now;
                    customerMessage.Status = CustomerMessageStatus.Sent;
                    customerMessage.Sender = loginUserName;

                    session.Save(customerMessage);
                }

                session.Flush();
                transaction.Commit();
            }
            catch (Exception exp)
            {
                log.Error(exp);

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "发送成功！" },
        });
    }
}
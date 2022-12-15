using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Message.Models.CustomerMessageCycleTimingSendSetting;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Message;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Message;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Message.Controllers;

[MessageArea]
[AdminAuth(PassRes.CustomerMessage.MessageSetting)]
public class CustomerMessageCycleTimingSendSettingController : Controller
{
    private readonly ISession session;
    private readonly CustomerMessageCycleTimingSendSettingService customerMessageCycleTimingSendSettingService;
    private readonly CustomerMessageCommonService customerMessageCommonService;

    public CustomerMessageCycleTimingSendSettingController(
        ISession session,
        CustomerMessageCycleTimingSendSettingService customerMessageCycleTimingSendSettingService,
        CustomerMessageCommonService customerMessageCommonService)
    {
        this.session = session;
        this.customerMessageCycleTimingSendSettingService = customerMessageCycleTimingSendSettingService;
        this.customerMessageCommonService = customerMessageCommonService;
    }

    [HttpGet]
    [Route("/admin/customer/message/cycle/timing/send/setting")]
    public IActionResult Index()
    {
        customerMessageCycleTimingSendSettingService.InitSettings();

        CustomerMessageCycleTimingSendSettingIndexModel model = new CustomerMessageCycleTimingSendSettingIndexModel();

        return View("CustomerMessageCycleTimingSendSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/cycle/timing/send/setting/list")]
    public IActionResult List(CustomerMessageCycleTimingSendSettingListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerMessageCycleTimingSendSettingInfo> customerMessageCycleTimingSendSettingList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerMessageCycleTimingSendSettingInfo>();

        foreach (CustomerMessageCycleTimingSendSettingInfo customerMessageCycleTimingSendSettingInfo in customerMessageCycleTimingSendSettingList)
        {
            // customerMessageCycleTimingSendSettingInfo
        }

        CustomerMessageCycleTimingSendSettingListModel model = new CustomerMessageCycleTimingSendSettingListModel(listPager);
        model.CustomerMessageCycleTimingSendSettingList = customerMessageCycleTimingSendSettingList;

        return View("CustomerMessageCycleTimingSendSettingList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerMessageCycleTimingSendSettingListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerMessageCycleTimingSendSettingInfo>();

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/message/cycle/timing/send/setting/detail")]
    public IActionResult Detail(int id)
    {
        session.DefaultReadOnly = true;

        CustomerMessageCycleTimingSendSettingInfo setting = session.Get<CustomerMessageCycleTimingSendSettingInfo>(id);
        if (setting == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        CustomerMessageCycleTimingSendSettingDetailModel model = new CustomerMessageCycleTimingSendSettingDetailModel();
        model.SettingId = setting.Id;
        model.Type = setting.Type;
        model.WechatMessageTemplate1 = setting.WechatMessageTemplate1;
        model.WechatMessageTemplate2 = setting.WechatMessageTemplate2;
        model.WechatMessageTemplate3 = setting.WechatMessageTemplate3;
        model.SmsMessageTemplate = setting.SmsMessageTemplate;
        model.Status = setting.Status;
        model.SmsSend = setting.SmsSend;
        model.WechatSend = setting.WechatSend;
        model.IntervalType = setting.IntervalType;
        model.IntervalWeeks = setting.IntervalWeeks;
        model.IntervalDays = setting.IntervalDays;
        model.SendWeekday = setting.SendWeekday;
        model.SendHour = setting.SendHour;
        model.ClientCustomizable = setting.ClientCustomizable;
        model.ClientTitle = setting.ClientTitle;
        model.ClientDesc = setting.ClientDesc;

        model.MessageTypeList = CustomerMessageType.GetList();
        model.StatusList = CustomerMessageSettingStatus.GetList();
        model.IntervalTypeList = CustomerMessageCycleTimingSendIntervalType.GetList();

        // 发送周期列表
        IList<int> intervalWeeksList = new List<int>();
        for (int i = 1; i <= 20; i++)
        {
            intervalWeeksList.Add(i);
        }

        model.IntervalWeeksList = intervalWeeksList;

        // 发送周期列表
        IList<int> intervalDaysList = new List<int>();
        for (int i = 1; i <= 20; i++)
        {
            intervalDaysList.Add(i);
        }

        model.IntervalDaysList = intervalDaysList;

        // 发送天列表
        IList<int> sendWeekdayList = new List<int>();
        for (int i = 0; i < 7; i++)
        {
            sendWeekdayList.Add(i);
        }

        model.SendWeekDayList = sendWeekdayList;

        // 发送小时列表
        IList<int> sendHourList = new List<int>();
        for (int i = 0; i < 24; i++)
        {
            sendHourList.Add(i);
        }

        model.SendHourList = sendHourList;

        return View("CustomerMessageCycleTimingSendSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/cycle/timing/send/setting/save")]
    [Transactional]
    public IActionResult Save(CustomerMessageCycleTimingSendSettingSaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int settingId = model.SettingId;

        CustomerMessageCycleTimingSendSettingInfo setting = session.Get<CustomerMessageCycleTimingSendSettingInfo>(settingId);

        if (setting == null)
        {
            throw new ValidationException("配置信息不存在！");
        }

        // 这里不保存 type

        setting.WechatMessageTemplate1 = model.WechatMessageTemplate1;
        setting.WechatMessageTemplate2 = model.WechatMessageTemplate2 ?? string.Empty;
        setting.WechatMessageTemplate3 = model.WechatMessageTemplate3 ?? string.Empty;
        setting.SmsMessageTemplate = model.SmsMessageTemplate;
        setting.Status = model.Status;
        setting.SmsSend = model.SmsSend;
        setting.WechatSend = model.WechatSend;
        setting.IntervalType = model.IntervalType;
        setting.IntervalWeeks = model.IntervalWeeks;
        setting.IntervalDays = model.IntervalDays;
        setting.SendWeekday = model.SendWeekday;
        setting.SendHour = model.SendHour;
        setting.ClientCustomizable = model.ClientCustomizable;
        setting.ClientTitle = model.ClientTitle;
        setting.ClientDesc = model.ClientDesc;

        session.Update(setting);

        session.Flush();

        customerMessageCommonService.MakeSettingCacheInvalid();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}
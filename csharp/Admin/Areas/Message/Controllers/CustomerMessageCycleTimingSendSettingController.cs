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
[AdminAuth(PassRes.CustomerMessage.CycleTiming)]
public class CustomerMessageCycleTimingSendSettingController : Controller
{
    private ISession session;
    private CustomerMessageCycleTimingSendSettingService customerMessageCycleTimingSendSettingService;

    public CustomerMessageCycleTimingSendSettingController(
        ISession session,
        CustomerMessageCycleTimingSendSettingService customerMessageCycleTimingSendSettingService)
    {
        this.session = session;
        this.customerMessageCycleTimingSendSettingService = customerMessageCycleTimingSendSettingService;
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

        CustomerMessageCycleTimingSendSettingInfo customerMessageCycleTimingSendSetting = session.Get<CustomerMessageCycleTimingSendSettingInfo>(id);
        if (customerMessageCycleTimingSendSetting == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        CustomerMessageCycleTimingSendSettingDetailModel model = new CustomerMessageCycleTimingSendSettingDetailModel();
        model.CustomerMessageCycleTimingSendSetting = customerMessageCycleTimingSendSetting;

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

        CustomerMessageCycleTimingSendSettingInfo customerMessageCycleTimingSendSettingFromUi = model.CustomerMessageCycleTimingSendSetting;
        int customerMessageCycleTimingSendSettingId = customerMessageCycleTimingSendSettingFromUi.Id;

        CustomerMessageCycleTimingSendSettingInfo customerMessageCycleTimingSendSetting = session.Get<CustomerMessageCycleTimingSendSettingInfo>(customerMessageCycleTimingSendSettingId);

        if (customerMessageCycleTimingSendSetting == null)
        {
            throw new ValidationException("配置信息不存在！");
        }

        customerMessageCycleTimingSendSetting.WechatMessageTemplate1 = customerMessageCycleTimingSendSettingFromUi.WechatMessageTemplate1;
        customerMessageCycleTimingSendSetting.WechatMessageTemplate2 = customerMessageCycleTimingSendSettingFromUi.WechatMessageTemplate2;
        customerMessageCycleTimingSendSetting.WechatMessageTemplate3 = customerMessageCycleTimingSendSettingFromUi.WechatMessageTemplate3;
        customerMessageCycleTimingSendSetting.SmsMessageTemplate = customerMessageCycleTimingSendSettingFromUi.SmsMessageTemplate;
        customerMessageCycleTimingSendSetting.Status = customerMessageCycleTimingSendSettingFromUi.Status;
        customerMessageCycleTimingSendSetting.SmsSend = customerMessageCycleTimingSendSettingFromUi.SmsSend;
        customerMessageCycleTimingSendSetting.WechatSend = customerMessageCycleTimingSendSettingFromUi.WechatSend;
        customerMessageCycleTimingSendSetting.IntervalType = customerMessageCycleTimingSendSettingFromUi.IntervalType;
        customerMessageCycleTimingSendSetting.IntervalWeeks = customerMessageCycleTimingSendSettingFromUi.IntervalWeeks;
        customerMessageCycleTimingSendSetting.IntervalDays = customerMessageCycleTimingSendSettingFromUi.IntervalDays;
        customerMessageCycleTimingSendSetting.SendWeekday = customerMessageCycleTimingSendSettingFromUi.SendWeekday;
        customerMessageCycleTimingSendSetting.SendHour = customerMessageCycleTimingSendSettingFromUi.SendHour;

        session.Update(customerMessageCycleTimingSendSetting);

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}
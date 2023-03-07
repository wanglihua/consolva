using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Message.Models.CustomerMessageRealTimeSendSetting;
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
public class CustomerMessageRealTimeSendSettingController : Controller {
    private readonly ISession session;
    private readonly CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService;
    private readonly CustomerMessageCommonService customerMessageCommonService;

    public CustomerMessageRealTimeSendSettingController(
        ISession session,
        CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService,
        CustomerMessageCommonService customerMessageCommonService) {
        this.session = session;
        this.customerMessageRealTimeSendSettingService = customerMessageRealTimeSendSettingService;
        this.customerMessageCommonService = customerMessageCommonService;
    }

    [HttpGet]
    [Route("/admin/customer/message/real/time/send/setting")]
    [Transactional]
    public IActionResult Index() {
        CustomerMessageRealTimeSendSettingIndexModel model = new CustomerMessageRealTimeSendSettingIndexModel();

        customerMessageRealTimeSendSettingService.InitSetting();

        return View("CustomerMessageRealTimeSendSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/real/time/send/setting/list")]
    public IActionResult List(CustomerMessageRealTimeSendSettingListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerMessageRealTimeSendSettingInfo> customerMessageRealTimeSendSettingList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerMessageRealTimeSendSettingInfo>();

        foreach (CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSettingInfo in customerMessageRealTimeSendSettingList) {
            // customerMessageRealTimeSendSettingInfo
        }

        CustomerMessageRealTimeSendSettingListModel model = new CustomerMessageRealTimeSendSettingListModel(listPager);
        model.CustomerMessageRealTimeSendSettingList = customerMessageRealTimeSendSettingList;

        return View("CustomerMessageRealTimeSendSettingList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerMessageRealTimeSendSettingListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<CustomerMessageRealTimeSendSettingInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.TypeNameQuery) == false) {
            string typeNameQuery = queryParam.TypeNameQuery.Trim();
            IList<CustomerMessageType> typeList = CustomerMessageType.GetList()
                .Where(e => e.Name.Contains(typeNameQuery))
                .ToList();

            criteria = criteria.Add(Restrictions.In($"{nameof(CustomerMessageRealTimeSendSettingInfo.Type)}", typeList.ToArray()));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/message/real/time/send/setting/detail")]
    public IActionResult Detail(int id) {
        session.DefaultReadOnly = true;

        CustomerMessageRealTimeSendSettingInfo setting = session.Get<CustomerMessageRealTimeSendSettingInfo>(id);
        if (setting == null) {
            throw new ValidationException("指定的数据项不存在！");
        }

        CustomerMessageRealTimeSendSettingDetailModel model = new CustomerMessageRealTimeSendSettingDetailModel();
        model.SettingId = setting.Id;
        model.Type = setting.Type;
        model.WechatMessageTemplate1 = setting.WechatMessageTemplate1;
        model.WechatMessageTemplate2 = setting.WechatMessageTemplate2;
        model.WechatMessageTemplate3 = setting.WechatMessageTemplate3;
        model.SmsMessageTemplate = setting.SmsMessageTemplate;
        model.Status = setting.Status;
        model.SmsSend = setting.SmsSend;
        model.WechatSend = setting.WechatSend;
        model.Placeholders = setting.Placeholders;
        model.ClientCustomizable = setting.ClientCustomizable;
        model.ClientTitle = setting.ClientTitle;
        model.ClientDesc = setting.ClientDesc;

        model.MessageTypeList = CustomerMessageType.GetList();
        model.StatusList = CustomerMessageSettingStatus.GetList();

        return View("CustomerMessageRealTimeSendSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/real/time/send/setting/save")]
    [Transactional]
    public IActionResult Save(CustomerMessageRealTimeSendSettingSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int settingId = model.SettingId;

        CustomerMessageRealTimeSendSettingInfo setting = session.Get<CustomerMessageRealTimeSendSettingInfo>(settingId);
        if (setting == null) {
            throw new ValidationException("配置信息不存在！");
        }

        setting.WechatMessageTemplate1 = model.WechatMessageTemplate1;
        setting.WechatMessageTemplate2 = model.WechatMessageTemplate2 ?? string.Empty;
        setting.WechatMessageTemplate3 = model.WechatMessageTemplate3 ?? string.Empty;
        setting.SmsMessageTemplate = model.SmsMessageTemplate;
        setting.Status = model.Status;
        setting.SmsSend = model.SmsSend;
        setting.WechatSend = model.WechatSend;
        // 占位符 PlaceHolders 不在这里保存修改

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
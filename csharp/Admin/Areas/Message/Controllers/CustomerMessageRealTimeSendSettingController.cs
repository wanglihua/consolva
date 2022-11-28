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
[AdminAuth(PassRes.CustomerMessage.RealTime)]
public class CustomerMessageRealTimeSendSettingController : Controller
{
    private ISession session;
    private CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService;

    public CustomerMessageRealTimeSendSettingController(ISession session, CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService)
    {
        this.session = session;
        this.customerMessageRealTimeSendSettingService = customerMessageRealTimeSendSettingService;
    }

    [HttpGet]
    [Route("/admin/customer/message/real/time/send/setting")]
    public IActionResult Index()
    {
        CustomerMessageRealTimeSendSettingIndexModel model = new CustomerMessageRealTimeSendSettingIndexModel();

        customerMessageRealTimeSendSettingService.InitSetting();

        return View("CustomerMessageRealTimeSendSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/real/time/send/setting/list")]
    public IActionResult List(CustomerMessageRealTimeSendSettingListQueryParam queryParam)
    {
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

        foreach (CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSettingInfo in customerMessageRealTimeSendSettingList)
        {
            // customerMessageRealTimeSendSettingInfo
        }

        CustomerMessageRealTimeSendSettingListModel model = new CustomerMessageRealTimeSendSettingListModel(listPager);
        model.CustomerMessageRealTimeSendSettingList = customerMessageRealTimeSendSettingList;

        return View("CustomerMessageRealTimeSendSettingList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerMessageRealTimeSendSettingListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerMessageRealTimeSendSettingInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.TypeNameQuery) == false)
        {
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
    public IActionResult Detail(int id)
    {
        session.DefaultReadOnly = true;

        CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSetting = session.Get<CustomerMessageRealTimeSendSettingInfo>(id);
        if (customerMessageRealTimeSendSetting == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        CustomerMessageRealTimeSendSettingDetailModel model = new CustomerMessageRealTimeSendSettingDetailModel();
        model.CustomerMessageRealTimeSendSetting = customerMessageRealTimeSendSetting;

        model.MessageTypeList = CustomerMessageType.GetList();
        model.StatusList = CustomerMessageSettingStatus.GetList();

        return View("CustomerMessageRealTimeSendSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/message/real/time/send/setting/save")]
    [Transactional]
    public IActionResult Save(CustomerMessageRealTimeSendSettingSaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSettingFromUi = model.CustomerMessageRealTimeSendSetting;
        int customerMessageRealTimeSendSettingId = customerMessageRealTimeSendSettingFromUi.Id;

        CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSetting = session.Get<CustomerMessageRealTimeSendSettingInfo>(customerMessageRealTimeSendSettingId);
        if (customerMessageRealTimeSendSetting == null)
        {
            throw new ValidationException("配置信息不存在！");
        }

        customerMessageRealTimeSendSetting.WechatMessageTemplate1 = customerMessageRealTimeSendSettingFromUi.WechatMessageTemplate1;
        customerMessageRealTimeSendSetting.WechatMessageTemplate2 = customerMessageRealTimeSendSettingFromUi.WechatMessageTemplate2;
        customerMessageRealTimeSendSetting.WechatMessageTemplate3 = customerMessageRealTimeSendSettingFromUi.WechatMessageTemplate3;
        customerMessageRealTimeSendSetting.SmsMessageTemplate = customerMessageRealTimeSendSettingFromUi.SmsMessageTemplate;
        customerMessageRealTimeSendSetting.Status = customerMessageRealTimeSendSettingFromUi.Status;
        customerMessageRealTimeSendSetting.SmsSend = customerMessageRealTimeSendSettingFromUi.SmsSend;
        customerMessageRealTimeSendSetting.WechatSend = customerMessageRealTimeSendSettingFromUi.WechatSend;
        // 占位符 PlaceHolders 不在这里保存修改

        customerMessageRealTimeSendSetting.ClientCustomizable = customerMessageRealTimeSendSettingFromUi.ClientCustomizable;
        if (customerMessageRealTimeSendSettingFromUi.ClientTitle == null)
        {
            customerMessageRealTimeSendSettingFromUi.ClientTitle = String.Empty;
        }

        customerMessageRealTimeSendSetting.ClientTitle = customerMessageRealTimeSendSettingFromUi.ClientTitle;

        if (customerMessageRealTimeSendSettingFromUi.ClientDesc == null)
        {
            customerMessageRealTimeSendSettingFromUi.ClientDesc = string.Empty;
        }

        customerMessageRealTimeSendSetting.ClientDesc = customerMessageRealTimeSendSettingFromUi.ClientDesc;

        session.Update(customerMessageRealTimeSendSetting);

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}
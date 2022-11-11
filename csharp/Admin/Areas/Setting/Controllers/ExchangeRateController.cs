using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Setting.Models.ExchangeRate;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

[SettingArea]
[AdminAuth(PassRes.SystemSetting.ExchangeRate)]
public class ExchangeRateController : Controller
{
    private readonly ExchangeRateService exchangeRateService;
    private readonly ISession session;

    public ExchangeRateController(ISession session, ExchangeRateService exchangeRateService)
    {
        this.session = session;
        this.exchangeRateService = exchangeRateService;
    }

    [HttpGet]
    [Route("/admin/exchange/rate")]
    public IActionResult Index()
    {
        var model = new ExchangeRateIndexModel();
        return View("ExchangeRateIndex", model);
    }

    [HttpPost]
    [Route("/admin/exchange/rate/list")]
    public IActionResult List(ExchangeRateListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var exchangeRateList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ExchangeRateInfo>();

        var model = new ExchangeRateListModel(listPager);
        model.ExchangeRateList = exchangeRateList;

        return View("ExchangeRateList", model);
    }

    private ICriteria GetListCriteria(ExchangeRateListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<ExchangeRateInfo>();
        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/exchange/rate/detail")]
    public IActionResult Detail(IdParam param)
    {
        session.DefaultReadOnly = true;

        var exchangeRate = session.Get<ExchangeRateInfo>(param.Id);
        if (exchangeRate == null) throw new ValidationException($"指定的汇率数据项#{param.Id}不存在！");

        var model = new ExchangeRateDetailModel();
        model.ExchangeRate = exchangeRate;

        model.ExchangeRateId = exchangeRate.Id;
        model.RateFix = exchangeRate.RateFix;

        return View("ExchangeRateDetail", model);
    }

    [HttpPost]
    [Route("/admin/exchange/rate/save")]
    [Transactional]
    public IActionResult Save(ExchangeRateSaveModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var exchangeRateId = model.ExchangeRateId;

        var exchangeRate = session.Get<ExchangeRateInfo>(exchangeRateId);
        if (exchangeRate == null) throw new ValidationException($"指定的汇率数据项#{exchangeRateId}不存在！");

        exchangeRate.RateFix = model.RateFix;

        session.Update(exchangeRate);
        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/exchange/rate/refresh")]
    // trans in exchangeRateService.RefreshRate();
    public IActionResult Refresh()
    {
        exchangeRateService.RefreshRate();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "更新成功！" }
        });
    }
}
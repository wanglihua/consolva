using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Setting.Models.ExchangeRate;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

[SettingArea]
[AdminAuth(PassRes.SystemSetting.ExchangeRate)]
public class ExchangeRateController : Controller
{
    private ISession session;
    private ExchangeRateService exchangeRateService;

    public ExchangeRateController(ISession session, ExchangeRateService exchangeRateService)
    {
        this.session = session;
        this.exchangeRateService = exchangeRateService;
    }

    [HttpGet]
    [Route("/admin/exchange/rate")]
    public IActionResult Index()
    {
        ExchangeRateIndexModel model = new ExchangeRateIndexModel();
        return View("ExchangeRateIndex", model);
    }

    [HttpPost]
    [Route("/admin/exchange/rate/list")]
    public IActionResult List(ExchangeRateListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExchangeRateInfo> exchangeRateList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ExchangeRateInfo>();

        ExchangeRateListModel model = new ExchangeRateListModel(listPager);
        model.ExchangeRateList = exchangeRateList;

        return View("ExchangeRateList", model);
    }

    private ICriteria GetListCriteria(ExchangeRateListQueryParam queryParam)
    {
        ICriteria criteria = this.session.CreateCriteria<ExchangeRateInfo>();
        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/exchange/rate/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        this.session.DefaultReadOnly = true;

        ExchangeRateInfo exchangeRate = null;

        if (id == null || id.Value == 0)
        {
            exchangeRate = new ExchangeRateInfo();
        }
        else
        {
            exchangeRate = this.session.Get<ExchangeRateInfo>(id.Value);
            if (exchangeRate == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        ExchangeRateDetailModel model = new ExchangeRateDetailModel();
        model.ExchangeRate = exchangeRate;

        return View("ExchangeRateDetail", model);
    }

    [HttpPost]
    [Route("/admin/exchange/rate/save")]
    [Transactional]
    public IActionResult Save(ExchangeRateDetailModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        ExchangeRateInfo exchangeRateFromUi = model.ExchangeRate;
        int exchangeRateId = exchangeRateFromUi.Id;

        bool isCreate = exchangeRateId == 0;

        ExchangeRateInfo exchangeRate = null;

        if (isCreate)
        {
            exchangeRate = new ExchangeRateInfo();
        }
        else
        {
            exchangeRate = this.session.Get<ExchangeRateInfo>(exchangeRateId);
        }

        // exchangeRate.FromCurrencyCode = exchangeRateFromUi.FromCurrencyCode;
        // exchangeRate.FromCurrencyName = exchangeRateFromUi.FromCurrencyName;
        // exchangeRate.ToCurrencyCode = exchangeRateFromUi.ToCurrencyCode;
        // exchangeRate.ToCurrencyName = exchangeRateFromUi.ToCurrencyName;

        exchangeRate.Rate = exchangeRateFromUi.Rate;
        exchangeRate.RateFix = exchangeRateFromUi.RateFix;

        if (isCreate)
        {
            this.session.Save(exchangeRate);
        }
        else
        {
            this.session.Update(exchangeRate);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/exchange/rate/delete")]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            ExchangeRateInfo exchangeRate = this.session.Load<ExchangeRateInfo>(id);
            this.session.Delete(exchangeRate);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/exchange/rate/refresh")]
    // trans in exchangeRateService.RefreshRate();
    public IActionResult Refresh()
    {
        exchangeRateService.RefreshRate(useInnerTransaction: true);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "更新成功！" },
        });
    }
}
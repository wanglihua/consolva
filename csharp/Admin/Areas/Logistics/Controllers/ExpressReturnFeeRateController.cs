using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Logistics.Models.ExpressReturnFeeRate;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.ExpressReturnFeeRate)]
public class ExpressReturnFeeRateController : Controller
{
    private ISession session;

    public ExpressReturnFeeRateController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/express/return/fee/rate")]
    public IActionResult Index()
    {
        ExpressReturnFeeRateIndexModel model = new ExpressReturnFeeRateIndexModel();

        ExpressReturnFeeRateInfo? expressReturnFeeRate = session.Query<ExpressReturnFeeRateInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        model.FeePerExpress = expressReturnFeeRate?.FeePerExpress ?? 0;

        return View("ExpressReturnFeeRateIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/return/fee/rate/save")]
    [Transactional]
    public IActionResult Save(ExpressReturnFeeRateIndexModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        ExpressReturnFeeRateInfo? expressReturnFeeRate = session.Query<ExpressReturnFeeRateInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (expressReturnFeeRate == null)
        {
            expressReturnFeeRate = new ExpressReturnFeeRateInfo();
        }

        expressReturnFeeRate.FeePerExpress = model.FeePerExpress;

        session.SaveOrUpdate(expressReturnFeeRate);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Logistics.Models.ExpressReturnFeeRate;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.ExpressReturnFeeRate)]
public class ExpressReturnFeeRateController : Controller {
    private readonly ISession session;
    private readonly ExpressReturnFeeRateService expressReturnFeeRateService;

    public ExpressReturnFeeRateController(
        ISession session,
        ExpressReturnFeeRateService expressReturnFeeRateService
    ) {
        this.session = session;
        this.expressReturnFeeRateService = expressReturnFeeRateService;
    }

    [HttpGet]
    [Route("/admin/express/return/fee/rate")]
    public IActionResult Index() {
        var model = new ExpressReturnFeeRateIndexModel();

        model.FeePerExpress = expressReturnFeeRateService.GetFeeRate();

        return View("ExpressReturnFeeRateIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/return/fee/rate/save")]
    [Transactional]
    public IActionResult Save(ExpressReturnFeeRateIndexModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        expressReturnFeeRateService.SaveFeeRate(model.FeePerExpress);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpress;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeList)]
public class LogisticsFeeRateExpressController : Controller {
    private readonly ISession session;

    public LogisticsFeeRateExpressController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/fee/rate/express/{logisticsModeId:int}")]
    public IActionResult Index(int logisticsModeId) {
        var logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);
        if (logisticsMode == null) {
            throw new ValidationException($"指定的物流方式#{logisticsModeId}不存在！");
        }

        var feeRate = session
            .Query<LogisticsFeeRateExpressInfo>()
            .Where(e => e.LogisticsMode.Id == logisticsModeId)
            .Take(1)
            .SingleOrDefault();

        var model = new LogisticsFeeRateExpressIndexModel();
        model.LogisticsModeId = logisticsModeId;
        model.LogisticsMode = logisticsMode;
        model.FeeRateExpress = feeRate;

        if (feeRate != null) {
            model.StartWeight = feeRate.StartWeight;
            model.StartPrice = feeRate.StartPrice;
            model.StepWeight = feeRate.StepWeight;
            model.StepPrice = feeRate.StepPrice;
            model.FeeByKilogram = feeRate.FeeByKilogram;
            model.StartFreeMinWeight = feeRate.StartFreeMinWeight;
            model.WeightBreakStart = feeRate.WeightBreakStart;
            model.WeightBreakDiscount = feeRate.WeightBreakDiscount;
        }

        return View("LogisticsFeeRateExpressIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/fee/rate/express/save")]
    [Transactional]
    public IActionResult Save(LogisticsFeeRateExpressSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        var modeId = model.LogisticsModeId;
        var logisticsMode = session.Get<LogisticsModeInfo>(modeId);
        if (logisticsMode == null) {
            throw new ValidationException($"指定的物流方式#{modeId}不存在！");
        }

        var feeRate = session
            .Query<LogisticsFeeRateExpressInfo>()
            .Where(e => e.LogisticsMode.Id == modeId)
            .Take(1)
            .SingleOrDefault();

        var isCreate = true;
        if (feeRate == null) {
            isCreate = true;
            feeRate = new LogisticsFeeRateExpressInfo();
            feeRate.LogisticsMode = logisticsMode;
        } else {
            isCreate = false;
        }

        feeRate.StartWeight = model.StartWeight;
        feeRate.StartPrice = model.StartPrice;
        feeRate.StepWeight = model.StepWeight;
        feeRate.StepPrice = model.StepPrice;
        feeRate.FeeByKilogram = model.FeeByKilogram ?? false;
        feeRate.StartFreeMinWeight = model.StartFreeMinWeight;
        feeRate.WeightBreakStart = model.WeightBreakStart;
        feeRate.WeightBreakDiscount = model.WeightBreakDiscount;

        if (isCreate) {
            session.Save(feeRate);
        } else {
            session.Update(feeRate);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "id", feeRate.Id }
        });
    }
}
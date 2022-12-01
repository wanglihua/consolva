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
public class LogisticsFeeRateExpressController : Controller
{
    private readonly ISession session;

    public LogisticsFeeRateExpressController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/fee/rate/express/{logisticsModeId:int}")]
    public IActionResult Index(int logisticsModeId)
    {
        var model = new LogisticsFeeRateExpressIndexModel();
        model.LogisticsModeId = logisticsModeId;
        model.LogisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);

        var logisticsFeeRateExpressInfo = session
            .Query<LogisticsFeeRateExpressInfo>()
            .Where(e => e.LogisticsModeId == logisticsModeId)
            .Take(1)
            .SingleOrDefault();

        if (logisticsFeeRateExpressInfo == null)
        {
            logisticsFeeRateExpressInfo = new LogisticsFeeRateExpressInfo();
            logisticsFeeRateExpressInfo.LogisticsModeId = logisticsModeId;
        }

        model.LogisticsFeeRateExpress = logisticsFeeRateExpressInfo;

        return View("LogisticsFeeRateExpressIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/fee/rate/express/save")]
    [Transactional]
    public IActionResult Save(LogisticsFeeRateExpressSaveModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var logisticsFeeRateExpressFromUi = model.LogisticsFeeRateExpress;
        var logisticsFeeRateExpressId = logisticsFeeRateExpressFromUi.Id;


        var logisticsModeId = model.LogisticsFeeRateExpress.LogisticsModeId;
        var logisticsFeeRateExpress = session
            .Query<LogisticsFeeRateExpressInfo>()
            .Where(e => e.LogisticsModeId == logisticsModeId)
            .Take(1)
            .SingleOrDefault();

        var isCreate = true;
        if (logisticsFeeRateExpress == null)
        {
            isCreate = true;
            logisticsFeeRateExpress = new LogisticsFeeRateExpressInfo();
        }
        else
        {
            isCreate = false;

            // 删除掉其他的可能的这个物流方式的多余的费率，防止不合法的多余数据的出现
            session.Query<LogisticsFeeRateExpressInfo>().Where(e => e.LogisticsModeId == logisticsModeId && e.Id != logisticsFeeRateExpressId).Delete();

            if (logisticsFeeRateExpress.LogisticsModeId != logisticsFeeRateExpressFromUi.LogisticsModeId) throw new ValidationException("指定的物流方式Id和当前费率不匹配！");
        }

        logisticsFeeRateExpress.LogisticsModeId = logisticsFeeRateExpressFromUi.LogisticsModeId;
        logisticsFeeRateExpress.StartWeight = logisticsFeeRateExpressFromUi.StartWeight;
        logisticsFeeRateExpress.StartPrice = logisticsFeeRateExpressFromUi.StartPrice;
        logisticsFeeRateExpress.StepWeight = logisticsFeeRateExpressFromUi.StepWeight;
        logisticsFeeRateExpress.StepPrice = logisticsFeeRateExpressFromUi.StepPrice;
        logisticsFeeRateExpress.StartFreeMinWeight = logisticsFeeRateExpressFromUi.StartFreeMinWeight;
        logisticsFeeRateExpress.WeightBreakStart = logisticsFeeRateExpressFromUi.WeightBreakStart;
        logisticsFeeRateExpress.WeightBreakDiscount = logisticsFeeRateExpressFromUi.WeightBreakDiscount;

        if (isCreate)
            session.Save(logisticsFeeRateExpress);
        else
            session.Update(logisticsFeeRateExpress);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "id", logisticsFeeRateExpress.Id }
        });
    }
}
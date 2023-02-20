using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderPackConfig;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.OrderPackConfig)]
public class LogisticsOrderPackConfigController : Controller {
    private readonly ISession session;

    public LogisticsOrderPackConfigController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/pack/config")]
    public IActionResult Index() {
        session.DefaultReadOnly = true;

        var config = session
            .Query<LogisticsOrderPackConfigInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (config == null) {
            config = new LogisticsOrderPackConfigInfo();
        }

        LogisticsOrderPackConfigIndexModel model = new LogisticsOrderPackConfigIndexModel();
        model.SortTypeList = LogisticsOrderPackSortType.GetList();
        model.DisplayTypeList = LogisticsOrderPackDisplayType.GetList();

        model.SortType = config.SortType;
        model.DisplayType = config.DisplayType;

        model.PackDelayMinutes = config.PackDelayMinutes;

        model.Label1015Enabled = config.Label1015Enabled;
        model.Label1015PrintCount = config.Label1015PrintCount;

        model.Label1005Enabled = config.Label1005Enabled;
        model.Label1005PrintCount = config.Label1005PrintCount;

        model.Label0505Enabled = config.Label0505Enabled;
        model.Label0505PrintCount = config.Label0505PrintCount;

        model.TrackingNoManualInputEnabled = config.TrackingNoManualInputEnabled;

        return View("LogisticsOrderPackConfigIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pack/config/save")]
    [Transactional]
    public IActionResult Save(LogisticsOrderPackConfigIndexModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        var config = session
            .Query<LogisticsOrderPackConfigInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        bool isCreate = false;
        if (config == null) {
            isCreate = true;
            config = new LogisticsOrderPackConfigInfo();
        }

        config.SortType = model.SortType;
        config.DisplayType = model.DisplayType;

        config.PackDelayMinutes = model.PackDelayMinutes;

        config.Label1015Enabled = model.Label1015Enabled;
        config.Label1015PrintCount = model.Label1015PrintCount;

        config.Label1005Enabled = model.Label1005Enabled;
        config.Label1005PrintCount = model.Label1005PrintCount;

        config.Label0505Enabled = model.Label0505Enabled;
        config.Label0505PrintCount = model.Label0505PrintCount;

        config.TrackingNoManualInputEnabled = model.TrackingNoManualInputEnabled;

        if (isCreate) {
            session.Save(config);
        } else {
            session.Update(config);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "订单打包配置 保存成功！" },
        });
    }
}
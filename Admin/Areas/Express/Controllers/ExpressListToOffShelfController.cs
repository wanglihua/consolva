using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Services;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.Pass;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressListToOffShelfController : Controller {
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly LoginUserService loginUserService;
    private readonly ExpressExcelService expressExcelService;
    private readonly ExpressService expressService;
    private readonly WarehouseService warehouseService;
    private readonly PassResService passResService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressListToOffShelfController(ISession session,
        ExpressListService expressListService,
        LoginUserService loginUserService,
        ExpressExcelService expressExcelService,
        ExpressService expressService,
        WarehouseService warehouseService,
        PassResService passResService,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.expressListService = expressListService;
        this.loginUserService = loginUserService;
        this.expressExcelService = expressExcelService;
        this.expressService = expressService;
        this.warehouseService = warehouseService;
        this.passResService = passResService;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    [HttpGet]
    [Route("/admin/express/to/off/shelf")]
    public IActionResult Index() {
        return View("");
    }

    [HttpPost]
    [Route("/admin/express/to/off/shelf/list")]
    public IActionResult List() {
        return View("");
    }
}
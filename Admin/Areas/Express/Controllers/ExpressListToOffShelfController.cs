using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Express.Models.ExpressListToOffShelf;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.Logistics;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressListToOffShelfController : Controller {
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly LoginUserService loginUserService;
    private readonly WarehouseService warehouseService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressListToOffShelfController(ISession session,
        ExpressListService expressListService,
        LoginUserService loginUserService,
        WarehouseService warehouseService,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.expressListService = expressListService;
        this.loginUserService = loginUserService;
        this.warehouseService = warehouseService;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    [HttpGet]
    [Route("/admin/express/to/off/shelf")]
    public IActionResult Index() {
        var model = new ExpressListToOffShelfIndexModel();
        model.WarehouseList = warehouseService.GetWarehouseListWithCacheForAdmin();

        return View("ExpressListToOffShelfIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/to/off/shelf/list")]
    public IActionResult List(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.ToOffShelf, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.ToOffShelf, queryParam, listPager);

        var model = new ExpressListToOffShelfListModel(listPager) {
            ExpressList = expressList,
            VolumeWeightDenominator = logisticsModeSettingService.GetSetting().VolumeWeightDenominator
        };

        return View("ExpressListToOffShelfList", model);
    }
}
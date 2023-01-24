using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Admin.Services;
using shunda.Service;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly SidebarStateService sidebarStateService;
    private readonly WarehouseService warehouseService;

    public SidebarViewComponent(
        ISession session,
        LoginUserService loginUserService,
        SidebarStateService sidebarStateService,
        WarehouseService warehouseService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.sidebarStateService = sidebarStateService;
        this.warehouseService = warehouseService;
    }

    public IViewComponentResult Invoke() {
        var isAdmin = loginUserService.IsAdmin;
        var loginUserId = loginUserService.GetLoginUserId();

        var model = new SidebarModel();
        model.IsAdmin = isAdmin;
        model.PassResIdMap = loginUserService.GetPassResIdMap();
        model.IsSidebarMin = sidebarStateService.GetMinState(loginUserId);
        model.WarehouseList = warehouseService.GetWarehouseListWithCache();

        return View("Sidebar", model);
    }
}
using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Admin.Services;
using shunda.Service;
using shunda.Service.Pass;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly SidebarStateService sidebarStateService;
    private readonly WarehouseService warehouseService;
    private readonly PassResService passResService;

    public SidebarViewComponent(
        ISession session,
        LoginUserService loginUserService,
        SidebarStateService sidebarStateService,
        WarehouseService warehouseService,
        PassResService passResService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.sidebarStateService = sidebarStateService;
        this.warehouseService = warehouseService;
        this.passResService = passResService;
    }

    public IViewComponentResult Invoke() {
        var isAdmin = loginUserService.IsAdmin;
        var loginUserId = loginUserService.GetLoginUserId();

        var model = new SidebarModel {
            IsAdmin = isAdmin,
            PassResIdMap = passResService.GetPassResIdMap(loginUserId),
            IsSidebarMin = sidebarStateService.GetMinState(loginUserId),
            WarehouseList = warehouseService.GetWarehouseListWithCacheForAdmin()
        };

        return View("Sidebar", model);
    }
}
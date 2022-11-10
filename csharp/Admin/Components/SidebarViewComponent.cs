using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Admin.Services;
using shunda.Service;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent
{
    private readonly LoginUserService loginUserService;
    private readonly SidebarStateService sidebarStateService;

    public SidebarViewComponent(
        LoginUserService loginUserService,
        SidebarStateService sidebarStateService)
    {
        this.loginUserService = loginUserService;
        this.sidebarStateService = sidebarStateService;
    }

    public IViewComponentResult Invoke()
    {
        var isAdmin = loginUserService.IsAdmin;
        var loginUserId = loginUserService.GetLoginUserId();

        var model = new SidebarModel();
        model.IsAdmin = isAdmin;
        model.PassResIdMap = loginUserService.GetPassResIdMap();


        model.IsSidebarMin = sidebarStateService.GetMinState(loginUserId);

        return View("Sidebar", model);
    }
}
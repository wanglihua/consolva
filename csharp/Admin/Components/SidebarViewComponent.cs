using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Admin.Services;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent
{
    private readonly LoginUserService loginUserService;
    private  readonly ISession session;
    private readonly SidebarStateService sidebarStateService;

    public SidebarViewComponent(
        ISession session,
        LoginUserService loginUserService,
        SidebarStateService sidebarStateService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
        this.sidebarStateService = sidebarStateService;
    }

    public IViewComponentResult Invoke()
    {
        // int loginUserId = loginUserService.GetLoginUserId();
        var isAdmin = loginUserService.IsAdmin;
        var loginUserId = loginUserService.GetLoginUserId();

        var model = new SidebarModel();
        model.IsAdmin = isAdmin;
        model.PassResIdMap = loginUserService.GetPassResIdMap();


        model.IsSidebarMin = sidebarStateService.GetMinState(loginUserId);

        return View("Sidebar", model);
    }
}
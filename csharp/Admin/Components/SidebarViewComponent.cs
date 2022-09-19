using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent
{
    private ISession session;
    private LoginUserService loginUserService;
    private SidebarStateService sidebarStateService;

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
        bool isAdmin = loginUserService.IsAdmin;

        SidebarModel model = new SidebarModel();
        model.IsAdmin = isAdmin;
        model.PassResIdMap = loginUserService.GetPassResIdMap();


        model.IsSidebarMin = sidebarStateService.GetMinState();

        return View("Sidebar", model);
    }
}
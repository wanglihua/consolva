using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.Sidebar;
using shunda.Service;

namespace shunda.Admin.Components;

public class SidebarViewComponent : ViewComponent
{
    private SidebarStateService sidebarStateService;

    public SidebarViewComponent(SidebarStateService sidebarStateService)
    {
        this.sidebarStateService = sidebarStateService;
    }

    public IViewComponentResult Invoke()
    {
        SidebarModel model = new SidebarModel();
        model.IsSidebarMin = sidebarStateService.GetMinState();

        return View("Sidebar", model);
    }
}
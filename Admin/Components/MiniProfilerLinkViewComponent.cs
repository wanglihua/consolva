using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Models.MiniProfilerLink;

namespace shunda.Admin.Components;

public class MiniProfilerLinkViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new MiniProfilerLinkModel();
        return View("MiniProfilerLink", model);
    }
}
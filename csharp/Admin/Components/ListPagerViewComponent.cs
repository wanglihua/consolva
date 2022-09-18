using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Admin.Components;

public class ListPagerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ListModel model, string loadListFunc)
    {
        ViewBag.LoadListFunc = loadListFunc;
        return View("ListPager", model);
    }
}
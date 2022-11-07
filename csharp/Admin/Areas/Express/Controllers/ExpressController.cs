using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressController : Controller
{
    [HttpGet]
    [Route("/admin/express")]
    public IActionResult Index()
    {
        return Content("express index");
    }

    [HttpGet]
    [Route("/admin/express/{id:int}")]
    public IActionResult Detail(int id)
    {
        return Content("express detail " + id);
    }
}
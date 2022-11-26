using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.Web;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class LodopScriptController : Controller
{
    [HttpGet]
    [Route("/admin/lodop/script")]
    public IActionResult Index()
    {
        // LODOP.SET_PRINTER_INDEX(-1); 尝试使用默认打印机

        var script = @"
function getLodopWithLicenses() {
    var LODOP = getLodop();
    LODOP.SET_LICENSES('浙江雅莹服装有限公司', '853717765858688748719056235623', '', '');
    LODOP.SET_PRINTER_INDEX(-1);
    return LODOP;
}
";

        return Content(script, ContentType.JavaScript);
    }
}
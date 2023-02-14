using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

public class HomeController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ClientHomeDataService clientHomeDataService;

    public HomeController(ISession session,
        LoginCustomerService loginCustomerService,
        ClientHomeDataService clientHomeDataService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.clientHomeDataService = clientHomeDataService;
    }

    // 这个只能通过dotnet 部署的端口号访问到
    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index() {
        var model = new HomeIndexModel();
        return View("HomeIndex", model);
    }

    // 这个可以通过nginx反向代理访问到
    [HttpGet]
    [Route("/client")]
    public IActionResult ClientIndex() {
        var model = new HomeClientIndexModel();
        return View("HomeClientIndex", model);
    }

    [HttpPost]
    [Route("/client/home/data")]
    public IActionResult ClientHomeData() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var clientHomeData = clientHomeDataService.GetCustomerClientHomeData(loginCustomerId);
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = clientHomeData,
        });
    }
}
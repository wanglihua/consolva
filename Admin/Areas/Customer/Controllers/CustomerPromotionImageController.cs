using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

/// <summary>
/// 客户推广图片 控制器
/// </summary>
[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerPromotionImage)]
public class CustomerPromotionImageController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionImageController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/customer/promotion/image")]
    public IActionResult Index() {
        CustomerPromotionImageIndexModel model = new CustomerPromotionImageIndexModel();

        return View("CustomerPromotionImageIndex", model);
    }
}
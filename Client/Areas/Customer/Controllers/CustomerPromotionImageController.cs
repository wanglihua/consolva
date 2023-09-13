using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[NoLoginRequired]
[CustomerArea]
public class CustomerPromotionImageController : Controller {
    private readonly ISession session;
    private readonly CustomerPromotionImageService customerPromotionImageService;

    public CustomerPromotionImageController(
        ISession session,
        CustomerPromotionImageService customerPromotionImageService
    ) {
        this.session = session;
        this.customerPromotionImageService = customerPromotionImageService;
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/{customerId:int}/{imageNum:int}")]
    // [Transactional]
    public IActionResult Index(int customerId, int imageNum) {
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByScale(customerId, imageNum, 1.0m);
        return new FileContentResult(bytes, "image/jpeg");
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/small/scale/{customerId:int}/{imageNum:int}")]
    // [Transactional]
    public IActionResult SmallByScale(int customerId, int imageNum) {
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByScale(customerId, imageNum, 0.1m);
        return new FileContentResult(bytes, "image/jpeg");
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/small/height/{customerId:int}/{imageNum:int}")]
    // [Transactional]
    public IActionResult SmallByHeight(int customerId, int imageNum) {
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByHeight(customerId, imageNum, 200);
        return new FileContentResult(bytes, "image/jpeg");
    }
}
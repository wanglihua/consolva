using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerPromotionImageController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerPromotionImageService customerPromotionImageService;

    public CustomerPromotionImageController(
        ISession session,
        LoginCustomerService loginCustomerService,
        CustomerPromotionImageService customerPromotionImageService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerPromotionImageService = customerPromotionImageService;
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/{imageNum:int}")]
    // [Transactional]
    public IActionResult Index(int imageNum) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByScale(loginCustomerId, imageNum, 1.0m);
        return new FileContentResult(bytes, "image/jpeg");
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/small/scale/{imageNum:int}")]
    // [Transactional]
    public IActionResult SmallByScale(int imageNum) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByScale(loginCustomerId, imageNum, 0.1m);
        return new FileContentResult(bytes, "image/jpeg");
    }

    [HttpGet]
    [Route("/client/customer/promotion/image/small/height/{imageNum:int}")]
    // [Transactional]
    public IActionResult SmallByHeight(int customerId, int imageNum) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        byte[] bytes = customerPromotionImageService.GetPromotionImageContentByHeight(loginCustomerId, imageNum, 200);
        return new FileContentResult(bytes, "image/jpeg");
    }
}
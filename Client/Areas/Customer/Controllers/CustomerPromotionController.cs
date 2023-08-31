using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Services;
using shunda.Domain.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerPromotionController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatSceneService wechatSceneService;

    public CustomerPromotionController(
        ISession session,
        LoginCustomerService loginCustomerService,
        WechatSceneService wechatSceneService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatSceneService = wechatSceneService;
    }

    [HttpPost]
    [Route("/mine/customer/promotion")]
    //[Transactional]
    public IActionResult CustomerPromotion() {
        int customerId = loginCustomerService.GetLoginCustomerId();

        // 在这里把微信场景确认创建好，后面显示图片的时候，调用生成 scene id 时，产生并发问题的可能性就大大减少了
        wechatSceneService.CreateOrGetCustomerPromotionWechatSceneId(customerId); // 使用内部事务

        // string qrCodeUrl = weiXinSceneService.CreateOrGetCustomerPromotionScene(customerId);
        string qrCodeUrl = wechatSceneService.CreateOrGetCustomerPromotionSceneImageUrl(customerId);

        IList<CustomerPromotionImageInfo> customerPromotionImageListInDb = session.Query<CustomerPromotionImageInfo>()
            .OrderBy(e => e.ImageNum)
            .ToList();

        IList<Dictionary<string, object>> customerPromotionImageList = new List<Dictionary<string, object>>();

        for (int num = 1; num <= 8; num++) {
            var customerPromotionImage = customerPromotionImageListInDb
                .Where(e => e.ImageNum == num)
                .Take(1)
                .SingleOrDefault();

            Dictionary<string, object> promotionImageItem = new Dictionary<string, object>();

            if (customerPromotionImage == null) {
                promotionImageItem["imageNum"] = num;
                promotionImageItem["imageUrl"] = "/static/images/noimg.jpg";
                promotionImageItem["smallImageUrl"] = "/static/images/noimg.jpg";
            } else {
                string imageUrl = Url.Action("Index", "PromotionImage", new { customerId = customerId, imageNum = customerPromotionImage.ImageNum }) ?? string.Empty;
                string smallImageUrl = Url.Action("SmallByHeight", "PromotionImage", new { customerId = customerId, imageNum = customerPromotionImage.ImageNum }) ?? string.Empty;

                promotionImageItem["imageNum"] = num;
                promotionImageItem["imageUrl"] = imageUrl;
                promotionImageItem["smallImageUrl"] = smallImageUrl;
            }

            customerPromotionImageList.Add(promotionImageItem);

            if (num == 4) {
                // 加一个中间二维码位置的图片
                customerPromotionImageList.Add(new Dictionary<string, object>() {
                    { "imageNum", -1 },
                    { "imageUrl", qrCodeUrl },
                    { "smallImageUrl", qrCodeUrl },
                });
            }
        }

        var customerPromotionImageSetting = session.Query<CustomerPromotionImageSettingInfo>().Take(1).SingleOrDefault();
        string promotionImageDesc = customerPromotionImageSetting?.ImageDesc ?? string.Empty;

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取客户推广图片数据成功！" },
            { "promotionImageDesc", promotionImageDesc },
            { "customerPromotionImageList", customerPromotionImageList },
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Customer.Models.CustomerPromotion;
using shunda.Client.Services;
using shunda.Core.Config;
using shunda.Core.Web;
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
    [Route("/client/customer/promotion")]
    //[Transactional]
    public IActionResult CustomerPromotion() {
        // todo 检查是否关注过公众号

#if DEBUG
        var promotionImageList = new List<CustomerPromotionImageItem>();

        for (int num = 1; num <= 9; num++) {
            promotionImageList.Add(new CustomerPromotionImageItem {
                ImageNum = num,
                ImageUrl = SiteConfig.SiteUrlBase + "/static/client/images/noimg.jpg",
                SmallImageUrl = SiteConfig.SiteUrlBase + "/static/client/images/noimg.jpg",
            });
        }
#else
        int customerId = loginCustomerService.GetLoginCustomerId();

        // 在这里把微信场景确认创建好，后面显示图片的时候，调用生成 scene id 时，产生并发问题的可能性就大大减少了
        wechatSceneService.CreateOrGetCustomerPromotionWechatSceneId(customerId); // 使用内部事务

        // string qrCodeUrl = weiXinSceneService.CreateOrGetCustomerPromotionScene(customerId);
        string qrCodeUrl = wechatSceneService.CreateOrGetCustomerPromotionSceneImageUrl(customerId);

        IList<CustomerPromotionImageInfo> customerPromotionImageListInDb = session.Query<CustomerPromotionImageInfo>()
            .OrderBy(e => e.ImageNum)
            .ToList();

        var promotionImageList = new List<CustomerPromotionImageItem>();

        for (int num = 1; num <= 8; num++) {
            var customerPromotionImage = customerPromotionImageListInDb
                .Where(e => e.ImageNum == num)
                .Take(1)
                .SingleOrDefault();

            var promotionImageItem = new CustomerPromotionImageItem();

            if (customerPromotionImage == null) {
                promotionImageItem.ImageNum = num;
                promotionImageItem.ImageUrl = SiteConfig.SiteUrlBase + "/static/client/images/noimg.jpg";
                promotionImageItem.SmallImageUrl = SiteConfig.SiteUrlBase + "/static/client/images/noimg.jpg";
            } else {
                string imageUrl = Url.Action("Index", "CustomerPromotionImage", new { imageNum = customerPromotionImage.ImageNum }) ?? string.Empty;
                string smallImageUrl = Url.Action("SmallByHeight", "CustomerPromotionImage", new { imageNum = customerPromotionImage.ImageNum }) ?? string.Empty;

                promotionImageItem.ImageNum = num;
                promotionImageItem.ImageUrl = SiteConfig.SiteUrlBase + imageUrl;
                promotionImageItem.SmallImageUrl = SiteConfig.SiteUrlBase + smallImageUrl;
            }

            promotionImageList.Add(promotionImageItem);

            if (num == 4) {
                // 加一个中间二维码位置的图片
                promotionImageList.Add(new CustomerPromotionImageItem {
                    ImageNum = -1,
                    ImageUrl = qrCodeUrl,
                    SmallImageUrl = qrCodeUrl,
                });
            }
        }
#endif
        var customerPromotionImageSetting = session.Query<CustomerPromotionImageSettingInfo>().Take(1).SingleOrDefault();
        string promotionImageDesc = customerPromotionImageSetting?.ImageDesc ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "获取客户推广图片数据成功！",
            Data = new CustomerPromotionModel {
                PromotionImageDesc = promotionImageDesc,
                PromotionImageList = promotionImageList,
            },
        });
    }
}
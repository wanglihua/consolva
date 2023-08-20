using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
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
    private readonly CustomerPromotionImageService customerPromotionImageService;

    public CustomerPromotionImageController(
        ISession session,
        CustomerPromotionImageService customerPromotionImageService
    ) {
        this.session = session;
        this.customerPromotionImageService = customerPromotionImageService;
    }

    [HttpGet]
    [Route("/admin/customer/customer/promotion/image")]
    public IActionResult Index() {
        CustomerPromotionImageIndexModel model = new CustomerPromotionImageIndexModel();

        var customerPromotionImageListInDb = session.Query<CustomerPromotionImageInfo>()
            .OrderBy(e => e.ImageNum)
            .ToList();

        var customerPromotionImageList = new List<CustomerPromotionImageInfo>();

        for (int num = 1; num <= 8; num++) {
            var customerPromotionImage = customerPromotionImageListInDb
                .Where(e => e.ImageNum == num)
                .Take(1)
                .SingleOrDefault();

            if (customerPromotionImage == null) {
                customerPromotionImage = new CustomerPromotionImageInfo();
                customerPromotionImage.ImageNum = num;

                customerPromotionImage.ImageFileUrlCache = "/static/images/noimg.jpg";
            } else {
                customerPromotionImage.ImageFileUrlCache = customerPromotionImageService.GetFullImageUrl(customerPromotionImage.ImageFileName);
            }

            customerPromotionImageList.Add(customerPromotionImage);

            if (num == 4) {
                // 加一个中间二维码位置的图片
                customerPromotionImageList.Add(new CustomerPromotionImageInfo { ImageNum = -1 });
            }
        }

        model.CustomerPromotionImageList = customerPromotionImageList;

        var customerPromotionImageSetting = session.Query<CustomerPromotionImageSettingInfo>().Take(1).SingleOrDefault();
        if (customerPromotionImageSetting != null) {
            model.PromotionClientShow = customerPromotionImageSetting.ClientShow;
            model.PromotionImageDesc = customerPromotionImageSetting.ImageDesc;
        }

        return View("CustomerPromotionImageIndex", model);
    }


    [HttpPost]
    [Route("/admin/write/customer/promotion/image/upload")]
    // [Transactional] // 内部有事务
    public IActionResult UploadCustomerPromotionImage(IFormFile imageFile, int imageNum) {
        string fileExt = customerPromotionImageService.GetFileExt(imageFile.FileName);
        if (fileExt.ToLower() != ".png".ToLower() && fileExt.ToLower() != ".jpg".ToLower()) {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", "只支持 png 和 jpg 格式的图片文件！" },
            });
        }

        if (imageNum < 1 || imageNum > 8) {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", "图片序号应为 1-8 的数字！" },
            });
        }

        string imageFileName = customerPromotionImageService.SaveImageFile(imageFile);
        string oldImageFileName = string.Empty;

        Transactional.Wrap(session, () => {
            var customerPromotionImage = session.Query<CustomerPromotionImageInfo>()
                .Where(e => e.ImageNum == imageNum)
                .Take(1)
                .SingleOrDefault();

            if (customerPromotionImage == null) {
                customerPromotionImage = new CustomerPromotionImageInfo {
                    ImageNum = imageNum
                };
            } else {
                oldImageFileName = customerPromotionImage.ImageFileName;
            }

            customerPromotionImage.ImageFileName = imageFileName;

            session.SaveOrUpdate(customerPromotionImage);
            session.Flush();
        });

        if (string.IsNullOrWhiteSpace(oldImageFileName) == false) {
            customerPromotionImageService.DeleteImageFile(oldImageFileName);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "上传成功！" },
            { "imageFileUrl", "/" + CustomerPromotionImageService.ImageUploadUrl + "/" + imageFileName },
        });
    }

    [HttpPost]
    [Route("/admin/write/customer/promotion/setting/save")]
    // [Transactional] // 内部有事务
    public IActionResult SettingSave(CustomerPromotionImageSettingModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        Transactional.Wrap(session, () => {
            var customerPromotionImageSetting = session.Query<CustomerPromotionImageSettingInfo>()
                .Take(1)
                .SingleOrDefault();
            if (customerPromotionImageSetting == null) {
                customerPromotionImageSetting = new CustomerPromotionImageSettingInfo();
            }

            customerPromotionImageSetting.ClientShow = model.PromotionClientShow;
            customerPromotionImageSetting.ImageDesc = model.PromotionImageDesc;

            session.SaveOrUpdate(customerPromotionImageSetting);
            session.Flush();
        });

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }
}
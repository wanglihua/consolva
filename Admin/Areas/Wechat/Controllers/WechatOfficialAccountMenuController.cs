using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using shunda.Admin.Areas.Wechat.Models.WechatOfficialAccountMenu;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Wechat.Controllers;

[AdminAuth(AdminOnly = true)]
[WechatArea]
public class WechatOfficialAccountMenuController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService;

    public WechatOfficialAccountMenuController(ISession session,
        WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService
    ) {
        this.session = session;
        this.wechatOfficialAccountAccessTokenService = wechatOfficialAccountAccessTokenService;
    }

    [HttpGet]
    [Route("/admin/wechat/official/account/menu")]
    public IActionResult Index() {
        var model = new WechatOfficialAccountMenuIndexModel();
        return View("WechatOfficialAccountMenuIndex", model);
    }

    [HttpPost]
    [Route("/admin/wechat/official/account/menu/create")]
    public IActionResult Create() {
        ButtonGroup menu = new ButtonGroup();

        #region menu1

        // menu1

        var mainMenu1 = new SubButton() { name = "转运管理" };

        // mainMenu1.sub_button.Add(new SingleMiniProgramButton {
        //     name = "仓库地址",
        //     appid = WechatAppConfig.AppId,
        //     pagepath = "pages/instruction/instruction_warehouse_address",
        // });
        //
        // mainMenu1.sub_button.Add(new SingleMiniProgramButton {
        //     name = "包裹预报",
        //     appid = WechatAppConfig.AppId,
        //     pagepath = "pages/express/express_expect/express_expect_list",
        // });

        mainMenu1.sub_button.Add(new SingleViewButton {
            name = "活动福利",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/promotion",
        });

        mainMenu1.sub_button.Add(new SingleViewButton {
            name = "PayPal支付",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/paypal",
        });

        mainMenu1.sub_button.Add(new SingleViewButton {
            name = "支付宝支付",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/alipay",
        });

        menu.button.Add(mainMenu1);

        #endregion

        #region menu2

        // menu2

        // var mainMenu2 = new SubButton("开始集运");
        //
        // mainMenu2.sub_button.Add(new SingleMiniProgramButton() {
        //     name = "开始集运",
        //     appid = WechatAppConfig.AppId,
        //     pagepath = "pages/home/home_index",
        // });
        //
        // menu.button.Add(mainMenu2);

        #endregion

        #region menu3

        // menu3

        var mainMenu3 = new SubButton() { name = "转运须知" };

        mainMenu3.sub_button.Add(new SingleViewButton {
            name = "物品分类",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/goods/type",
        });

        mainMenu3.sub_button.Add(new SingleViewButton {
            name = "使用协议",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/use/agreement",
        });

        mainMenu3.sub_button.Add(new SingleViewButton {
            name = "理赔条款",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/insurance/clause",
        });

        mainMenu3.sub_button.Add(new SingleViewButton {
            name = "积分说明",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/score/remark",
        });

        mainMenu3.sub_button.Add(new SingleViewButton {
            name = "收费标准",
            url = SiteConfig.SiteUrlBase + "/client/wechat/official/account/article/freight/desc",
        });

        menu.button.Add(mainMenu3);

        #endregion

        // create
        var accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();
        var result = CommonApi.CreateMenu(accessToken, menu);

        log.Error($"create menu result:  errcode: {result.errcode}, ErrorCodeValue:{result.ErrorCodeValue}, errmsg:{result.errmsg}");

        return Json(new JsonResultData {
            Success = true,
            Message = $"微信公众号菜单创建 result:  errcode: {result.errcode}, ErrorCodeValue:{result.ErrorCodeValue}, errmsg:{result.errmsg}！",
        });
    }

    [HttpPost]
    [Route("/admin/wechat/official/account/menu/delete")]
    public IActionResult Delete() {
        var accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();
        var result = CommonApi.DeleteMenu(accessToken);

        return Json(new JsonResultData {
            Success = true,
            Message = $"微信公众号菜单删除 result:  errcode: {result.errcode}, ErrorCodeValue:{result.ErrorCodeValue}, errmsg:{result.errmsg}！",
        });
    }
}
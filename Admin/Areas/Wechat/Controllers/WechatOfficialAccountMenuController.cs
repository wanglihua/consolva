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
    private static Logger log = LogManager.GetCurrentClassLogger();
    
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
    [Route("/admin/wei/xin/create/menu/1")]
    public IActionResult UploadMenu() {
        ButtonGroup menu = new ButtonGroup();

        // menu1
        var mainMenu1 = new SubButton() { name = "转运管理" };

        // mainMenu1.sub_button.Add(new SingleViewButton {
        //     name = "下单流程",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/order/process")
        // });
        //
        // mainMenu1.sub_button.Add(new SingleViewButton {
        //     name = "快递预报",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/express/expect/list")
        // });
        //
        // mainMenu1.sub_button.Add(new SingleViewButton {
        //     name = "我要认领",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/express/claim/list")
        // });
        //
        // mainMenu1.sub_button.Add(new SingleViewButton {
        //     name = "提交转运",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/express/checkin/list")
        // });

        mainMenu1.sub_button.Add(new SingleViewButton {
            name = "官网首页",
            // url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile")
        });

        menu.button.Add(mainMenu1);

        // // menu2
        // var mainMenu2 = new SubButton() { name = "费用相关" };
        //
        // mainMenu2.sub_button.Add(new SingleViewButton {
        //     name = "收费标准",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/freight/desc")
        // });
        //
        // mainMenu2.sub_button.Add(new SingleViewButton {
        //     name = "优惠来袭",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/promotion")
        // });
        //
        // mainMenu2.sub_button.Add(new SingleViewButton {
        //     name = "预估运费",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/freight/estimate")
        // });
        //
        // mainMenu2.sub_button.Add(new SingleViewButton {
        //     name = "积分查询",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/score")
        // });
        //
        // mainMenu2.sub_button.Add(new SingleViewButton {
        //     name = "支付明细",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/pay/history")
        // });
        //
        // menu.button.Add(mainMenu2);

        // // menu3
        // var mainMenu3 = new SubButton() { name = "转运须知" };
        //
        // mainMenu3.sub_button.Add(new SingleViewButton {
        //     name = "澳拼仓库",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/warehouse/address")
        // });
        //
        // mainMenu3.sub_button.Add(new SingleViewButton {
        //     name = "产品分类",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/goods/list")
        // });
        //
        // mainMenu3.sub_button.Add(new SingleViewButton {
        //     name = "使用协议",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/use/agreement")
        // });
        //
        // mainMenu3.sub_button.Add(new SingleViewButton {
        //     name = "运输时效",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/transport/limit")
        // });
        //
        // mainMenu3.sub_button.Add(new SingleViewButton {
        //     name = "保险条款",
        //     url = GetWeiXinAccount1AutoLoginUrl(SiteConfig.SiteRoot + "/mobile/insurance/clause")
        // });
        //
        // menu.button.Add(mainMenu3);

        // create
        var accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();
        var result = CommonApi.CreateMenu(accessToken, menu);

        log.Error($"create menu result:  errcode: {result.errcode}, ErrorCodeValue:{result.ErrorCodeValue}, errmsg:{result.errmsg}" );

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", $"微信公众号菜单上传 result:  errcode: {result.errcode}, ErrorCodeValue:{result.ErrorCodeValue}, errmsg:{result.errmsg}！"},
        });
    }
}
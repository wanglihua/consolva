using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

/// <summary>
/// 用于查询获得记录的微信公众号文章URL并重定向
/// </summary>
[WechatOfficialAccountArea]
[NoLoginRequired]
public class WechatOfficialAccountArticleViewController : Controller {
    private readonly ISession session;

    public WechatOfficialAccountArticleViewController(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 活动福利
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/promotion")]
    public IActionResult Promotion() {
        return Redirect(GetArticleUrl(ClientInstructionType.Promotion));
    }

    /// <summary>
    /// 收费标准
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/freight/desc")]
    public IActionResult FreightDesc() {
        return Redirect(GetArticleUrl(ClientInstructionType.FreightDesc));
    }    
    
    /// <summary>
    /// 转运流程
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/order/simple/step")]
    public IActionResult OrderSimpleStep() {
        return Redirect(GetArticleUrl(ClientInstructionType.OrderSimpleStep));
    }       

    /// <summary>
    /// 物品分类
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/goods/type")]
    public IActionResult GoodsType() {
        return Redirect(GetArticleUrl(ClientInstructionType.GoodsType));
    }

    /// <summary>
    /// 使用协议
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/use/agreement")]
    public IActionResult UseAgreement() {
        return Redirect(GetArticleUrl(ClientInstructionType.UseAgreement));
    }

    /// <summary>
    /// 理赔条款
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/insurance/clause")]
    public IActionResult InsuranceClause() {
        return Redirect(GetArticleUrl(ClientInstructionType.InsuranceClause));
    }

    /// <summary>
    /// 积分说明
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/score/remark")]
    public IActionResult ScoreRemark() {
        return Redirect(GetArticleUrl(ClientInstructionType.ScoreRemark));
    }

    /// <summary>
    /// 常见问题
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/article/faq")]
    public IActionResult Faq() {
        return Redirect(GetArticleUrl(ClientInstructionType.Faq));
    }    

    [NonAction]
    private string GetArticleUrl(ClientInstructionType clientInstructionType) {
        var clientInstruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == clientInstructionType)
            .Take(1)
            .SingleOrDefault();
        if (clientInstruction != null && string.IsNullOrWhiteSpace(clientInstruction.Url) == false) {
            return clientInstruction.Url;
        }

        return "https://mp.weixin.qq.com/s/LP44u5HDzJKo4C8z8sgujw"; // 这是客服二维码的文章，暂时拿这个做为后备吧
    }
}
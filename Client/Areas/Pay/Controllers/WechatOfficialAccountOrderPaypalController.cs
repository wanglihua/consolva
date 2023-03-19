using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号 PayPal支付 处理支付接口
/// </summary>
[NoLoginRequired]
[PayArea]
public class WechatOfficialAccountPaypalController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    
}
using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(AdminOnly = true)]
public class PaypalWebhookAdminController : Controller {
    
    
}
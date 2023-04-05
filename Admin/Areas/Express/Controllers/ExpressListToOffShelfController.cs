using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;

namespace shunda.Admin.Areas.Express.Controllers; 

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressListToOffShelfController : Controller {
    
}
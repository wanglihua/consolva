using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerAccountEntry;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

public class CustomerAccountEntryController : Controller
{
    private readonly ISession session;

    public CustomerAccountEntryController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/account/entry/list")]
    public IActionResult List([FromBody] CustomerAccountEntryListParam param)
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerAccountEntryListModel {
            }
        });
    }
}
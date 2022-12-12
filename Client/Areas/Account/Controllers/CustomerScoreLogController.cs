using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerScoreLog;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreLogController : Controller
{
    private readonly ISession session;

    public CustomerScoreLogController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/score/log")]
    public IActionResult List([FromBody] CustomerScoreLogListParam param)
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerScoreLogListModel {
            }
        });
    }
}
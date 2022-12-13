using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerScoreLog;
using shunda.Client.Models;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreLogController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerScoreLogController(ISession session,
        LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/score/log")]
    public IActionResult List([FromBody] CustomerScoreLogListParam param)
    {
        var listQuery = GetListQuery(param);

        if (param.LastId != ListLastIdModel.LAST_ID_NOT_SET_VALUE)
        {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        var scoreLogList = listQuery
            .OrderByDescending(e => e.Id)
            .Take(15)
            .ToList();

        var scoreLogItemList = scoreLogList
            .Select(e => new CustomerScoreLogListItem() {
                Id = e.Id,
                TypeName = e.ScoreType.Name,
                Score = (e.Score > 0 ? "+" : "") + e.Score.ToString("#0.00"),
                Amount = e.Amount.ToString("#0.00"),
                IsValidScore = e.IsValidScore,
                Description = e.Description,
                OrderId = e.LogisticsOrder?.Id,
                OrderCode = e.LogisticsOrderCode ?? string.Empty,
                OrderCustomerId = e.LogisticsOrderCustomer?.Id,
                CreateTime = e.CreateTime.ToString("yyyy-MM-dd HH:mm"),
            })
            .ToList();

        var lastData = scoreLogList.LastOrDefault();
        int lastId = lastData?.Id ?? ListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        int logCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerScoreLogListModel {
                LogList = scoreLogItemList,
                LogCount = logCount,
                LastId = lastId,
            }
        });
    }

    [NonAction]
    private IQueryable<CustomerScoreLogInfo> GetListQuery(CustomerScoreLogListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return session.Query<CustomerScoreLogInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);
    }
}
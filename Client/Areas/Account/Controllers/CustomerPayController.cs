using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerPay;
using shunda.Client.Models;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerPayController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerPayController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/pay/list")]
    public IActionResult List([FromBody] CustomerPayListParam param)
    {
        var listQuery = GetListQuery(param);

        if (param.LastId != ListLastIdModel.LAST_ID_NOT_SET_VALUE)
        {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        IList<LogisticsOrderPayInfo> orderPayList = listQuery
            .OrderByDescending(e => e.Id)
            .Take(15)
            .ToList();

        IList<CustomerPayListItem> payItemList = orderPayList
            .Select(e => new CustomerPayListItem() {
                Id = e.Id,
                TypeName = e.PayType.Name,
                Amount = e.Amount.ToString("#0.00") + "元",
                OrderCode = e.OrderCode,
                PayTime = e.CreateTime.ToString("yyyy-MM-dd HH:mm")
            })
            .ToList();

        var lastPay = orderPayList.LastOrDefault();
        int lastId = lastPay?.Id ?? ListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        int orderPayCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerPayListModel {
                PayList = payItemList,
                PayCount = orderPayCount,
                LastId = lastId,
            },
        });
    }

    [NonAction]
    private IQueryable<LogisticsOrderPayInfo> GetListQuery(CustomerPayListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.Customer.Id == loginCustomerId
                        && e.Status == PayStatus.Paid);
    }
}
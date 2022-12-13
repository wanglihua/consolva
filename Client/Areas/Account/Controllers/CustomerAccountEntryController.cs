using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerAccountEntry;
using shunda.Client.Models;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerAccountEntryController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerAccountEntryController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/account/entry/list")]
    public IActionResult List([FromBody] CustomerAccountEntryListParam param)
    {
        var listQuery = GetListQuery(param);

        if (param.LastId != ListLastIdModel.LAST_ID_NOT_SET_VALUE)
        {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        var accountEntryList = listQuery
            .OrderByDescending(e => e.Id)
            .Take(15)
            .ToList();

        var entryItemList = accountEntryList
            .Select(e => new CustomerAccountEntryListItem {
                Id = e.Id,
                ChangeTypeName = e.ChangeType.Name,
                Amount = (e.Amount > 0 ? "+" : "") + e.Amount.ToString("#0.00"),
                AmountBefore = e.AccountBefore.ToString("#0.00"),
                AmountAfter = e.AccountAfter.ToString("#0.00"),
                Description = e.Description,
                CreateTime = e.CreateTime.ToString("yyyy-MM-dd HH:mm"),
            })
            .ToList();

        var lastData = accountEntryList.LastOrDefault();
        int lastId = lastData?.Id ?? ListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        int accountEntryCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerAccountEntryListModel {
                EntryList = entryItemList,
                EntryCount = accountEntryCount,
                LastId = lastId,
            }
        });
    }

    [NonAction]
    private IQueryable<CustomerAccountEntryInfo> GetListQuery(CustomerAccountEntryListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return session.Query<CustomerAccountEntryInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);
    }
}
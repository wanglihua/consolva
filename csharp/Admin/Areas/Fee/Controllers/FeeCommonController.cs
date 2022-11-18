using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Fee.Models.FeeCommon;
using shunda.Core;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
public class FeeCommonController : Controller
{
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public FeeCommonController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    /// <summary>
    /// 获取客户最近订单编号列表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("/admin/fee/common/get/customer/recent/order/select")]
    public IActionResult GetCustomerRecentOrderSelect(string customerCode)
    {
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);
        if (customerId == 0)
        {
            throw new ValidationException("指定的客户ID不存在！");
        }

        IList<string> orderCodeList = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.OrderStatus != LogisticsOrderStatus.Completed)
            .OrderByDescending(e => e.Id)
            .Take(10)
            .Select(e => e.Code)
            .ToList();

        FeeCommonGetCustomerRecentOrderSelectModel model = new FeeCommonGetCustomerRecentOrderSelectModel();
        model.OrderCodeList = orderCodeList;

        return View("FeeCommonGetCustomerRecentOrderSelect", model);
    }

    [HttpPost]
    [Route("/admin/fee/common/get/customer/code/by/order/code")]
    public IActionResult GetCustomerCodeByOrderCode(string orderCode)
    {
        string customerCode = string.Empty;

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>().Where(e => e.Code == orderCode).Take(1).SingleOrDefault();
        if (logisticsOrder != null)
        {
            customerCode = customerCodeService.GetCustomerCode(logisticsOrder.Customer.Id);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取成功！" }, // 没有异常啥的就算成功了
            { "customerCode", customerCode }
        });
    }
}
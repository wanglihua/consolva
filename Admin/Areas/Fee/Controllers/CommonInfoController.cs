using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Fee.Models.CommonInfo;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[NoLoginRequired]
public class CommonInfoController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;

    public CommonInfoController(ISession session, CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    /// <summary>
    /// 获取客户最近订单编号列表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("/admin/fee/common/info/get/customer/recent/order/select")]
    public IActionResult GetCustomerRecentOrderSelect(string customerCode) {
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);
        if (customerId == 0) {
            throw new ValidationException("指定的客户ID不存在！");
        }

        IList<string> orderCodeList = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.OrderStatus != LogisticsOrderStatus.Canceled
                        && e.OrderStatus != LogisticsOrderStatus.Completed)
            .OrderByDescending(e => e.Id)
            .Take(10)
            .Select(e => e.Code)
            .ToList();

        CommonInfoGetCustomerRecentOrderSelectModel model = new CommonInfoGetCustomerRecentOrderSelectModel();
        model.OrderCodeList = orderCodeList;

        return View("CommonInfoGetCustomerRecentOrderSelect", model);
    }

    [HttpPost]
    [Route("/admin/fee/common/info/get/customer/code/by/order/code")]
    public IActionResult GetCustomerCodeByOrderCode(string orderCode) {
        string customerCode = string.Empty;

        var logisticsOrder = session.Query<LogisticsOrderInfo>().Where(e => e.Code == orderCode).Take(1).SingleOrDefault();
        if (logisticsOrder != null) {
            customerCode = logisticsOrder.Customer.Id.ToString();
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取成功！" },
            { "customerCode", customerCode }
        });
    }
}
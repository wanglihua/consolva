using Microsoft.AspNetCore.Mvc;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderListController : Controller {
    private readonly ISession session;
    private readonly LogisticsOrderClientListService logisticsOrderClientListService;

    public OrderListController(ISession session,
        LogisticsOrderClientListService logisticsOrderClientListService
    ) {
        this.session = session;
        this.logisticsOrderClientListService = logisticsOrderClientListService;
    }

    [HttpPost]
    [Route("/client/order/list/pay")]
    public IActionResult ListPay([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}


/*

    @RequestMapping(value = "/customer/order/list/pay", method = RequestMethod.POST)
    public Map<String, Object> listPay(@RequestBody @Valid CustomerOrderListParam param, HttpServletRequest request) {

        int customerId = loginCustomerService.getLoginCustomerId(request);

        List<Map<String, Object>> orderList = getCustomerOrderList(customerId, CustomerOrderStatusCate.Pay, param);
        int orderCount = getCustomerOrderCount(customerId, CustomerOrderStatusCate.Pay, param);

        Integer lastOrderId = param.getPreLastOrderId();
        if (orderList.size() != 0) {
            lastOrderId = (Integer) orderList.get(orderList.size() - 1).get("id");
        }

        final Integer lastOrderIdFinal = lastOrderId;

        return new HashMap<>() {{
            put("success", true);
            put("message", "");
            put("orderList", orderList);
            put("orderCount", orderCount);
            put("lastOrderId", lastOrderIdFinal);
        }};
    }

    @RequestMapping(value = "/customer/order/list/submitted", method = RequestMethod.POST)
    public Map<String, Object> listSubmitted(@RequestBody @Valid CustomerOrderListParam param, HttpServletRequest request) {

        int customerId = loginCustomerService.getLoginCustomerId(request);

        List<Map<String, Object>> orderList = getCustomerOrderList(customerId, CustomerOrderStatusCate.Submitted, param);
        int orderCount = getCustomerOrderCount(customerId, CustomerOrderStatusCate.Submitted, param);

        Integer lastOrderId = param.getPreLastOrderId();
        if (orderList.size() != 0) {
            lastOrderId = (Integer) orderList.get(orderList.size() - 1).get("id");
        }

        final Integer lastOrderIdFinal = lastOrderId;

        return new HashMap<>() {{
            put("success", true);
            put("message", "");
            put("orderList", orderList);
            put("orderCount", orderCount);
            put("lastOrderId", lastOrderIdFinal);
        }};
    }

    @RequestMapping(value = "/customer/order/list/waiting/transport", method = RequestMethod.POST)
    public Map<String, Object> listWaitingTransport(@RequestBody @Valid CustomerOrderListParam param, HttpServletRequest request) {

        int customerId = loginCustomerService.getLoginCustomerId(request);

        List<Map<String, Object>> orderList = getCustomerOrderList(customerId, CustomerOrderStatusCate.WaitingTransport, param);
        int orderCount = getCustomerOrderCount(customerId, CustomerOrderStatusCate.WaitingTransport, param);

        Integer lastOrderId = param.getPreLastOrderId();
        if (orderList.size() != 0) {
            lastOrderId = (Integer) orderList.get(orderList.size() - 1).get("id");
        }

        final Integer lastOrderIdFinal = lastOrderId;

        return new HashMap<>() {{
            put("success", true);
            put("message", "");
            put("orderList", orderList);
            put("orderCount", orderCount);
            put("lastOrderId", lastOrderIdFinal);
        }};
    }

    @RequestMapping(value = "/customer/order/list/transporting", method = RequestMethod.POST)
    public Map<String, Object> listTransporting(@RequestBody @Valid CustomerOrderListParam param, HttpServletRequest request) {

        int customerId = loginCustomerService.getLoginCustomerId(request);

        List<Map<String, Object>> orderList = getCustomerOrderList(customerId, CustomerOrderStatusCate.Transporting, param);
        int orderCount = getCustomerOrderCount(customerId, CustomerOrderStatusCate.Transporting, param);

        Integer lastOrderId = param.getPreLastOrderId();
        if (orderList.size() != 0) {
            lastOrderId = (Integer) orderList.get(orderList.size() - 1).get("id");
        }

        final Integer lastOrderIdFinal = lastOrderId;

        return new HashMap<>() {{
            put("success", true);
            put("message", "");
            put("orderList", orderList);
            put("orderCount", orderCount);
            put("lastOrderId", lastOrderIdFinal);
        }};
    }

    @RequestMapping(value = "/customer/order/list/delivered", method = RequestMethod.POST)
    public Map<String, Object> listDelivered(@RequestBody @Valid CustomerOrderListParam param, HttpServletRequest request) {

        int customerId = loginCustomerService.getLoginCustomerId(request);

        List<Map<String, Object>> orderList = getCustomerOrderList(customerId, CustomerOrderStatusCate.Delivered, param);
        int orderCount = getCustomerOrderCount(customerId, CustomerOrderStatusCate.Delivered, param);

        Integer lastOrderId = param.getPreLastOrderId();
        if (orderList.size() != 0) {
            lastOrderId = (Integer) orderList.get(orderList.size() - 1).get("id");
        }

        final Integer lastOrderIdFinal = lastOrderId;

        return new HashMap<>() {{
            put("success", true);
            put("message", "");
            put("orderList", orderList);
            put("orderCount", orderCount);
            put("lastOrderId", lastOrderIdFinal);
        }};
    }
    
*/
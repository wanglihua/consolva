using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderList;
using shunda.Client.Services;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderListController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderClientListService logisticsOrderClientListService;

    public OrderListController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderClientListService logisticsOrderClientListService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderClientListService = logisticsOrderClientListService;
    }

    [HttpPost]
    [Route("/client/order/list/pay")]
    public IActionResult ListPay([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        param.CustomerId = loginCustomerId;
        param.Cate = LogisticsOrderStatusClientCate.Pay;

        int orderCount = logisticsOrderClientListService.GetCustomerOrderCount(param);
        var orderList = logisticsOrderClientListService.GetCustomerOrderList(param);

        var lastData = orderList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderListModel {
                OrderCount = orderCount,
                OrderList = orderList,
                LastId = lastId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/list/submitted")]
    public IActionResult ListSubmitted([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        param.CustomerId = loginCustomerId;
        param.Cate = LogisticsOrderStatusClientCate.Submitted;

        int orderCount = logisticsOrderClientListService.GetCustomerOrderCount(param);
        var orderList = logisticsOrderClientListService.GetCustomerOrderList(param);

        var lastData = orderList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderListModel {
                OrderCount = orderCount,
                OrderList = orderList,
                LastId = lastId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/list/waiting/transport")]
    public IActionResult ListWaitingTransport([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        param.CustomerId = loginCustomerId;
        param.Cate = LogisticsOrderStatusClientCate.WaitingTransport;

        int orderCount = logisticsOrderClientListService.GetCustomerOrderCount(param);
        var orderList = logisticsOrderClientListService.GetCustomerOrderList(param);

        var lastData = orderList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderListModel {
                OrderCount = orderCount,
                OrderList = orderList,
                LastId = lastId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/list/transporting")]
    public IActionResult ListTransporting([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        param.CustomerId = loginCustomerId;
        param.Cate = LogisticsOrderStatusClientCate.Transporting;

        int orderCount = logisticsOrderClientListService.GetCustomerOrderCount(param);
        var orderList = logisticsOrderClientListService.GetCustomerOrderList(param);

        var lastData = orderList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderListModel {
                OrderCount = orderCount,
                OrderList = orderList,
                LastId = lastId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/list/delivered")]
    public IActionResult ListDelivered([FromBody] LogisticsOrderClientListParam param) {
        ValidationUtil.Validate<LogisticsOrderClientListParamWebValidator, LogisticsOrderClientListParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        param.CustomerId = loginCustomerId;
        param.Cate = LogisticsOrderStatusClientCate.Transporting;

        int orderCount = logisticsOrderClientListService.GetCustomerOrderCount(param);
        var orderList = logisticsOrderClientListService.GetCustomerOrderList(param);

        var lastData = orderList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderListModel {
                OrderCount = orderCount,
                OrderList = orderList,
                LastId = lastId,
            },
        });
    }
}
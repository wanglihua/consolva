using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailOrderPackController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly PassResService passResService;
    private readonly LogisticsOrderFeeService logisticsOrderFeeService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;

    public LogisticsOrderDetailOrderPackController(
        ISession session,
        LoginUserService loginUserService,
        PassResService passResService,
        LogisticsOrderFeeService logisticsOrderFeeService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.passResService = passResService;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/order/pack/list")]
    public IActionResult List(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        // 订单头程物流，打包信息，箱号
        IList<LogisticsOrderFirstModeInfo> orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<LogisticsOrderPackInfo> orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var orderPackExpressList = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.Express)
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var expressIdList = orderPackExpressList.Select(e => e.Express.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IList<LogisticsOrderPackBoxInfo> orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        foreach (var packBox in orderPackBoxList) {
            packBox.CodeCache = logisticsOrderPackBoxService.GetBoxCode(packBox.Id);
        }

        foreach (var orderPack in orderPackList) {
            int orderPackId = orderPack.Id;

            var boxListOfPack = orderPackBoxList.Where(e => e.OrderPack.Id == orderPackId).OrderBy(e => e.Id).ToList();
            orderPack.PackBoxCodeListCache = boxListOfPack.Select(e => e.CodeCache).ToList();

            var orderPackExpressListOfPack = orderPackExpressList.Where(e => e.OrderPack.Id == orderPackId).ToList();
            var expressListOfPack = orderPackExpressListOfPack.Select(e => e.Express).ToList();
            orderPack.TrackingNoListCache = expressListOfPack.Select(e => e.TrackingNo).ToList();

            var expressIdListOfPack = expressListOfPack.Select(e => e.Id).ToList();
            var expressGoodsListOfPack = expressGoodsList.Where(e => expressIdListOfPack.Contains(e.Express.Id)).ToList();
            orderPack.GoodsNameListCache = expressGoodsListOfPack.Select(e => e.Goods.Name).Distinct().ToList();
        }

        foreach (var orderFirstMode in orderFirstModeList) {
            int orderFirstModeId = orderFirstMode.Id;

            orderFirstMode.OrderPackListCache = orderPackList
                .Where(e => e.OrderFirstMode.Id == orderFirstModeId)
                .OrderBy(e => e.Id)
                .ToList();
        }

        // 打包汇总信息记录
        var orderPackData = session.Query<LogisticsOrderPackDataInfo>()
            .Where(e => e.Order.Id == orderId)
            .SingleOrDefault();
        if (orderPackData == null) {
            throw new ValidationException($"指定的订单{order.Code}打包汇总数据不存在！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        var model = new LogisticsOrderDetailOrderPackListModel {
            OrderFirstModeList = orderFirstModeList,
            OrderPackData = orderPackData,
            HasModifyOrderPackDataPass = passResService.UserHasPassRes(loginUserId, PassRes.LogisticsOrder.OrderPackDataModify)
        };

        return View("LogisticsOrderDetailPackList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/show/order/pack/detail")]
    [AdminAuth(PassRes.LogisticsOrder.OrderPackDataModify)]
    public IActionResult ShowDetail(int orderPackId) {
        var orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException($"指定的的订单打包信息#{orderPackId}不存在！");
        }

        LogisticsOrderDetailOrderPackDetailModel model = new LogisticsOrderDetailOrderPackDetailModel();
        model.Width = orderPack.Width;
        model.Length = orderPack.Length;
        model.Height = orderPack.Height;
        model.Weight = orderPack.Weight;

        return View("LogisticsOrderDetailPackDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/save/order/pack")]
    [AdminAuth(PassRes.LogisticsOrder.OrderPackDataModify)]
    public IActionResult SavePack(LogisticsOrderDetailOrderPackSavelModel model) {
        ValidationUtil.Validate<LogisticsOrderDetailOrderPackSavelModelValidator, LogisticsOrderDetailOrderPackSavelModel>(model);

        if (model.Width <= 0 || model.Length <= 0 || model.Height <= 0 || model.Weight <= 0) {
            throw new ValidationException("长宽高重量数据不能小于0！");
        }

        int orderPackId = model.OrderPackId;
        var orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException($"指定的订单打包信息{orderPackId}不存在！");
        }

        orderPack.Width = model.Width;
        orderPack.Length = model.Length;
        orderPack.Height = model.Height;
        orderPack.Weight = model.Weight;

        session.Save(orderPack);

        // 重新计算费用
        int orderId = orderPack.Order.Id;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        // 重新计费
        logisticsOrderFeeService.CalculateAndSaveFee(orderId);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "打包信息保存成功！" }
        });
    }
}
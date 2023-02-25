using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Components;

public class LogisticsOrderDetailOrderPackListViewComponent : ViewComponent {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly PassResService passResService;
    private readonly LogisticsOrderFeeService logisticsOrderFeeService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;

    public LogisticsOrderDetailOrderPackListViewComponent(
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

    public IViewComponentResult Invoke(int orderId) {
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

        // 这个循环里面，很多的 order first mode 都是相同的，所以也不大会影响性能
        foreach (var orderPack in orderPackList) {
            NHibernateUtil.Initialize(orderPack.OrderFirstMode);
            NHibernateUtil.Initialize(orderPack.OrderFirstMode.FirstMode);
        }

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
        var model = new LogisticsOrderDetailOrderPackListViewCompenentModel {
            OrderFirstModeList = orderFirstModeList,
            OrderPackData = orderPackData,
            HasModifyOrderPackDataPass = passResService.UserHasPassRes(loginUserId, PassRes.LogisticsOrder.OrderPackDataModify)
        };

        return View("~/Areas/LogisticsOrder/Views/LogisticsOrderDetailOrderPack/LogisticsOrderDetailPackListViewComponent.cshtml", model);
    }
}
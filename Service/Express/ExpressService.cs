using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressService {
    private readonly ISession session;
    private readonly ExpressLogService expressLogService;
    private readonly StorageLocExpressService storageLocExpressService;

    public ExpressService(ISession session, ExpressLogService expressLogService, StorageLocExpressService storageLocExpressService) {
        this.session = session;
        this.expressLogService = expressLogService;
        this.storageLocExpressService = storageLocExpressService;
    }

    /// <summary>
    /// 获取订单快递单列表，用于订单详细界面显示
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public IList<ExpressInfo> GetExpressListForOrderDetail(int orderId) {
        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .ToList();

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        foreach (var express in expressList) {
            int expressId = express.Id;
            express.ExpressGoodsListCache = expressGoodsList
                .Where(e => e.Express.Id == expressId)
                .OrderBy(e => e.Id)
                .ToList();
        }

        return expressList;
    }

    public void Delete(int expressId, int userId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        if (ExpressStatus.IsPicked(express.Status)) {
            throw new ValidationException("已认领的快递单，不能被删除！");
        }

        var shelfSchedule = session
            .Query<ExpressShelfScheduleInfo>()
            .Where(e => e.Express.Id == expressId)
            .Take(1)
            .SingleOrDefault();

        if (shelfSchedule != null) {
            throw new ValidationException("快递单有排班记录，先取消排班再删除！");
        }

        expressLogService.UserLog(expressId, ExpressLogType.Delete, userId);

        var expressLogList = session.Query<ExpressLogInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();
        if (expressLogList.Count != 0) {
            foreach (var expressLog in expressLogList) {
                expressLog.Express = null;
                session.Update(expressLog);
            }

            session.Flush();
        }

        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();
        foreach (var expressGoods in expressGoodsList) {
            session.Delete(expressGoods);
        }

        session.Flush();

        session.Delete(express);
        session.Flush();
    }

    public void ForceDelete(int expressId, int userId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        if (ExpressStatus.IsOrderCreate(express.Status)) {
            throw new ValidationException("已创建转运单的快递单不能被删除！");
        }

        expressLogService.UserLog(expressId, ExpressLogType.Delete, userId);

        // 设置日志中的快递单关联为空
        var expressLogList = session.Query<ExpressLogInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();
        if (expressLogList.Count != 0) {
            foreach (var expressLog in expressLogList) {
                expressLog.Express = null;
                session.Update(expressLog);
            }

            session.Flush();
        }

        // 设置尺寸重量中的快递单关联为空
        var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();
        foreach (var sw in sizeWeightList) {
            sw.Express = null;
            session.Update(sw);
        }

        session.Flush();

        // 设置快递单图片中的快递单关联为空
        var photoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();
        foreach (var p in photoList) {
            p.Express = null;
            session.Update(p);
        }

        session.Flush();

        // 删除上下架排班
        var shelfScheduleList = session
            .Query<ExpressShelfScheduleInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();
        if (shelfScheduleList.Count != 0) {
            foreach (var shelfSchedule in shelfScheduleList) {
                session.Delete(shelfSchedule);
            }

            session.Flush();
        }

        // 快递单物品
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();
        foreach (var expressGoods in expressGoodsList) {
            session.Delete(expressGoods);
        }

        session.Flush();

        session.Delete(express);
        session.Flush();
    }

    public void ThrowAway(int expressId, int userId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        if (ExpressStatus.IsOrderCreate(express.Status)) {
            throw new ValidationException("已创建转运单的快递单不能设置已丢弃！");
        }

        expressLogService.UserLog(expressId, ExpressLogType.ThrowAway, userId);

        express.Status = ExpressStatus.ThrowAway;
        session.Update(express);
        session.Flush();

        // 释放仓位

        storageLocExpressService.UpdateSingle(express.StorageLoc.Id);
        session.Flush();
    }
}
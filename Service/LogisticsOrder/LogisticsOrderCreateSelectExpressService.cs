using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectExpressService {
    private readonly ISession session;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public LogisticsOrderCreateSelectExpressService(ISession session, LogisticsOrderCreateSelectService logisticsOrderCreateSelectService) {
        this.session = session;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    /// <summary>
    /// 添加已入库快递单号至待建单
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="customerId"></param>
    public void AddWaitOrder(int expressId, int customerId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (express.Customer == null || express.Status != ExpressStatus.Checkin) {
            throw new ValidationException($"快递单号#{expressId}未被认领，或不是已入库状态！");
        }

        if ((express.Customer?.Id ?? 0) != customerId) {
            throw new ValidationException($"指定的快递单号不属于指定的会员#{customerId}！");
        }

        // 检查价格是不是设置了
        if (express.GoodsValue <= 0) {
            throw new ValidationException($"快递单号 {express.TrackingNo} 货品总值为空，不能转运，请点击货品总值修改！");
        }

        var createSelectExpress = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == customerId && e.ExpressId == expressId)
            .Take(1)
            .SingleOrDefault();
        if (createSelectExpress == null) {
            createSelectExpress = new LogisticsOrderCreateSelectExpressInfo {
                CustomerId = customerId,
                ExpressId = expressId
            };

            session.Save(createSelectExpress);
            session.Flush();
        }

        logisticsOrderCreateSelectService.MakeInvalid(customerId);
    }

    /// <summary>
    /// 添加客户所有已入库快递单号至待建单
    /// </summary>
    /// <param name="customerId"></param>
    public void AddWaitOrderAll(int customerId) {
        // 同时，物品列表查询时，相当于对应的快递单实体已经预取了
        var expressList = session.Query<ExpressInfo>()
            .Where(e => e.Customer!.Id == customerId && e.Status == ExpressStatus.Checkin)
            .ToList();

        // 检查物品价格设置
        foreach (var express in expressList) {
            if (express.GoodsValue <= 0) {
                throw new ValidationException($"快递单号 {express.TrackingNo} 货品总值为空，不能转运，请点击货品总值修改！");
            }
        }

        var createSelectExpressList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == customerId)
            .ToList();

        var expressIdList = expressList.Select(e => e.Id).ToList();
        foreach (var expressId in expressIdList) {
            var createSelectExpress = createSelectExpressList
                .Where(e => e.ExpressId == expressId)
                .Take(1)
                .SingleOrDefault();

            if (createSelectExpress == null) // 当前不是待建单的才需要保存
            {
                createSelectExpress = new LogisticsOrderCreateSelectExpressInfo {
                    ExpressId = expressId,
                    CustomerId = customerId,
                };

                session.Save(createSelectExpress);
            }
        }

        session.Flush();

        logisticsOrderCreateSelectService.MakeInvalid(customerId);
    }

    /// <summary>
    /// 将已入库快递单号从待建单移除
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="customerId"></param>
    public void RemoveWaitOrder(int expressId, int customerId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (express.Customer == null || express.Status != ExpressStatus.Checkin) {
            throw new ValidationException($"快递单号#{expressId}未被认领，或不是已入库状态！");
        }

        if ((express.Customer?.Id ?? 0) != customerId) {
            throw new ValidationException($"指定的快递单号不属于指定的会员#{customerId}！");
        }

        var createSelectExpress = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == customerId && e.ExpressId == expressId)
            .Take(1)
            .SingleOrDefault();
        if (createSelectExpress != null) {
            session.Delete(createSelectExpress);
            session.Flush();
        }

        logisticsOrderCreateSelectService.MakeInvalid(customerId);
    }

    /// <summary>
    /// 将客户所有已入库快递单号从待建单移除
    /// </summary>
    /// <param name="customerId"></param>
    public void RemoveWaitOrderAll(int customerId) {
        var createSelectExpressList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == customerId)
            .ToList();

        foreach (var createSelectExpress in createSelectExpressList) {
            session.Delete(createSelectExpress);
        }

        session.Flush();

        logisticsOrderCreateSelectService.MakeInvalid(customerId);
    }
}
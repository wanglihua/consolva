using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectExpressService
{
    private readonly ISession session;

    public LogisticsOrderCreateSelectExpressService(ISession session)
    {
        this.session = session;
    }

    /// <summary>
    /// 添加已入库快递单号至待建单
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="customerId"></param>
    public void AddWaitOrder(int expressId, int customerId)
    {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (express.Customer == null || express.Status != ExpressStatus.CheckIn)
        {
            throw new ValidationException($"快递单号#{expressId}未被认领，或不是已入库状态！");
        }

        if ((express.Customer?.Id ?? 0) != customerId)
        {
            throw new ValidationException($"指定的快递单号不属于指定的会员#{customerId}！");
        }
    }

    /// <summary>
    /// 将已入库快递单号从待建单移除
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="customerId"></param>
    public void RemoveWaitOrder(int expressId, int customerId)
    {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (express.Customer == null || express.Status != ExpressStatus.CheckIn)
        {
            throw new ValidationException($"快递单号#{expressId}未被认领，或不是已入库状态！");
        }

        if ((express.Customer?.Id ?? 0) != customerId)
        {
            throw new ValidationException($"指定的快递单号不属于指定的会员#{customerId}！");
        }        
    }
}
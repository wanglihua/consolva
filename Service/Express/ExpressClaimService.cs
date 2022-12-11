using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressClaimService
{
    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;

    public ExpressClaimService(ISession session, ExpressTrackingNoService expressTrackingNoService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
    }

    /// <summary>
    /// 快递单号认领
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="trackingNoInput"></param>
    /// <param name="customerId"></param>
    public void Pick(int expressId, string trackingNoInput, int customerId)
    {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        if (expressTrackingNoService.IsTrackingNoMatch(express.TrackingNo, trackingNoInput) == false)
        {
            throw new ValidationException("快递单号匹配失败，请检查重新录入！");
        }

        // 检查下状态，不能重复认领
        if (express.Status != ExpressStatus.WaitPick)
        {
            throw new ValidationException("当前快递单号状态不是待认领状态！");
        }

        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();
    }
}
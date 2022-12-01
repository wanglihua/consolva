using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Fee;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class FeeApplyCountService
{
    private ISession session;

    public FeeApplyCountService(ISession session)
    {
        this.session = session;
    }

    public IDictionary<int, int> GetApplyCount(IList<int> orderIdList)
    {
        IList<FeeWriteOffApplyCountInfo> writeOffApplyCountList = session
            .Query<FeeWriteOffApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        IList<ManualFeeApplyCountInfo> manualFeeApplyCountList = session
            .Query<ManualFeeApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        IList<FeeRefundApplyCountInfo> refundApplyCountList = session
            .Query<FeeRefundApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        IDictionary<int, int> dictOrderApplyCount = new Dictionary<int, int>();

        foreach (FeeWriteOffApplyCountInfo applyCount in writeOffApplyCountList)
        {
            int count = applyCount.ApplyCount;
            if (dictOrderApplyCount.ContainsKey(applyCount.OrderId))
            {
                count = count + dictOrderApplyCount[applyCount.OrderId];
            }

            dictOrderApplyCount[applyCount.OrderId] = count;
        }

        foreach (ManualFeeApplyCountInfo applyCount in manualFeeApplyCountList)
        {
            int count = applyCount.ApplyCount;
            if (dictOrderApplyCount.ContainsKey(applyCount.OrderId))
            {
                count = count + dictOrderApplyCount[applyCount.OrderId];
            }

            dictOrderApplyCount[applyCount.OrderId] = count;
        }

        foreach (FeeRefundApplyCountInfo applyCount in refundApplyCountList)
        {
            int count = applyCount.ApplyCount;
            if (dictOrderApplyCount.ContainsKey(applyCount.OrderId))
            {
                count = count + dictOrderApplyCount[applyCount.OrderId];
            }

            dictOrderApplyCount[applyCount.OrderId] = count;
        }

        return dictOrderApplyCount;
    }

    public IList<string> GetApplyCountDescList(int orderId)
    {
        IList<string> applyCountDescList = new List<string>();

        FeeWriteOffApplyCountInfo? writeOffApplyCount = session
            .Query<FeeWriteOffApplyCountInfo>()
            .Where(e => e.OrderId == orderId)
            .Take(1)
            .SingleOrDefault();

        if (writeOffApplyCount != null)
        {
            applyCountDescList.Add(string.Format("订单有{0}条【减少费用】申请", writeOffApplyCount.ApplyCount));
        }

        ManualFeeApplyCountInfo? manualFeeApplyCount = session
            .Query<ManualFeeApplyCountInfo>()
            .Where(e => e.OrderId == orderId)
            .Take(1)
            .SingleOrDefault();

        if (manualFeeApplyCount != null)
        {
            applyCountDescList.Add(string.Format("订单有{0}条【费用核销】申请", manualFeeApplyCount.ApplyCount));
        }

        FeeRefundApplyCountInfo? refundApplyCount = session
            .Query<FeeRefundApplyCountInfo>()
            .Where(e => e.OrderId == orderId)
            .Take(1)
            .SingleOrDefault();

        if (refundApplyCount != null)
        {
            applyCountDescList.Add(string.Format("订单有{0}条【退赔款】申请", refundApplyCount.ApplyCount));
        }

        return applyCountDescList;
    }
}
using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressClaimService {
    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressExpectService expressExpectService;
    private readonly ExpressLogService expressLogService;
    private readonly ExpressPhotoMessageService expressPhotoMessageService;

    public ExpressClaimService(ISession session,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressExpectService expressExpectService,
        ExpressLogService expressLogService,
        ExpressPhotoMessageService expressPhotoMessageService) {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressExpectService = expressExpectService;
        this.expressLogService = expressLogService;
        this.expressPhotoMessageService = expressPhotoMessageService;
    }

    /// <summary>
    /// 快递单号认领
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="trackingNoInput"></param>
    /// <param name="customerId"></param>
    public void Pick(int expressId, string trackingNoInput, int customerId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        string trackingNo = express.TrackingNo;

        if (expressTrackingNoService.IsTrackingNoMatch(trackingNo, trackingNoInput) == false) {
            throw new ValidationException("快递单号匹配失败，请检查重新录入！");
        }

        // 检查下状态，不能重复认领
        // if (express.Status != ExpressStatus.Expect && express.Status != ExpressStatus.WaitPick)
        // if (express.Status != ExpressStatus.WaitPick) {
        //     throw new ValidationException("当前快递单号状态不是 待认领 状态！");
        // }

        if (express.Customer != null) {
            throw new ValidationException("当前快递单号之前已经被认领！");
        }

        if (express.GoodsValue <= 0) {
            throw new ValidationException("快递单货品总值不能为空！");
        }

        express.ClaimTime = DateTime.Now;
        express.Status = ExpressStatus.Checkin;
        express.Customer = customer;

        session.Update(express);
        session.Flush();

        var expressExpect = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (expressExpect != null) {
            expressExpectService.Pick(expressExpect.Id, customerId);
        }

        expressLogService.CustomerLog(expressId, ExpressLogType.Claim, customerId);

        // 发送可能的快递单图片
        expressPhotoMessageService.SendSignAndCheckinMessageIfNotSentWhenCheckinOrPick(expressId);
    }
}
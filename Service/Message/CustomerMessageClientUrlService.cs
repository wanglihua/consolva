using NHibernate;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Message.CustomerMessage;

namespace shunda.Service.Message;

/// <summary>
/// 客户消息客户端URL 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageClientUrlService {
    private readonly ISession session;
    private readonly CustomerMessageAccountChangeService customerMessageAccountChangeService;
    private readonly CustomerMessageCouponFollowService customerMessageCouponFollowService;
    private readonly CustomerMessageExpressCheckinService customerMessageExpressCheckinService;
    private readonly CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService;
    private readonly CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService;
    private readonly CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService;

    public CustomerMessageClientUrlService(
        ISession session,
        CustomerMessageAccountChangeService customerMessageAccountChangeService,
        CustomerMessageCouponFollowService customerMessageCouponFollowService,
        CustomerMessageExpressCheckinService customerMessageExpressCheckinService,
        CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService,
        CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService,
        CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService
    ) {
        this.session = session;
        this.customerMessageAccountChangeService = customerMessageAccountChangeService;
        this.customerMessageCouponFollowService = customerMessageCouponFollowService;
        this.customerMessageExpressCheckinService = customerMessageExpressCheckinService;
        this.customerMessageExpressExpectEnterService = customerMessageExpressExpectEnterService;
        this.customerMessageExpressPhotoCheckinService = customerMessageExpressPhotoCheckinService;
        this.customerMessageExpressPhotoSignService = customerMessageExpressPhotoSignService;
    }

    public string GetUrl(CustomerMessageInfo customerMessage) {
        return GetUrl(TerminalType.WechatApp, customerMessage);
    }

    public string GetUrl(TerminalType terminalType, CustomerMessageInfo customerMessage) {
        if (terminalType != TerminalType.WechatApp) {
            return string.Empty;
        }

        CustomerMessageType messageType = customerMessage.Type;
        int relatedId = customerMessage.RelatedId;

        if (messageType == CustomerMessageType.AccountChange) {
            return customerMessageAccountChangeService.GetUrl();
        }

        if (messageType == CustomerMessageType.CouponFollow) {
            return customerMessageCouponFollowService.GetUrl();
        }

        if (messageType == CustomerMessageType.ExpressCheckin) {
            int expressId = relatedId;
            return customerMessageExpressCheckinService.GetUrl(expressId);
        }

        if (messageType == CustomerMessageType.ExpressExpectEnter) {
            int expressId = relatedId;
            return customerMessageExpressExpectEnterService.GetUrl(expressId);
        }

        if (messageType == CustomerMessageType.ExpressPhotoCheckin) {
            int expressPhotoId = relatedId;
            var expressPhoto = session.Get<ExpressPhotoInfo>(expressPhotoId);
            if (expressPhoto == null) {
                return string.Empty;
            }

            string trackingNo = expressPhoto.TrackingNo;
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (express == null) {
                return string.Empty;
            }

            return customerMessageExpressPhotoCheckinService.GetUrl(express.Id);
        }
        
        if (messageType == CustomerMessageType.ExpressPhotoSign) {
            int expressPhotoId = relatedId;
            var expressPhoto = session.Get<ExpressPhotoInfo>(expressPhotoId);
            if (expressPhoto == null) {
                return string.Empty;
            }

            string trackingNo = expressPhoto.TrackingNo;
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (express == null) {
                return string.Empty;
            }

            return customerMessageExpressPhotoSignService.GetUrl(express.Id);
        }        

        return string.Empty;
    }
}
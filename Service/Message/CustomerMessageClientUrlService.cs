using NHibernate;
using NLog;
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
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly string DEFAULT_URL_TITLE = "查看消息关联页面";

    private readonly ISession session;
    private readonly CustomerMessageAccountChangeService customerMessageAccountChangeService;
    private readonly CustomerMessageCouponFollowService customerMessageCouponFollowService;
    private readonly CustomerMessageExpressCheckinService customerMessageExpressCheckinService;
    private readonly CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService;
    private readonly CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService;
    private readonly CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService;
    private readonly CustomerMessageHelpMeMakeMoneyService customerMessageHelpMeMakeMoneyService;
    private readonly CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService;
    private readonly CustomerMessageInternationalExpressTrackingNoService customerMessageInternationalExpressTrackingNoService;
    private readonly CustomerMessageMoneyEarnedService customerMessageMoneyEarnedService;
    private readonly CustomerMessageNewClaimService customerMessageNewClaimService;
    private readonly CustomerMessageNoOrderService customerMessageNoOrderService;
    private readonly CustomerMessageOrderDeliveredService customerMessageOrderDeliveredService;
    private readonly CustomerMessageOrderPhotoShipService customerMessageOrderPhotoShipService;
    private readonly CustomerMessagePackedService customerMessagePackedService;
    private readonly CustomerMessageScoreChangeService customerMessageScoreChangeService;
    private readonly CustomerMessageTransferedService customerMessageTransferedService;

    public CustomerMessageClientUrlService(
        ISession session,
        CustomerMessageAccountChangeService customerMessageAccountChangeService,
        CustomerMessageCouponFollowService customerMessageCouponFollowService,
        CustomerMessageExpressCheckinService customerMessageExpressCheckinService,
        CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService,
        CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService,
        CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService,
        CustomerMessageHelpMeMakeMoneyService customerMessageHelpMeMakeMoneyService,
        CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService,
        CustomerMessageInternationalExpressTrackingNoService customerMessageInternationalExpressTrackingNoService,
        CustomerMessageMoneyEarnedService customerMessageMoneyEarnedService,
        CustomerMessageNewClaimService customerMessageNewClaimService,
        CustomerMessageNoOrderService customerMessageNoOrderService,
        CustomerMessageOrderDeliveredService customerMessageOrderDeliveredService,
        CustomerMessageOrderPhotoShipService customerMessageOrderPhotoShipService,
        CustomerMessagePackedService customerMessagePackedService,
        CustomerMessageScoreChangeService customerMessageScoreChangeService,
        CustomerMessageTransferedService customerMessageTransferedService
    ) {
        this.session = session;
        this.customerMessageAccountChangeService = customerMessageAccountChangeService;
        this.customerMessageCouponFollowService = customerMessageCouponFollowService;
        this.customerMessageExpressCheckinService = customerMessageExpressCheckinService;
        this.customerMessageExpressExpectEnterService = customerMessageExpressExpectEnterService;
        this.customerMessageExpressPhotoCheckinService = customerMessageExpressPhotoCheckinService;
        this.customerMessageExpressPhotoSignService = customerMessageExpressPhotoSignService;
        this.customerMessageHelpMeMakeMoneyService = customerMessageHelpMeMakeMoneyService;
        this.customerMessageInternationalExpressStateUpdateService = customerMessageInternationalExpressStateUpdateService;
        this.customerMessageInternationalExpressTrackingNoService = customerMessageInternationalExpressTrackingNoService;
        this.customerMessageMoneyEarnedService = customerMessageMoneyEarnedService;
        this.customerMessageNewClaimService = customerMessageNewClaimService;
        this.customerMessageNoOrderService = customerMessageNoOrderService;
        this.customerMessageOrderDeliveredService = customerMessageOrderDeliveredService;
        this.customerMessageOrderPhotoShipService = customerMessageOrderPhotoShipService;
        this.customerMessagePackedService = customerMessagePackedService;
        this.customerMessageScoreChangeService = customerMessageScoreChangeService;
        this.customerMessageTransferedService = customerMessageTransferedService;
    }

    public (string url, string urlTitle) GetUrl(CustomerMessageInfo customerMessage) {
        return GetUrl(TerminalType.WechatApp, customerMessage);
    }

    public (string url, string urlTitle) GetUrl(TerminalType terminalType, CustomerMessageInfo customerMessage) {
        if (terminalType != TerminalType.WechatApp) {
            return (string.Empty, DEFAULT_URL_TITLE);
        }

        CustomerMessageType messageType = customerMessage.Type;
        int relatedId = customerMessage.RelatedId;

        if (messageType == CustomerMessageType.AccountChange) {
            return (customerMessageAccountChangeService.GetUrl(), "查看我的余额");
        }

        if (messageType == CustomerMessageType.CouponFollow) {
            return (customerMessageCouponFollowService.GetUrl(), "查看我的优惠券");
        }

        if (messageType == CustomerMessageType.ExpressCheckin) {
            int expressId = relatedId;
            return (customerMessageExpressCheckinService.GetUrl(expressId), "查看快递单");
        }

        if (messageType == CustomerMessageType.ExpressExpectEnter) {
            int expressId = relatedId;
            return (customerMessageExpressExpectEnterService.GetUrl(expressId), "查看快递单");
        }

        if (messageType == CustomerMessageType.ExpressPhotoCheckin) {
            int expressPhotoId = relatedId;
            var expressPhoto = session.Get<ExpressPhotoInfo>(expressPhotoId);
            if (expressPhoto == null) {
                return (string.Empty, DEFAULT_URL_TITLE);
            }

            string trackingNo = expressPhoto.TrackingNo;
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (express == null) {
                return (string.Empty, DEFAULT_URL_TITLE);
            }

            return (customerMessageExpressPhotoCheckinService.GetUrl(express.Id), "查看快递单");
        }

        if (messageType == CustomerMessageType.ExpressPhotoSign) {
            int expressPhotoId = relatedId;
            var expressPhoto = session.Get<ExpressPhotoInfo>(expressPhotoId);
            if (expressPhoto == null) {
                return (string.Empty, DEFAULT_URL_TITLE);
            }

            string trackingNo = expressPhoto.TrackingNo;
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (express == null) {
                return (string.Empty, DEFAULT_URL_TITLE);
            }

            return (customerMessageExpressPhotoSignService.GetUrl(express.Id), "查看快递单");
        }

        if (messageType == CustomerMessageType.HelpMeMakeMoney) {
            return (customerMessageHelpMeMakeMoneyService.GetUrl(), DEFAULT_URL_TITLE);
        }

        if (messageType == CustomerMessageType.InternationalExpressStateUpdate) {
            int orderId = relatedId;
            return (customerMessageInternationalExpressStateUpdateService.GetUrl(orderId), "查看订单详细");
        }

        if (messageType == CustomerMessageType.InternationalExpressTrackingNo) {
            int orderId = relatedId;
            return (customerMessageInternationalExpressTrackingNoService.GetUrl(orderId), "查看订单详细");
        }

        if (messageType == CustomerMessageType.MoneyEarned) {
            return (customerMessageMoneyEarnedService.GetUrl(), "查看我的积分");
        }

        if (messageType == CustomerMessageType.NewClaim) {
            return (customerMessageNewClaimService.GetUrl(), "查看申领件");
        }

        if (messageType == CustomerMessageType.CustomerNoOrder) {
            return (customerMessageNoOrderService.GetUrl(), DEFAULT_URL_TITLE);
        }

        if (messageType == CustomerMessageType.OrderDelivered) {
            int orderId = relatedId;
            return (customerMessageOrderDeliveredService.GetUrl(orderId), DEFAULT_URL_TITLE);
        }

        if (messageType == CustomerMessageType.OrderPhotoShip) {
            int orderId = relatedId;
            return (customerMessageOrderPhotoShipService.GetUrl(orderId), "查看订单详细");
        }

        if (messageType == CustomerMessageType.Packed) {
            int orderId = relatedId;
            return (customerMessagePackedService.GetUrl(orderId), "查看订单详细");
        }

        if (messageType == CustomerMessageType.ScoreChange) {
            return (customerMessageScoreChangeService.GetUrl(), "查看我的积分");
        }

        if (messageType == CustomerMessageType.Transfered) {
            int orderId = relatedId;
            return (customerMessageTransferedService.GetUrl(orderId), "查看订单详细");
        }

        return (string.Empty, DEFAULT_URL_TITLE);
    }
}
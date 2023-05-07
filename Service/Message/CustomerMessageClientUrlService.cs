using NHibernate;
using shunda.Core.Service;
using shunda.Domain;
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

    public CustomerMessageClientUrlService(
        ISession session,
        CustomerMessageAccountChangeService customerMessageAccountChangeService,
        CustomerMessageCouponFollowService customerMessageCouponFollowService,
        CustomerMessageExpressCheckinService customerMessageExpressCheckinService,
        CustomerMessageExpressExpectEnterService customerMessageExpressExpectEnterService
    ) {
        this.session = session;
        this.customerMessageAccountChangeService = customerMessageAccountChangeService;
        this.customerMessageCouponFollowService = customerMessageCouponFollowService;
        this.customerMessageExpressCheckinService = customerMessageExpressCheckinService;
        this.customerMessageExpressExpectEnterService = customerMessageExpressExpectEnterService;
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

        return string.Empty;
    }
}
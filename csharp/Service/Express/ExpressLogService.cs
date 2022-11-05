using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressLogService
{
    private ISession session = null;

    public ExpressLogService(ISession session)
    {
        this.session = session;
    }

    public void UserClaimLog(int claimId, ExpressLogType type, string description, int userId)
    {
        Log(claimId: claimId, orderExpressId: 0, type: type, description: description, userId: userId, customerId: 0);
    }

    public void UserClaimLog(int claimId, ExpressLogType type, int userId)
    {
        Log(claimId: claimId, orderExpressId: 0, type: type, description: string.Empty, userId: userId, customerId: 0);
    }

    public void UserExpressLog(int orderExpressId, ExpressLogType type, string description, int userId)
    {
        Log(claimId: 0, orderExpressId: orderExpressId, type: type, description: description, userId: userId, customerId: 0);
    }

    public void UserExpressLog(int orderExpressId, ExpressLogType type, int userId)
    {
        Log(claimId: 0, orderExpressId: orderExpressId, type: type, description: string.Empty, userId: userId, customerId: 0);
    }

    public void ExpressLog(int orderExpressId, ExpressLogType type)
    {
        Log(claimId: 0, orderExpressId: orderExpressId, type: type, description: string.Empty, userId: 0, customerId: 0);
    }

    public void UserLog(int claimId, int orderExpressId, ExpressLogType type, string description, int userId)
    {
        Log(claimId: claimId, orderExpressId: orderExpressId, type: type, description: description, userId: userId, customerId: 0);
    }

    public void UserLog(int claimId, int orderExpressId, ExpressLogType type, int userId)
    {
        Log(claimId: claimId, orderExpressId: orderExpressId, type: type, description: string.Empty, userId: userId, customerId: 0);
    }

    public void UserLog(string trackingNo, ExpressLogType type, string description, int userId)
    {
        Log(claimId: 0, orderExpressId: 0, trackingNo: trackingNo, type: type, description: description, userId: userId, customerId: 0);
    }

    public void CustomerClaimLog(int claimId, ExpressLogType type, string description, int customerId)
    {
        Log(claimId: claimId, orderExpressId: 0, type: type, description: description, userId: 0, customerId: customerId);
    }

    public void CustomerClaimLog(int claimId, ExpressLogType type, int customerId)
    {
        Log(claimId: claimId, orderExpressId: 0, type: type, description: string.Empty, userId: 0, customerId: customerId);
    }

    public void CustomerExpressLog(int orderExpressId, ExpressLogType type, string description, int customerId)
    {
        Log(claimId: 0, orderExpressId: orderExpressId, type: type, description: description, userId: 0, customerId: customerId);
    }

    public void CustomerExpressLog(int orderExpressId, ExpressLogType type, int customerId)
    {
        Log(claimId: 0, orderExpressId: orderExpressId, type: type, description: string.Empty, userId: 0, customerId: customerId);
    }

    public void CustomerLog(int claimId, int orderExpressId, ExpressLogType type, string description, int customerId)
    {
        Log(claimId: claimId, orderExpressId: orderExpressId, type: type, description: description, userId: 0, customerId: customerId);
    }

    public void CustomerLog(int claimId, int orderExpressId, ExpressLogType type, int customerId)
    {
        Log(claimId: claimId, orderExpressId: orderExpressId, type: type, description: string.Empty, userId: 0, customerId: customerId);
    }

    public void Log(string trackingNo, ExpressLogType type, string description)
    {
        Log(claimId: 0, orderExpressId: 0, trackingNo: trackingNo, type: type, description: description, userId: 0, customerId: 0);
    }

    private void Log(int claimId, int orderExpressId, ExpressLogType type, string description, int userId, int customerId)
    {
        Log(claimId: claimId, orderExpressId: orderExpressId, trackingNo: "", type: type, description: description, userId: userId, customerId: customerId);
    }

    private void Log(int claimId, int orderExpressId, string trackingNo, ExpressLogType type, string description, int userId, int customerId)
    {
        /* todo a 完成
        ExpressLogInfo log = new ExpressLogInfo();

        if (claimId != 0)
        {
            LogisticsClaimGoodsInfo claimGoods = session.Get<LogisticsClaimGoodsInfo>(claimId);
            if (claimGoods == null)
            {
                throw new ValidationException($"指定的申领件#{claimId}不存在！");
            }

            log.Claim = claimGoods;
            log.TrackingNo = claimGoods.TrackingNo;
        }

        if (orderExpressId != 0)
        {
            LogisticsOrderGoodsExpressInfo orderExpress = session.Get<LogisticsOrderGoodsExpressInfo>(orderExpressId);
            if (orderExpress == null)
            {
                throw new ValidationException($"指定的订单快递单号#{orderExpressId}不存在！");
            }

            log.Express = orderExpress;
            log.ExpressOrder = orderExpress.Order;
            log.TrackingNo = orderExpress.ExpressTrackingNo;
        }

        if (claimId == 0 && orderExpressId == 0 && string.IsNullOrWhiteSpace(trackingNo) == false)
        {
            log.TrackingNo = trackingNo;
        }

        log.Type = type;
        log.Description = description;

        if (userId != 0)
        {
            UserInfo user = session.Get<UserInfo>(userId);
            log.User = user;
        }

        if (customerId != 0)
        {
            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            log.Customer = customer;
        }

        session.Save(log);
        session.Flush();
        */
    }
}
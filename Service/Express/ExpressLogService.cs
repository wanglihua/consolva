using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Pass;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressLogService {
    private readonly ISession session;

    public ExpressLogService(ISession session) {
        this.session = session;
    }

    public void UserLog(int expressId, ExpressLogType type, string description, int userId) {
        Log(expressId: expressId, type: type, description: description, userId: userId, customerId: 0);
    }

    public void UserLog(int expressId, ExpressLogType type, int userId) {
        Log(expressId: expressId, type: type, description: string.Empty, userId: userId, customerId: 0);
    }

    public void UserLog(string trackingNo, ExpressLogType type, string description, int userId) {
        Log(expressId: 0, trackingNo: trackingNo, type: type, description: description, userId: userId, customerId: 0);
    }

    public void CustomerLog(int expressId, ExpressLogType type, string description, int customerId) {
        Log(expressId: expressId, type: type, description: description, userId: 0, customerId: customerId);
    }

    public void CustomerLog(int expressId, ExpressLogType type, int customerId) {
        Log(expressId: expressId, type: type, description: string.Empty, userId: 0, customerId: customerId);
    }

    public void Log(int expressId, ExpressLogType type, string description) {
        Log(expressId: expressId, trackingNo: "", type: type, description: description, userId: 0, customerId: 0);
    }

    public void Log(string trackingNo, ExpressLogType type, string description) {
        Log(expressId: 0, trackingNo: trackingNo, type: type, description: description, userId: 0, customerId: 0);
    }

    private void Log(int expressId, ExpressLogType type, string description, int userId, int customerId) {
        Log(expressId: expressId, trackingNo: "", type: type, description: description, userId: userId, customerId: customerId);
    }

    private void Log(int expressId, string trackingNo, ExpressLogType type, string description, int userId, int customerId) {
        if (expressId == 0 && string.IsNullOrWhiteSpace(trackingNo) == true) {
            throw new Exception("快递单号日志expressId和trackingNo不能同时为空！");
        }

        ExpressLogInfo log = new ExpressLogInfo();

        if (expressId != 0) {
            ExpressInfo? express = session.Get<ExpressInfo>(expressId);
            if (express == null) {
                throw new ValidationException($"指定的申领件#{expressId}不存在！");
            }

            log.Express = express;
            log.Order = express.Order; // express.Order 这时可能为空，也可能不为空
            log.TrackingNo = express.TrackingNo;
        }

        if (expressId == 0 && string.IsNullOrWhiteSpace(trackingNo) == false) {
            log.TrackingNo = trackingNo;
        }

        log.Type = type;
        log.Description = description;

        if (userId != 0) {
            UserInfo? user = session.Get<UserInfo>(userId);
            log.User = user;
        }

        if (customerId != 0) {
            CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
            log.Customer = customer;
        }

        session.Save(log);
        session.Flush();
    }
}
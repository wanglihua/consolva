using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderPackBoxHistoryService {
    private readonly ISession session;

    public LogisticsOrderPackBoxHistoryService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 添加箱号历史
    /// </summary>
    public void AddHistory(LogisticsOrderPackBoxInfo orderPackBox, LogisticsOrderPackBoxHistoryType type, LogisticsOrderPackBoxStatus statusBefore, LogisticsOrderPackBoxStatus statusAfter, int userId, string description) {
        UserInfo? user = null;
        if (userId > 0) {
            user = session.Get<UserInfo>(userId);
            if (user == null) {
                throw new ValidationException($"指定的箱号历史操作用户#{userId}不存在！");
            }
        }

        LogisticsOrderPackBoxHistoryInfo packBoxHistory = new LogisticsOrderPackBoxHistoryInfo {
            Type = type,
            Order = orderPackBox.Order,
            PackBox = orderPackBox,
            BoxNum = orderPackBox.BoxNum,
            StatusBefore = statusBefore,
            StatusAfter = statusAfter,
            User = user,
            Description = description
        };

        session.Save(packBoxHistory);
        session.Flush();
    }

    /// <summary>
    /// 添加箱号历史
    /// </summary>
    public void AddHistory(LogisticsOrderPackBoxInfo orderPackBox, LogisticsOrderPackBoxHistoryType type, LogisticsOrderPackBoxStatus statusBefore, LogisticsOrderPackBoxStatus statusAfter, int userId) {
        AddHistory(orderPackBox, type, statusBefore, statusAfter, userId, string.Empty);
    }

    public void AddHistory(LogisticsOrderPackBoxInfo orderPackBox, LogisticsOrderPackBoxHistoryType type, LogisticsOrderPackBoxStatus statusBefore, LogisticsOrderPackBoxStatus statusAfter) {
        AddHistory(orderPackBox, type, statusBefore, statusAfter, 0, string.Empty);
    }
}
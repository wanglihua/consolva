using NHibernate;
using shunda.Core.Service;

namespace shunda.Service.Logistics;

/// <summary>
/// 舜达跟踪号服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ShunDaTrackingNoService {
    private readonly ISession session;

    public ShunDaTrackingNoService(ISession session) {
        this.session = session;
    }

    private string GetNextMainTrackinigNo() {
        // 这里应该在一个事务内，并且lock?
        // 暂时不用了，现在的入库速度，一般不会有并发的

        object[] seqObjs = session.CreateSQLQuery("CALL shun_da_tracking_no_next_seq()").UniqueResult<object[]>();
        if (seqObjs == null || seqObjs.Length != 3) {
            throw new Exception("获取下一个舜达跟踪号失败！");
        }

        int year = int.Parse(seqObjs[0].ToString() ?? string.Empty);
        int month = int.Parse(seqObjs[1].ToString() ?? string.Empty);
        int seq = int.Parse(seqObjs[2].ToString() ?? string.Empty);

        string yearLabel = GetYearLetter(year);
        string monthLabel = GetMonthLetter(month);

        return $"SD{seq:#00000}{yearLabel}{monthLabel}";
    }

    private string GetYearLetter(int year) {
        return ((char)((int)'A' + year - 2023)).ToString(); // 2023年 是 A
    }

    private string GetMonthLetter(int month) {
        return ((char)((int)'A' + month - 1)).ToString(); // 1月 是A
    }
}
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class CheckinTrackingNoService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly object LockObject = new object();

    private readonly ISessionFactory sessionFactory;
    private readonly ISession session;
    private readonly ExpressSizeWeightService expressSizeWeightService;

    public CheckinTrackingNoService(ISessionFactory sessionFactory, ISession session, ExpressSizeWeightService expressSizeWeightService) {
        this.sessionFactory = sessionFactory;
        this.session = session;
        this.expressSizeWeightService = expressSizeWeightService;
    }

    public string GetNextMainTrackinigNo() {
        lock (LockObject) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
        {
            int seq;

            string yearLabel;
            string monthLabel;

            ISession? sessionLocal = null;
            ITransaction? transaction = null;
            try {
                sessionLocal = HibernateSessionFactory.OpenSession(sessionFactory);
                transaction = sessionLocal.BeginTransaction();

                object[] seqObjs = sessionLocal.CreateSQLQuery("CALL checkin_tracking_no_next_seq()").UniqueResult<object[]>();
                if (seqObjs == null || seqObjs.Length != 3) {
                    throw new Exception("获取下一个入库跟踪号失败！");
                }

                int year = int.Parse(seqObjs[0].ToString() ?? string.Empty);
                int month = int.Parse(seqObjs[1].ToString() ?? string.Empty);
                seq = int.Parse(seqObjs[2].ToString() ?? string.Empty);

                yearLabel = GetYearLetter(year);
                monthLabel = GetMonthLetter(month);

                sessionLocal.Flush();
                transaction.Commit();
            }
            catch (Exception exp) {
                log.Error(exp);
                transaction?.Rollback();

                throw;
            }
            finally {
                sessionLocal?.Close();
            }

            return $"SD{seq:#00000}{yearLabel}{monthLabel}";
        }
    }

    private string GetYearLetter(int year) {
        return ((char)((int)'A' + year - 2023)).ToString(); // 2023年 是 A
    }

    private string GetMonthLetter(int month) {
        return ((char)((int)'A' + month - 1)).ToString(); // 1月 是A
    }

    // todo 1、入库的时候调用 首次入库和入库信息更新 2、新增删除更改尺寸重量的时候调用

    /// <summary>
    /// 生成入库跟踪号的具体子单号，如果一个尺寸有多箱的话，就会是多个单号
    /// 如果主单号存在，就用现成的；如果子单号不存在，就生成一个主单号
    /// 【生成或者重新生成】，如果已经有子单号，会删除后再生成
    /// 返回一个主单号
    /// </summary>
    /// <param name="expressId"></param>
    /// <returns>主单号</returns>
    public string Generate(int expressId) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        // 主单号不存在就新生成一个，如果主单号存在（重新生成入库跟踪号时），就只是获取

        var checkinTrackingNoExpressMain = session.Query<CheckinTrackingNoExpressInfo>()
            .Where(e => e.Express.Id == expressId && e.SizeWeight == null)
            .Take(1)
            .SingleOrDefault();

        if (checkinTrackingNoExpressMain == null) {
            checkinTrackingNoExpressMain = new CheckinTrackingNoExpressInfo {
                CheckinTrackingNo = GetNextMainTrackinigNo(),
                TrackingNo = express.TrackingNo,
                Express = express,
                SizeWeight = null,
                Status = CheckinTrackingNoExpressStatus.ToOnShelf
            };

            session.Save(checkinTrackingNoExpressMain);
            session.Flush();
        }

        string mainTrackingNo = checkinTrackingNoExpressMain.CheckinTrackingNo;

        if (string.IsNullOrEmpty(mainTrackingNo)) {
            throw new ValidationException("入库跟踪号主单号不能为空！");
        }

        string trackingNo = express.TrackingNo;
        // 这里排序顺序固定
        var sizeWeightList = expressSizeWeightService.GetSizeWeightList(trackingNo); // 这里的顺序是固定的，这样重新生成的时候，还是会尽量保持原来的入库跟踪号
        if (sizeWeightList == null || sizeWeightList.Count == 0) {
            throw new ValidationException($"指定的申领件#{trackingNo}没有尺寸重量记录，无法生成入库号跟踪，不能入库！");
        }

        // 删除已有的子单号
        session.Query<CheckinTrackingNoExpressInfo>()
            .Where(e => e.Express.Id == expressId && e.SizeWeight != null)
            .Delete();

        int totalPackageCount = sizeWeightList.Sum(e => e.PackageCount);
        int seq = 1;
        foreach (var sizeWeight in sizeWeightList) {
            int packageCount = sizeWeight.PackageCount;

            for (int index = 0; index < packageCount; index++) {
                string subCheckinTrackingNo = $"{mainTrackingNo}{totalPackageCount:#000}-{seq:#000}";

                var aoPinTrackingNoExpress = new CheckinTrackingNoExpressInfo {
                    CheckinTrackingNo = subCheckinTrackingNo,
                    TrackingNo = express.TrackingNo,
                    Express = express,
                    SizeWeight = sizeWeight,
                    Status = CheckinTrackingNoExpressStatus.ToOnShelf
                };

                session.Save(aoPinTrackingNoExpress);

                seq++;
            }
        }

        session.Flush();

        return mainTrackingNo;
    }

    /// <summary>
    /// 获取已生成的子单号列表
    /// LogisticsExpress 不为空的单号列表，包括 ChildExpress 为空(也可能会有多个)和不为空的数据
    /// </summary>
    /// <param name="expressId"></param>
    /// <returns></returns>
    public IList<CheckinTrackingNoExpressInfo> ListSubTrackingNo(int expressId) {
        var trackingNoExressList = session.Query<CheckinTrackingNoExpressInfo>()
            .Fetch(e => e.Express) // 打印标签是会用到里面单号
            .Where(e => e.Express.Id == expressId && e.SizeWeight != null)
            .OrderBy(e => e.SizeWeight!.TrackingNoSub) // 按子单号顺序排序
            .ToList();

        // 按照子单号的数字顺序排序
        foreach (var trackingNoExpress in trackingNoExressList) {
            string childExpressTrackingNo = trackingNoExpress.SizeWeight?.TrackingNo ?? string.Empty;
            int lastIndexOfMinus = childExpressTrackingNo?.LastIndexOf("-") ?? -1;
            if (lastIndexOfMinus == -1) {
                trackingNoExpress.SeqCache = 0;
            } else {
                if (int.TryParse(childExpressTrackingNo?.Substring(lastIndexOfMinus + 1) ?? string.Empty, out int childNum)) {
                    trackingNoExpress.SeqCache = childNum;
                } else {
                    trackingNoExpress.SeqCache = 0;
                }
            }
        }

        return trackingNoExressList.OrderBy(e => e.SeqCache).ThenBy(e => e.SizeWeight?.TrackingNo ?? string.Empty).ToList();
    }
}
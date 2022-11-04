using NHibernate;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class CheckinTrackingNoService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static readonly object LockObject = new object();

    private ISessionFactory sessionFactory;

    public CheckinTrackingNoService(ISessionFactory sessionFactory)
    {
        this.sessionFactory = sessionFactory;
    }

    public string GetNextMainTrackinigNo()
    {
        lock (LockObject) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
        {
            int seq;

            string yearLabel;
            string monthLabel;

            ISession? sessionLocal = null;
            ITransaction? transaction = null;
            try
            {
                sessionLocal = HibernateSessionFactory.OpenSession(sessionFactory);
                transaction = sessionLocal.BeginTransaction();

                object[] seqObjs = sessionLocal.CreateSQLQuery("CALL checkin_tracking_no_next_seq()").UniqueResult<object[]>();
                if (seqObjs == null || seqObjs.Length != 3)
                {
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
            catch (Exception exp)
            {
                log.Error(exp);
                transaction?.Rollback();

                throw;
            }
            finally
            {
                sessionLocal?.Close();
            }

            return $"SD{seq:#00000}{yearLabel}{monthLabel}";
        }
    }

    private string GetYearLetter(int year)
    {
        return ((char)((int)'A' + year - 2022)).ToString(); // 2022年 是 A
    }

    private string GetMonthLetter(int month)
    {
        return ((char)((int)'A' + month - 1)).ToString(); // 1月 是A
    }
}


/*

todo 其他的逻辑放在 CheckinTrackingNoExpressService 中，并且不用 注入 ISessionFactory，直接使用 ISession


using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Domain.Logistics;
using Domain.LogisticsOrder;
using NHibernate;
using NHibernate.Linq;
using Service.LogisticsOrder;

namespace Service.Logistics
{
    [Service(ServiceType.Scoped)]
    public class AoPinTrackingNoService
    {
        private ISession session = null;
        private LogisticsExpressService logisticsExpressService = null;

        public AoPinTrackingNoService(ISession session, LogisticsExpressService logisticsExpressService)
        {
            this.session = session;
            this.logisticsExpressService = logisticsExpressService;
        }

        private string GetNextMainTrackinigNo()
        {
            // 这里应该在一个事务内，并且lock?
            // 暂时不用了，现在的入库速度，一般不会有并发的
            
            object[] seqObjs = session.CreateSQLQuery("CALL ao_pin_tracking_no_next_seq()").UniqueResult<object[]>();
            if (seqObjs == null || seqObjs.Length != 3)
            {
                throw new Exception("获取下一个澳拼跟踪号失败！");
            }

            int year = int.Parse(seqObjs[0].ToString());
            int month = int.Parse(seqObjs[1].ToString());
            int seq = int.Parse(seqObjs[2].ToString());

            string yearLabel = GetYearLetter(year);
            string monthLabel = GetMonthLetter(month);

            return $"AP{seq:#00000}{yearLabel}{monthLabel}";
        }

        private string GetYearLetter(int year)
        {
            return ((char) ((int) 'A' + year - 2021)).ToString(); // 2021年 是 A
        }

        private string GetMonthLetter(int month)
        {
            return ((char) ((int) 'A' + month - 1)).ToString(); // 1月 是A
        }

        /// <summary>
        /// 生成澳拼跟踪号的具体子单号，如果一个尺寸有多箱的话，就会是多个单号
        /// 如果主单号存在，就用现成的；如果子单号不存在，就生成一个主单号
        /// 【生成或者重新生成】，如果已经有子单号，会删除后再生成
        /// 返回一个主单号
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns>主单号</returns>
        public string Generate(int claimId)
        {
            var claimGoods = session.Get<LogisticsClaimGoodsInfo>(claimId);
            if (claimGoods == null)
            {
                throw new ValidationException($"指定的申领件#{claimId}不存在！");
            }

            // 主单号不存在就新生成一个，如果主单号存在（重新生成澳拼入库号时），就只是获取

            var aoPinTrackingNoExpressMain = session.Query<AoPinTrackingNoExpressInfo>()
                .Where(e => e.Claim.Id == claimId && e.LogisticsExpress == null)
                .Take(1)
                .SingleOrDefault();

            if (aoPinTrackingNoExpressMain == null)
            {
                aoPinTrackingNoExpressMain = new AoPinTrackingNoExpressInfo();
                aoPinTrackingNoExpressMain.AoPinTrackingNo = GetNextMainTrackinigNo();
                aoPinTrackingNoExpressMain.TrackingNo = claimGoods.TrackingNo;
                aoPinTrackingNoExpressMain.Claim = claimGoods;
                aoPinTrackingNoExpressMain.LogisticsExpress = null;
                aoPinTrackingNoExpressMain.ChildExpress = null;
                aoPinTrackingNoExpressMain.Status = AoPinTrackingNoExpressStatus.ToOnShelf;

                session.Save(aoPinTrackingNoExpressMain);
                session.Flush();
            }

            string mainTrackingNo = aoPinTrackingNoExpressMain.AoPinTrackingNo;

            if (string.IsNullOrEmpty(mainTrackingNo))
            {
                throw new ValidationException("澳拼入库号主单号不能为空！");
            }

            string trackingNo = claimGoods.TrackingNo;
            // 这里排序顺序固定
            var expressItemList = logisticsExpressService.GetLogisticsExpressItemList(trackingNo); // 这里的顺序是固定的，这样重新生成的时候，还是会尽量保持原来的澳拼跟踪号
            if (expressItemList == null || expressItemList.Count == 0)
            {
                throw new ValidationException($"指定的申领件#{trackingNo}没有尺寸重量记录，无法生成澳拼入库号，不能入库！");
            }

            // 删除已有的子单号
            session.Query<AoPinTrackingNoExpressInfo>()
                .Where(e => e.Claim.Id == claimId && e.LogisticsExpress != null)
                .Delete();

            int totalQuantity = expressItemList.Sum(e => e.Quantity);
            int seq = 1;
            foreach (var expressItem in expressItemList)
            {
                int quantity = expressItem.Quantity;

                for (int index = 0; index < quantity; index++)
                {
                    string subAoPinTrackingNo = $"{mainTrackingNo}{totalQuantity:#000}-{seq:#000}";

                    var aoPinTrackingNoExpress = new AoPinTrackingNoExpressInfo();
                    aoPinTrackingNoExpress.AoPinTrackingNo = subAoPinTrackingNo;
                    aoPinTrackingNoExpress.TrackingNo = claimGoods.TrackingNo;
                    aoPinTrackingNoExpress.Claim = claimGoods;
                    aoPinTrackingNoExpress.LogisticsExpress = session.Load<LogisticsExpressInfo>(expressItem.LogisticsExpressId);
                    aoPinTrackingNoExpress.ChildExpress = null;
                    if (expressItem.ChildExpressId != null)
                    {
                        var childExpressIdValue = expressItem.ChildExpressId.Value;
                        aoPinTrackingNoExpress.ChildExpress = session.Load<LogisticsExpressChildInfo>(childExpressIdValue);
                    }
                    aoPinTrackingNoExpress.Status = AoPinTrackingNoExpressStatus.ToOnShelf;

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
        /// <param name="claimId"></param>
        /// <returns></returns>
        public IList<AoPinTrackingNoExpressInfo> ListSubTrackingNo(int claimId)
        {
            var aoPinTrackingNoExressList = session.Query<AoPinTrackingNoExpressInfo>()
                .Fetch(e => e.LogisticsExpress) // 打印标签是会用到里面单号
                .Fetch(e => e.ChildExpress) // 打印标签是会用到里面子单号，这个只会造成 left join ?
                .Where(e => e.Claim.Id == claimId && e.LogisticsExpress != null)
                // .OrderBy(e => e.Id) // 最先生成的放最前面
                .OrderBy(e => e.ChildExpress.TrackingNo) // 按子单号排序
                .ToList();

            // 按照子单号的数字顺序排序
            foreach (var aoPinTrackingNoExpress in aoPinTrackingNoExressList)
            {
                string childExpressTrackingNo = aoPinTrackingNoExpress.ChildExpress?.TrackingNo ?? string.Empty;
                int lastIndexOfMinus = childExpressTrackingNo?.LastIndexOf("-") ?? -1;
                if (lastIndexOfMinus == -1)
                {
                    aoPinTrackingNoExpress.SeqCache = 0;
                }
                else
                {
                    if (int.TryParse(childExpressTrackingNo?.Substring(lastIndexOfMinus + 1) ?? string.Empty, out int childNum))
                    {
                        aoPinTrackingNoExpress.SeqCache = childNum;
                    }
                    else
                    {
                        aoPinTrackingNoExpress.SeqCache = 0;
                    }
                }
            }

            return aoPinTrackingNoExressList.OrderBy(e => e.SeqCache).ThenBy(e => e.ChildExpress?.TrackingNo??string.Empty).ToList();
        }
    }
}

*/
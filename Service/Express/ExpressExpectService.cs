using shunda.Core.Service;
using shunda.Domain;
using NLog;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

/// <summary>
/// 快递单号预报 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressExpectService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;

    public ExpressExpectService(ISession session, ExpressTrackingNoService expressTrackingNoService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
    }

    public void Add(int customerId, string trackingNo, string description)
    {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            throw new ValidationException("快递单号不能为空！");
        }

        if (description == null)
        {
            description = string.Empty;
        }

        trackingNo = trackingNo.Trim().ToUpper();

        if (expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false)
        {
            throw new ValidationException("快递单号格式不正确或不能在快递单号中录入中文！");
        }

        var expressExpectInDb = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        // 已经存在
        if (expressExpectInDb != null)
        {
            if (expressExpectInDb.Customer.Id != customerId) // 不是本人的
            {
                throw new ValidationException($"定的快递单号预报 {trackingNo} 别的会员已经添加，请检查是否录入有误！");
            }

            //走到这里肯定是本人的了
            if (expressExpectInDb.Status == ExpressExpectStatus.Create || expressExpectInDb.Status == ExpressExpectStatus.Enter)
            {
                // 状态是新创建或者录货值的。另外一个已入库状态的，后面会检查是不是有快递单记录对应，有的话会报错；没有的话会更新已存在的预报状态
                throw new ValidationException($"指定的快递单号预报 {trackingNo} 已经存在，不能重复添加！");
            }
        }

        var express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        ExpressExpectStatus status = ExpressExpectStatus.Create;
        if (express != null)
        {
            if (express.Status == ExpressStatus.WaitPick)
            {
                status = ExpressExpectStatus.Enter;
            }
            else
            {
                if (express.Customer!.Id == customerId) // 被他人认领
                {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被他人认领，请检查是否录入有误！");
                }
                else // 自己认领的
                {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被本人认领，请检查是否录入有误！");
                }
            }
        }

        bool create = false;
        if (expressExpectInDb == null)
        {
            create = true;
            expressExpectInDb = new ExpressExpectInfo();
        }

        expressExpectInDb.Customer = customer;
        expressExpectInDb.TrackingNo = trackingNo;
        expressExpectInDb.Description = description;
        expressExpectInDb.LastEditTime = DateTime.Now;
        expressExpectInDb.Status = status;

        if (create)
        {
            session.Save(expressExpectInDb);
        }
        else
        {
            session.Update(expressExpectInDb);
        }

        session.Flush();
    }

    /// <summary>
    /// 编辑快递单号预报
    /// </summary>
    /// <param name="expressExpectId"></param>
    /// <param name="customerId">用于验证快递单号预报是不是属于当前会员</param>
    /// <param name="trackingNo"></param>
    /// <param name="description"></param>
    public void Edit(int expressExpectId, int customerId, string trackingNo, string description)
    {
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);
        if (expressExpect == null)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{customerId} ！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            throw new ValidationException("快递单号不能为空！");
        }

        trackingNo = trackingNo.Trim().ToUpper();

        if (expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false)
        {
            throw new ValidationException("快递单号格式不正确或不能在快递单号中录入中文！");
        }

        var expressExpectInDb = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        // 已经存在
        if (expressExpectInDb != null)
        {
            if (expressExpectInDb.Customer.Id != customerId) // 不是本人的
            {
                throw new ValidationException($"定的快递单号预报 {trackingNo} 别的会员已经添加，请检查是否录入有误！");
            }

            //走到这里肯定是本人的了，这里的逻辑跟添加的时候不一样些
            throw new ValidationException($"指定的快递单号预报 {trackingNo} 已经存在，不能重复添加！");
        }

        var express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        ExpressExpectStatus status = ExpressExpectStatus.Create;
        if (express != null)
        {
            if (express.Status == ExpressStatus.WaitPick)
            {
                status = ExpressExpectStatus.Enter;
            }
            else
            {
                if (express.Customer!.Id == customerId) // 被他人认领
                {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被他人认领，请检查是否录入有误！");
                }
                else // 自己认领的
                {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被本人认领，请检查是否录入有误！");
                }
            }
        }

        expressExpect.TrackingNo = trackingNo;
        expressExpect.Description = description;
        expressExpect.LastEditTime = DateTime.Now;
        expressExpect.Status = status;

        session.Update(expressExpect);
        session.Flush();
    }

    /// <summary>
    /// 设置快递单号预报已认领
    /// </summary>
    /// <param name="trackingNo">快递单号</param>
    /// <param name="customerId">用于验证快递单号预报是不是属于当前会员</param>
    public void Pick(string trackingNo, int customerId)
    {
        var expressExpect = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        if (expressExpect == null)
        {
            throw new ValidationException($"指定的快递单号预告 #{trackingNo} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId)
        {
            throw new ValidationException($"指定的快递单号预告 #{trackingNo} 不属于当前会员 #{customerId} ！");
        }

        expressExpect.Status = ExpressExpectStatus.Checkin;
        session.Update(expressExpect);
        session.Flush();
    }

    /// <summary>
    /// 删除快递单号预报
    /// </summary>
    /// <param name="expressExpectId">快递单号预报id</param>
    /// <param name="customerId">customerId 用于验证快递单号预报是不是属于当前会员</param>
    public void Delete(int expressExpectId, int customerId)
    {
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);
        if (expressExpect == null)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{customerId} ！");
        }

        session.Delete(expressExpect);
        session.Flush();
    }
}
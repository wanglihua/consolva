using FluentValidation;
using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using ValidationException = shunda.Core.ValidationException;

namespace shunda.Service.Express;

/// <summary>
/// 快递单号预报 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressExpectService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressLogService expressLogService;

    public ExpressExpectService(ISession session,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressLogService expressLogService
    ) {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    public class AddParam {
        public int CustomerId { get; set; }

        public string TrackingNo { get; set; } = string.Empty;

        public decimal GoodsValue { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    public class AddParamValidator : AbstractValidator<AddParam> {
        public AddParamValidator() {
            RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("快递单号预报添加，客户Id不能为空");
            RuleFor(e => e.TrackingNo).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("快递单号预报添加，快递单号不能为空！");
            RuleFor(e => e.GoodsValue).Must(p => p > 0).WithMessage("快递单号预报添加，物品总货值不能为空！");
            // RuleFor(e => e.Description).NotNull().WithMessage("快递单号预报添加，备注不能为空！"); // if (string.IsNullOrWhiteSpace(description)) { description = string.Empty }
        }
    }

    /// <summary>
    /// 添加快递单号预报
    /// </summary>
    /// <returns>是否通过添加预报成功认领</returns>
    public bool Add(AddParam param) {
        ValidationUtil.Validate<AddParamValidator, AddParam>(param);

        int customerId = param.CustomerId;
        string trackingNo = param.TrackingNo;
        decimal goodsValue = param.GoodsValue;
        string description = param.Description;

        trackingNo = trackingNo.Trim().ToUpper();

        if (expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false) {
            throw new ValidationException("快递单号格式不正确或不能在快递单号中录入中文！");
        }

        if (string.IsNullOrWhiteSpace(description)) {
            description = string.Empty;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        bool claim = false;

        var expressExpectInDb = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        // 已经存在
        if (expressExpectInDb != null) {
            if (expressExpectInDb.Customer.Id != customerId) // 不是本人的
            {
                throw new ValidationException($"定的快递单号预报 {trackingNo} 别的会员已经添加，请检查是否录入有误！");
            }

            throw new ValidationException($"指定的快递单号预报 {trackingNo} 已经存在，不能重复添加！");
        }

        bool create = false;
        if (expressExpectInDb == null) {
            create = true;
            expressExpectInDb = new ExpressExpectInfo();
        }

        expressExpectInDb.Customer = customer;
        expressExpectInDb.TrackingNo = trackingNo;
        expressExpectInDb.GoodsValue = goodsValue;
        expressExpectInDb.Description = description;
        expressExpectInDb.LastEditTime = DateTime.Now;
        expressExpectInDb.Status = ExpressExpectStatus.Create;

        var express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express != null) {
            // 已认领
            if (express.Customer != null && ExpressStatus.IsPacked(express.Status)) {
                // 被他人认领
                if (express.Customer!.Id != customerId) {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被他人认领，请检查是否录入有误！");
                }

                // 自己认领的
                throw new ValidationException("预告快递单号对应的快递单已存在且已被本人认领，请检查是否录入有误！");
            } else {
                // 未认领，做实际认领

                expressExpectInDb.Status = ExpressExpectStatus.Checkin;

                // express
                express.GoodsValue = expressExpectInDb.GoodsValue;
                express.ClaimTime = DateTime.Now;
                express.Status = ExpressStatus.Checkin;
                express.Customer = customer;

                session.Update(express);
                session.Flush(); // 这时是不是应该返回一个结果

                // 认领日志
                expressLogService.CustomerLog(express.Id, ExpressLogType.Checkin, "添加预报认领", customerId);

                // todo a customerMessageExpressExpectEnterService.SendMessage(expressExpect.Customer, expressExpect);

                claim = true;
            }
        }

        if (create) {
            session.Save(expressExpectInDb);
        } else {
            session.Update(expressExpectInDb);
        }

        session.Flush();

        return claim;
    }

    public class EditParam {
        public int ExpressExpectId { get; set; }

        /// <summary>
        /// 用于验证快递单号预报是不是属于当前会员
        /// </summary>
        public int CustomerId { get; set; }

        public string TrackingNo { get; set; } = string.Empty;

        public decimal GoodsValue { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    public class EditParamValidator : AbstractValidator<EditParam> {
        public EditParamValidator() {
            RuleFor(e => e.ExpressExpectId).Must(p => p > 0).WithMessage("快递单号预报编辑，快递单号预报Id不能为空");
            RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("快递单号预报编辑，客户Id不能为空");
            RuleFor(e => e.TrackingNo).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("快递单号预报编辑，快递单号不能为空！");
            RuleFor(e => e.GoodsValue).Must(p => p > 0).WithMessage("快递单号预报编辑，物品总货值不能为空！");
            // RuleFor(e => e.Description).NotNull().WithMessage("快递单号预报编辑，备注不能为空！"); // if (string.IsNullOrWhiteSpace(description)) { description = string.Empty }
        }
    }

    /// <summary>
    /// 编辑快递单号预报
    /// </summary>
    /// <returns>是否通过编辑预报成功认领</returns>
    public bool Edit(EditParam param) {
        ValidationUtil.Validate<EditParamValidator, EditParam>(param);

        int expressExpectId = param.ExpressExpectId;
        int customerId = param.CustomerId;
        string trackingNo = param.TrackingNo;
        decimal goodsValue = param.GoodsValue;
        string description = param.Description;

        trackingNo = trackingNo.Trim().ToUpper();

        if (expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false) {
            throw new ValidationException("快递单号格式不正确或不能在快递单号中录入中文！");
        }

        if (string.IsNullOrWhiteSpace(description)) {
            description = string.Empty;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);
        if (expressExpect == null) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{customerId} ！");
        }

        var expressExpectPre = session.Query<ExpressExpectInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        // 已经存在
        if (expressExpectPre != null) {
            // 不是本人的
            if (expressExpectPre.Customer.Id != customerId) {
                throw new ValidationException($"定的快递单号预报 {trackingNo} 别的会员已经添加，请检查是否录入有误！");
            }
        }

        bool claim = false;

        var express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express != null) {
            // 已认领
            if (express.Customer != null && ExpressStatus.IsPacked(express.Status)) {
                // 被他人认领
                if (express.Customer!.Id != customerId) {
                    throw new ValidationException("预告快递单号对应的快递单已存在且已被他人认领，请检查是否录入有误！");
                }

                // 自己认领的
                throw new ValidationException("预告快递单号对应的快递单已存在且已被本人认领，请检查是否录入有误！");
            } else {
                // 未认领，做实际认领

                expressExpect.Status = ExpressExpectStatus.Checkin;

                // express
                express.GoodsValue = expressExpect.GoodsValue;
                express.ClaimTime = DateTime.Now;
                express.Status = ExpressStatus.Checkin;
                express.Customer = customer;

                session.Update(express);
                session.Flush(); // 这时是不是应该返回一个结果

                // 认领日志
                expressLogService.CustomerLog(express.Id, ExpressLogType.Checkin, "编辑预报认领", customerId);

                // todo a customerMessageExpressExpectEnterService.SendMessage(expressExpect.Customer, expressExpect);

                claim = true;
            }
        }

        expressExpect.TrackingNo = trackingNo;
        expressExpect.GoodsValue = goodsValue;
        expressExpect.Description = description;
        expressExpect.LastEditTime = DateTime.Now;
        expressExpect.Status = ExpressExpectStatus.Create;

        session.Update(expressExpect);
        session.Flush();

        return claim;
    }

    /// <summary>
    /// 设置快递单号预报已认领
    /// </summary>
    /// <param name="expressExpectId">快递单号预报ID</param>
    /// <param name="customerId">用于验证快递单号预报是不是属于当前会员</param>
    public void Pick(int expressExpectId, int customerId) {
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);
        if (expressExpect == null) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{customerId} ！");
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
    public void Delete(int expressExpectId, int customerId) {
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);
        if (expressExpect == null) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != customerId) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{customerId} ！");
        }

        session.Delete(expressExpect);
        session.Flush();
    }
}
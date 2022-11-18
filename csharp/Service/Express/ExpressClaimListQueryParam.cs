using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Service.Express;

public class ExpressClaimListQueryParam : ListQueryParam
{
    /// <summary>
    /// 申领件ID列表
    /// 用逗号,隔开的
    /// </summary>
    public string? IdList { get; set; }

    public int WarehouseIdQuery { get; set; }

    /// <summary>
    /// 订单目的仓
    /// </summary>
    public int OrderTargetWarehouseIdQuery { get; set; }

    /// <summary>
    /// 头程物流Id
    /// </summary>
    public int FirstModeIdQuery { get; set; }

    /// <summary>
    /// 是否对接TLA查询
    /// </summary>
    public bool? SyncTlaQuery { get; set; }

    /// <summary>
    /// 是否打印澳拼标签
    /// </summary>
    public bool? PackPrintQuery { get; set; }

    public string? AoPinTrackingNoQuery { get; set; } // todo a 改成 CheckinTrackingNoQuery

    public string? TrackingNoQuery { get; set; }

    /// <summary>
    /// 面单客户查询
    /// </summary>
    public string? BillCustomerCodeQuery { get; set; }

    /// <summary>
    /// 面单客户ID是否解析成功
    /// </summary>
    public bool? HasBillCustomerIdQuery { get; set; }

    /// <summary>
    /// 认领客户查询
    /// </summary>
    public string? CustomerCodeQuery { get; set; }

    public string? ExpressCompanyQuery { get; set; }

    [ModelBinder(typeof(ExpressStatusModelBinder))]
    public ExpressStatus? StatusQuery { get; set; }

    [ModelBinder(typeof(ExpressShelfStatusModelBinder))]
    public ExpressShelfStatus? ShelfStatusQuery { get; set; }

    public string? GoodsNameQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public IList<int>? ScheduleShelfIdListQuery { get; set; }

    public IList<int>? ScheduleUserIdListQuery { get; set; }

    // public ExpressShelfScheduleType ScheduleType { get; set; }

    /// <summary>
    /// 客户认领时间开始
    /// </summary>
    public DateTime? PickTimeBeginQuery { get; set; }

    /// <summary>
    /// 客户认领时间结束
    /// </summary>
    public DateTime? PickTimeEndQuery { get; set; }

    /// <summary>
    /// 入库登记日期开始
    /// </summary>
    public DateTime? AddDateBeginQuery { get; set; }

    /// <summary>
    /// 入库登记日期结束
    /// </summary>
    public DateTime? AddDateEndQuery { get; set; }

    /// <summary>
    /// 入库登记时间开始，是精确的时间
    /// </summary>
    public DateTime? AddTimeBeginQuery { get; set; }

    /// <summary>
    /// 入库登记时间结束，是精确的时间
    /// </summary>
    public DateTime? AddTimeEndQuery { get; set; }

    /// <summary>
    /// 上架时间
    /// </summary>
    public DateTime? OnShelfTimeBeginQuery { get; set; }

    public DateTime? OnShelfTimeEndQuery { get; set; }

    /// <summary>
    /// 看上下文，如果是上架的，就是上架排班时间；如果是下架的，就是下架排班时间
    /// </summary>
    public DateTime? ScheduleTimeBeginQuery { get; set; }

    public DateTime? ScheduleTimeEndQuery { get; set; }

    /// <summary>
    /// 堆存时间
    /// </summary>
    public DateTime? DumpTimeBeginQuery { get; set; }

    public DateTime? DumpTimeEndQuery { get; set; }

    /// <summary>
    /// 提交打包时间起始，下架排班时使用
    /// </summary>
    public DateTime? SubmitPackTimeBeginQuery { get; set; }

    /// <summary>
    /// 提交打包时间截止，下架排班时使用
    /// </summary>
    public DateTime? SubmitPackTimeEndQuery { get; set; }

    /// <summary>
    /// 是否已预报
    /// </summary>
    public bool? ExpectedQuery { get; set; }

    /// <summary>
    /// 归档类型
    /// </summary>
    [ModelBinder(typeof(ExpressArchiveTypeModelBinder))]
    public ExpressArchiveType? ArchiveTypeQuery { get; set; }

    /// <summary>
    /// 入库人员查询
    /// </summary>
    public int? CheckinUserIdQuery { get; set; }
}
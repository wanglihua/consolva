using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderModel {
    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    [BindNever]
    // todo a OrderTotalExpressPackageCount 马上修改
    public int OrderTotalPackageCount { get; set; }

    // todo GoodsTypeCommonExpressPackageCount
    [BindNever]
    public int GoodsTypeCommonPackageCount { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    [BindNever]
    public IDictionary<LogisticsGoodsType, bool> GoodsTypePackCompleteDict { get; set; } = new Dictionary<LogisticsGoodsType, bool>();

    [BindNever]
    public bool IsOrderPackComplete { get; set; }

    /// <summary>
    /// 打包排班打包工
    /// </summary>
    [BindNever]
    public string PackScheduleUserName { get; set; } = string.Empty;

    [BindNever]
    public int RemarkTotalCount { get; set; }

    /// <summary>
    /// 当前登录的是不是管理员
    /// </summary>
    [BindNever]
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 是否允许快递单号手工输入
    /// </summary>
    [BindNever]
    public bool TrackingNoManualInputEnabled { get; set; }

#if DEBUG
    public bool Debug { get; } = true;
#else
        public bool Debug { get; } = false;
#endif
}
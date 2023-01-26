using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderModel {
    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    [BindNever]
    public int OrderTotalPackageCount { get; set; }

    [BindNever]
    public int GoodsTypeCommonPackageCount { get; set; }

    [BindNever]
    public int GoodsTypeRestrictPackageCount { get; set; }

    [BindNever]
    public int GoodsTypeBambooPackageCount { get; set; }

    [BindNever]
    public int GoodsTypeRestrictBambooPackageCount { get; set; }

    [BindNever]
    public int GoodsTypeSpecialPackageCount { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    public IDictionary<LogisticsGoodsType, bool> GoodsTypePackCompleteDict { get; set; } = new Dictionary<LogisticsGoodsType, bool>();

    public bool IsOrderPackComplete { get; set; }

    [BindNever]
    public IList<LogisticsOrderRemarkInfo> LogisticsOrderRemarkList { get; set; } = new List<LogisticsOrderRemarkInfo>();

    /// <summary>
    /// 打包排班打包工
    /// </summary>
    public string PackScheduleUserName { get; set; } = string.Empty;

    [BindNever]
    public bool IsOrderExpressWeightFitMode { get; set; } = true;

    /// <summary>
    /// 当前登录的是不是管理员
    /// </summary>
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 能不能手工打印PCA标签
    /// </summary>
    public bool CanPrintPcaLabel { get; set; }

    /// <summary>
    /// 能不能手工打印 TransDirect 标签
    /// </summary>
    public bool CanPrintTransDirectLabel { get; set; }

    /// <summary>
    /// 是不是中澳海运订单
    /// </summary>
    public bool IsCnAuSeaOrder { get; set; } = true;

    /// <summary>
    /// 是否允许快递单号手工输入
    /// </summary>
    public bool TrackingNoManualInputEnabled { get; set; }

    /// <summary>
    /// 打木架
    /// </summary>
    public bool WoodFrame { get; set; } = false;

    /// <summary>
    /// 清理树皮杂物
    /// </summary>        
    public bool BarkClean { get; set; } = false;

    /// <summary>
    /// 是否有未完成的打木架
    /// </summary>
    public bool WoodFrameNotDone { get; set; } = false;

    /// <summary>
    /// 是否有未完成的清理树皮杂物
    /// </summary>        
    public bool BarkCleanNotDone { get; set; } = false;


    public LogisticsOrderPackPrintType PrintTypeInLogisticsMode { get; set; } = LogisticsOrderPackPrintType.None;

#if DEBUG
    public bool Debug { get; } = true;
#else
        public bool Debug { get; } = false;
#endif
}
using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderActionType : CodeTypeBase
{
    /// <summary>
    /// 客户创建订单
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerCreateOrder = new LogisticsOrderActionType(1000, "客户创建订单", "客户创建订单");

    /// <summary>
    /// 导入创建订单
    /// </summary>
    public static readonly LogisticsOrderActionType ImportCreateOrder = new LogisticsOrderActionType(2000, "导入创建订单", "导入创建订单");

    /// <summary>
    /// 客户添加快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerAddExpress = new LogisticsOrderActionType(3000, "客户添加快递单号", "客户添加快递单号");

    /// <summary>
    /// 客户修改快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerModifyExpress = new LogisticsOrderActionType(4000, "客户修改快递单号", "客户修改快递单号");

    /// <summary>
    /// 客户删除快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerDeleteExpress = new LogisticsOrderActionType(5000, "客户删除快递单号", "客户删除快递单号");

    /// <summary>
    /// 客户修改订单
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerModifyOrder = new LogisticsOrderActionType(6000, "客户修改订单", "客户修改订单");

    /// <summary>
    /// 后台代理客户处理问题件
    /// </summary>
    public static readonly LogisticsOrderActionType UserResolveIssueGoodsForCustomer = new LogisticsOrderActionType(8000, "后台代理客户处理问题件", "后台代理客户处理问题件");

    /// <summary>
    /// 客户作废订单 
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerCancelOrder = new LogisticsOrderActionType(9000, "客户作废订单", "客户作废订单");

    /// <summary>
    /// 后台作废订单
    /// </summary>
    public static readonly LogisticsOrderActionType UserCancelOrder = new LogisticsOrderActionType(10000, "后台作废订单", "后台作废订单");

    /// <summary>
    /// 后台修改订单收货地址
    /// </summary>
    public static readonly LogisticsOrderActionType UserChangeOrderReceiver = new LogisticsOrderActionType(11000, "后台修改订单收货地址", "后台修改订单收货地址");

    /// <summary>
    /// 后台修改订单物流
    /// </summary>
    public static readonly LogisticsOrderActionType UserChangeLogisticsMode = new LogisticsOrderActionType(12000, "后台修改订单物流", "后台修改订单物流");

    /// <summary>
    /// 后台导入快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType UserImportOrderExpress = new LogisticsOrderActionType(13000, "后台导入快递单号", "后台导入快递单号");

    /// <summary>
    /// 后台添加订单物品
    /// </summary>
    public static readonly LogisticsOrderActionType UserAddOrderGoods = new LogisticsOrderActionType(14000, "后台添加订单物品", "后台添加订单物品");

    /// <summary>
    /// 后台修改订单物品
    /// </summary>
    public static readonly LogisticsOrderActionType UserModifyOrderGoods = new LogisticsOrderActionType(15000, "后台修改订单物品", "后台修改订单物品");

    /// <summary>
    /// 后台删除订单物品
    /// </summary>
    public static readonly LogisticsOrderActionType UserDeleteOrderGoods = new LogisticsOrderActionType(16000, "后台删除订单物品", "后台删除订单物品");

    /// <summary>
    /// 后台添加快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType UserAddOrderExpress = new LogisticsOrderActionType(17000, "后台添加快递单号", "后台添加快递单号");

    /// <summary>
    /// 后台修改快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType UserModifyOrderExpress = new LogisticsOrderActionType(18000, "后台修改快递单号", "后台修改快递单号");

    /// <summary>
    /// 后台删除快递单号
    /// </summary>
    public static readonly LogisticsOrderActionType UserDeleteOrderExpress = new LogisticsOrderActionType(19000, "后台删除快递单号", "后台删除快递单号");

    /// <summary>
    /// 后台修改物品单价
    /// </summary>
    public static readonly LogisticsOrderActionType UserModifyOrderGoodsPrice = new LogisticsOrderActionType(20000, "后台修改物品单价", "后台修改物品单价");

    /// <summary>
    /// 客户提交打包
    /// </summary>
    public static readonly LogisticsOrderActionType CustomerSubmitPack = new LogisticsOrderActionType(21000, "客户提交打包", "客户提交打包");

    /// <summary>
    /// 自动提交打包
    /// </summary>
    public static readonly LogisticsOrderActionType AutoSubmitPack = new LogisticsOrderActionType(22000, "自动提交打包", "自动提交打包");

    /// <summary>
    /// 取消打包提交
    /// </summary>
    public static readonly LogisticsOrderActionType RollbackSubmitPack = new LogisticsOrderActionType(23000, "取消打包提交", "取消打包提交");

    /// <summary>
    /// 订单支付
    /// </summary>
    public static readonly LogisticsOrderActionType Pay = new LogisticsOrderActionType(24000, "订单支付", "订单支付");

    /// <summary>
    /// 手动增加费用
    /// </summary>
    public static readonly LogisticsOrderActionType ManualIncreaseFee = new LogisticsOrderActionType(25000, "手动增加费用", "手动增加费用");

    /// <summary>
    /// 手动减少费用
    /// </summary>
    public static readonly LogisticsOrderActionType ManualDecreaseFee = new LogisticsOrderActionType(26000, "手动减少费用", "手动减少费用");

    /// <summary>
    /// 核销费用
    /// </summary>
    public static readonly LogisticsOrderActionType WriteOffFee = new LogisticsOrderActionType(27000, "核销费用", "核销费用");

    /// <summary>
    /// 快递单号问题件
    /// </summary>
    public static readonly LogisticsOrderActionType ExpressIssue = new LogisticsOrderActionType(28000, "快递单号问题件", "快递单号问题件");

    /// <summary>
    /// 问题件强制删除
    /// </summary>
    public static readonly LogisticsOrderActionType ExpressIssueForceDelete = new LogisticsOrderActionType(29000, "问题件强制删除", "问题件强制删除");

    /// <summary>
    /// 快递入流动仓
    /// </summary>
    public static readonly LogisticsOrderActionType ExpressPacking = new LogisticsOrderActionType(30000, "快递入流动仓", "快递入流动仓");

    /// <summary>
    /// 订单打包
    /// </summary>
    public static readonly LogisticsOrderActionType Pack = new LogisticsOrderActionType(31000, "订单打包", "订单打包");


    /// <summary>
    /// 装箱扫码
    /// </summary>
    public static readonly LogisticsOrderActionType PackLoad = new LogisticsOrderActionType(32000, "装箱扫码", "装箱扫码");

    /// <summary>
    /// 装箱扫码完成
    /// </summary>
    public static readonly LogisticsOrderActionType PackLoadComplete = new LogisticsOrderActionType(33000, "装箱扫码完成", "装箱扫码完成");

    /// <summary>
    /// 人工录箱号装箱
    /// </summary>
    public static readonly LogisticsOrderActionType PackLoadManual = new LogisticsOrderActionType(34000, "装箱扫码", "装箱扫码");

    /// <summary>
    /// 人工录箱号装箱完成
    /// </summary>
    public static readonly LogisticsOrderActionType PackLoadManualComplete = new LogisticsOrderActionType(35000, "人工录箱号装箱完成", "人工录箱号装箱完成");

    /// <summary>
    /// 订单转运
    /// </summary>
    public static readonly LogisticsOrderActionType Transfer = new LogisticsOrderActionType(36000, "订单转运", "订单转运");

    /// <summary>
    /// 空运订单出库
    /// </summary>
    public static readonly LogisticsOrderActionType AirOrderCheckout = new LogisticsOrderActionType(37000, "空运订单出库", "空运订单出库");

    /// <summary>
    /// 空运订单导入国际跟踪单号 
    /// </summary>
    public static readonly LogisticsOrderActionType OrderAirImportExpress = new LogisticsOrderActionType(38000, "导入国际跟踪单号", "导入国际跟踪单号");

    /// <summary>
    /// 订单盘点
    /// </summary>
    public static readonly LogisticsOrderActionType OrderStockCheck = new LogisticsOrderActionType(39000, "订单盘点", "订单盘点");

    /// <summary>
    /// 手动完成空运订单
    /// </summary>
    public static readonly LogisticsOrderActionType AirOrderManualComplete = new LogisticsOrderActionType(40000, "手动完成空运订单", "手动完成空运订单");

    /// <summary>
    /// 箱号交付
    /// </summary>
    public static readonly LogisticsOrderActionType PackBoxPickup = new LogisticsOrderActionType(41000, "箱号交付", "箱号交付");

    /// <summary>
    /// 箱号人工录箱号交付
    /// </summary>
    public static readonly LogisticsOrderActionType PackBoxManualPickup = new LogisticsOrderActionType(42000, "箱号人工录箱号交付", "箱号人工录箱号交付");

    /// <summary>
    /// 订单投递成功
    /// </summary>
    public static readonly LogisticsOrderActionType DeliverComplete = new LogisticsOrderActionType(88000, "订单投递成功", "订单投递成功");

    /// <summary>
    /// 订单超期关闭
    /// </summary>
    public static readonly LogisticsOrderActionType OutDateComplete = new LogisticsOrderActionType(89000, "订单超期关闭", "订单超期关闭");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderActionType None = new LogisticsOrderActionType(0, "None", "None");

    private static readonly IReadOnlyList<LogisticsOrderActionType> codeTypeList = new List<LogisticsOrderActionType>() {
        CustomerCreateOrder,
        ImportCreateOrder,
        CustomerAddExpress,
        CustomerModifyExpress,
        CustomerDeleteExpress,
        CustomerModifyOrder,
        UserResolveIssueGoodsForCustomer,
        CustomerCancelOrder,
        UserCancelOrder,
        UserChangeOrderReceiver,
        UserChangeLogisticsMode,
        UserImportOrderExpress,
        UserAddOrderGoods,
        UserModifyOrderGoods,
        UserDeleteOrderGoods,
        UserAddOrderExpress,
        UserModifyOrderExpress,
        UserDeleteOrderExpress,
        UserModifyOrderGoodsPrice,
        CustomerSubmitPack,
        AutoSubmitPack,
        RollbackSubmitPack,
        Pay,
        ManualIncreaseFee,
        ManualDecreaseFee,
        WriteOffFee,
        ExpressIssue,
        ExpressIssueForceDelete,
        ExpressPacking,
        Pack,
        PackLoad,
        PackLoadComplete,
        PackLoadManual,
        PackLoadManualComplete,
        Transfer,
        AirOrderCheckout,
        OrderAirImportExpress,
        OrderStockCheck,
        AirOrderManualComplete,
        PackBoxPickup,
        PackBoxManualPickup,
        DeliverComplete,
        OutDateComplete,
    };


    private static readonly Dictionary<int, LogisticsOrderActionType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderActionType GetById(int typeId)
    {
        if (codeTypeMap.TryGetValue(typeId, out LogisticsOrderActionType? actionType))
        {
            return actionType;
        }

        return None;
    }

    public static IList<LogisticsOrderActionType> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsOrderActionType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderActionTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderActionType>
{
    protected override LogisticsOrderActionType GetCodeTypeById(int id)
    {
        return LogisticsOrderActionType.GetById(id);
    }
}

public class LogisticsOrderActionTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderActionType>
{
    protected override LogisticsOrderActionType GetCodeTypeById(int id)
    {
        return LogisticsOrderActionType.GetById(id);
    }
}
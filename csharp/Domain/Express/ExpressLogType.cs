using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单号日志类型
/// todo d 这里的许多类型实际一个不需要，发布时到时清理一下
/// </summary>
public class ExpressLogType : CodeTypeBase
{
    /// <summary>
    /// 入库
    /// </summary>
    public static readonly ExpressLogType Checkin = new ExpressLogType(1000, "入库", "入库");

    /// <summary>
    /// 入库修改
    /// </summary>
    public static readonly ExpressLogType CheckinEdit = new ExpressLogType(2000, "入库修改", "入库信息修改");

    /// <summary>
    /// 手工打印入库标签
    /// </summary>
    public static readonly ExpressLogType ManualPrintLabel = new ExpressLogType(3000, "手工打印入库标签", "手工打印入库标签");

    /// <summary>
    /// 子单号上架准备
    /// </summary>
    public static readonly ExpressLogType SubOnShelfPrepare = new ExpressLogType(4000, "子单号上架准备", "子单号上架准备");

    /// <summary>
    /// 上架准备
    /// </summary>
    public static readonly ExpressLogType OnShelfPrepare = new ExpressLogType(5000, "上架准备", "上架准备");

    /// <summary>
    /// 子单号上架确认
    /// </summary>
    public static readonly ExpressLogType SubOnShelfConfirm = new ExpressLogType(6000, "子单号上架确认", "子单号上架确认");

    /// <summary>
    /// 上架确认
    /// </summary>
    public static readonly ExpressLogType OnShelfConfirm = new ExpressLogType(7000, "上架确认", "上架确认");

    /// <summary>
    /// 认领
    /// </summary>
    public static readonly ExpressLogType Pick = new ExpressLogType(8000, "认领", "认领");

    /// <summary>
    /// 删除
    /// </summary>
    public static readonly ExpressLogType Delete = new ExpressLogType(9000, "删除", "删除");

    /// <summary>
    /// 设置已丢弃
    /// </summary>
    public static readonly ExpressLogType ThrowAway = new ExpressLogType(10000, "设置已丢弃", "设置已丢弃");

    /// <summary>
    /// 图片上传
    /// </summary>
    public static readonly ExpressLogType PhotoUpload = new ExpressLogType(11000, "图片上传", "图片上传");

    /// <summary>
    /// 视频上传
    /// </summary>
    public static readonly ExpressLogType VideoUpload = new ExpressLogType(12000, "视频上传", "视频上传");

    /// <summary>
    /// 尺寸重量添加
    /// </summary>
    public static readonly ExpressLogType SizeWeightAdd = new ExpressLogType(13000, "尺寸重量添加", "尺寸重量添加");

    /// <summary>
    /// 尺寸重量编辑
    /// </summary>
    public static readonly ExpressLogType SizeWeightEdit = new ExpressLogType(14000, "尺寸重量编辑", "尺寸重量编辑");

    /// <summary>
    /// 尺寸重量删除
    /// </summary>
    public static readonly ExpressLogType SizeWeightDelete = new ExpressLogType(15000, "尺寸重量删除", "尺寸重量删除");

    /// <summary>
    /// 创建订单
    /// </summary>
    public static readonly ExpressLogType Order = new ExpressLogType(16000, "创建订单", "创建订单");

    /// <summary>
    /// 取消订单
    /// </summary>
    public static readonly ExpressLogType CancelOrder = new ExpressLogType(17000, "取消订单", "取消订单");

    /// <summary>
    /// 提交打包
    /// </summary>
    public static readonly ExpressLogType SubmitPack = new ExpressLogType(18000, "提交打包", "提交打包");

    /// <summary>
    /// 子单号下架
    /// </summary>
    public static readonly ExpressLogType SubOffShelf = new ExpressLogType(19000, "子单号下架", "子单号下架");

    /// <summary>
    /// 下架
    /// </summary>
    public static readonly ExpressLogType OffShelf = new ExpressLogType(20000, "下架", "下架");

    /// <summary>
    /// 退运申请
    /// </summary>
    public static readonly ExpressLogType ReturnApply = new ExpressLogType(21000, "退运申请", "退运申请");

    /// <summary>
    /// 取消退运
    /// </summary>
    public static readonly ExpressLogType CancelReturn = new ExpressLogType(22000, "取消退运", "取消退运");

    /// <summary>
    /// 设置已退运
    /// </summary>
    public static readonly ExpressLogType MarkReturned = new ExpressLogType(23000, "设置已退运", "设置已退运");

    /// <summary>
    /// 子单号入流动仓
    /// </summary>
    public static readonly ExpressLogType SubPacking = new ExpressLogType(24000, "子单号入流动仓", "子单号入流动仓");

    /// <summary>
    /// 入流动仓
    /// </summary>
    public static readonly ExpressLogType Packing = new ExpressLogType(25000, "入流动仓", "入流动仓");

    /// <summary>
    /// 子单号打包
    /// </summary>
    public static readonly ExpressLogType SubPack = new ExpressLogType(26000, "子单号打包", "子单号打包");

    /// <summary>
    /// 打包
    /// </summary>
    public static readonly ExpressLogType Pack = new ExpressLogType(27000, "打包", "打包");

    /// <summary>
    /// 自动打包
    /// </summary>
    public static readonly ExpressLogType AutoPack = new ExpressLogType(28000, "自动打包", "自动打包");

    /// <summary>
    /// 取消打包
    /// </summary>
    public static readonly ExpressLogType RemovePack = new ExpressLogType(29000, "取消打包", "取消打包");


    /// <summary>
    /// 装箱
    /// </summary>
    public static readonly ExpressLogType Load = new ExpressLogType(30000, "装箱", "装箱");


    /// <summary>
    /// 卸货
    /// </summary>
    public static readonly ExpressLogType Unload = new ExpressLogType(31000, "卸货", "卸货");


    /// <summary>
    /// 打印外箱标签
    /// </summary>
    public static readonly ExpressLogType PrintLabel = new ExpressLogType(32000, "打印外箱标签", "打印外箱标签");

    /// <summary>
    /// 长期未认领子单号堆存
    /// </summary>
    public static readonly ExpressLogType NoPickupPartDump = new ExpressLogType(33000, "长期未认领子单号堆存", "长期未认领子单号堆存");

    /// <summary>
    /// 长期未认领堆存
    /// </summary>
    public static readonly ExpressLogType NoPickupDump = new ExpressLogType(34000, "长期未认领堆存", "长期未认领堆存");

    /// <summary>
    /// 长期未处理子单号堆存
    /// </summary>
    public static readonly ExpressLogType NoProcessPartDump = new ExpressLogType(35000, "长期未处理子单号堆存", "长期未处理子单号堆存");

    /// <summary>
    /// 长期未处理堆存
    /// </summary>
    public static readonly ExpressLogType NoProcessDump = new ExpressLogType(36000, "长期未处理堆存", "长期未处理堆存");

    /// <summary>
    /// 退运下架
    /// </summary>
    public static readonly ExpressLogType ReturnOffShelf = new ExpressLogType(37000, "退运下架", "退运下架");

    /// <summary>
    /// 手工设置打木架
    /// </summary>
    public static readonly ExpressLogType ManualWoodFrame = new ExpressLogType(38000, "手工设置打木架", "手工设置打木架");

    /// <summary>
    /// 客户取消打木架
    /// </summary>
    public static readonly ExpressLogType CustomerCancelWoodFrame = new ExpressLogType(39000, "客户取消打木架", "客户取消打木架");

    /// <summary>
    /// 客户选择打木架
    /// </summary>
    public static readonly ExpressLogType CustomerSelectWoodFrame = new ExpressLogType(40000, "客户选择打木架", "客户选择打木架");

    /// <summary>
    /// 完成打木架
    /// </summary>
    public static readonly ExpressLogType WoodFrameDone = new ExpressLogType(41000, "完成打木架", "完成打木架");

    /// <summary>
    /// 手工设置清理树皮杂物
    /// </summary>
    public static readonly ExpressLogType ManualBarkClean = new ExpressLogType(42000, "手工设置清理树皮杂物", "手工设置清理树皮杂物");

    /// <summary>
    /// 客户取消清理树皮杂物
    /// </summary>
    public static readonly ExpressLogType CustomerCancelBarkClean = new ExpressLogType(43000, "客户取消清理树皮杂物", "客户取消清理树皮杂物");

    /// <summary>
    /// 客户选择清理树皮杂物
    /// </summary>
    public static readonly ExpressLogType CustomerSelectBarkClean = new ExpressLogType(44000, "客户选择清理树皮杂物", "客户选择清理树皮杂物");

    /// <summary>
    /// 完成清理树皮杂物
    /// </summary>
    public static readonly ExpressLogType BarkCleanDone = new ExpressLogType(45000, "完成清理树皮杂物", "完成清理树皮杂物");

    /// <summary>
    /// 客户忽略申领件
    /// </summary>
    public static readonly ExpressLogType CustomerIgnoreClaim = new ExpressLogType(46000, "客户忽略申领件", "客户忽略申领件");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressLogType None = new ExpressLogType(0, "", "");

    private static readonly List<ExpressLogType> codeTypeList = new List<ExpressLogType>() {
        Checkin,
        CheckinEdit,
        ManualPrintLabel,
        SubOnShelfPrepare,
        OnShelfPrepare,
        SubOnShelfConfirm,
        OnShelfConfirm,
        Pick,
        Delete,
        ThrowAway,

        PhotoUpload,
        VideoUpload,

        SizeWeightAdd,
        SizeWeightEdit,
        SizeWeightDelete,

        Order,
        CancelOrder,
        SubmitPack,
        SubOffShelf,
        OffShelf,
        ReturnApply,
        CancelReturn,
        MarkReturned,
        SubPacking,
        Packing,
        SubPack,
        Pack,
        AutoPack,
        RemovePack,
        Load,
        Unload,
        PrintLabel,

        NoPickupPartDump,
        NoPickupDump,
        NoProcessPartDump,
        NoProcessDump,

        ReturnOffShelf,

        ManualWoodFrame,
        CustomerCancelWoodFrame,
        CustomerSelectWoodFrame,
        WoodFrameDone,
        ManualBarkClean,
        CustomerCancelBarkClean,
        CustomerSelectBarkClean,
        BarkCleanDone,

        CustomerIgnoreClaim,
    };

    private static readonly Dictionary<int, ExpressLogType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressLogType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressLogType? logType) == true)
        {
            return logType;
        }

        return None;
    }

    public static IList<ExpressLogType> GetList()
    {
        return codeTypeList;
    }

    private ExpressLogType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressLogTypeHibernateType : CodeHibernateTypeBase<ExpressLogType>
{
    protected override ExpressLogType GetCodeTypeById(int id)
    {
        return ExpressLogType.GetById(id);
    }
}

public class ExpressLogTypeModelBinder : CodeTypeModelBinderBase<ExpressLogType>
{
    protected override ExpressLogType GetCodeTypeById(int id)
    {
        return ExpressLogType.GetById(id);
    }
}
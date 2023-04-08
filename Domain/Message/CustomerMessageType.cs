using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息类型
/// </summary>
public class CustomerMessageType : CodeTypeBase {
    /// <summary>
    /// 手动推送消息
    /// </summary>
    public static readonly CustomerMessageType Manual = new CustomerMessageType(1000, "手动推送", "手动推送消息");

    /// <summary>
    /// 全员推送消息
    /// </summary>
    public static readonly CustomerMessageType AllCustomer = new CustomerMessageType(2000, "全员推送", "全员推送消息");

    /// <summary>
    /// 定时提醒，帮我赚钱提醒
    /// </summary>
    public static readonly CustomerMessageType HelpMeMakeMoney = new CustomerMessageType(3000, "帮我赚钱提醒", "帮我赚钱提醒");

    /// <summary>
    /// 定时提醒，已注册未下单推送
    /// </summary>
    public static readonly CustomerMessageType CustomerNoOrder = new CustomerMessageType(4000, "已注册未下单推送", "已注册未下单推送");

    /// <summary>
    /// 实时提醒，新人大礼包
    /// </summary>
    public static readonly CustomerMessageType CouponFollow = new CustomerMessageType(5000, "新人大礼包", "新人大礼包");

    /// <summary>
    /// 定时提醒，公共申领件处理提醒
    /// </summary>
    public static readonly CustomerMessageType NewClaim = new CustomerMessageType(6000, "公共申领件处理提醒", "公共申领件处理提醒");

    /// <summary>
    /// 实时提醒，我赚到的钱提醒
    /// </summary>
    public static readonly CustomerMessageType MoneyEarned = new CustomerMessageType(7000, "我赚到的钱提醒", "我赚到的钱提醒");

    /// <summary>
    /// 实时提醒，快递单入库提醒
    /// </summary>
    public static readonly CustomerMessageType ExpressCheckin = new CustomerMessageType(8000, "快递单入库提醒", "快递单入库提醒");

    /// <summary>
    /// 实时提醒，预报快递单入库提醒
    /// </summary>
    public static readonly CustomerMessageType ExpressExpectEnter = new CustomerMessageType(9000, "预报快递单入库提醒", "预报快递单入库提醒");

    /// <summary>
    /// 实时提醒，已打包提醒
    /// </summary>
    public static readonly CustomerMessageType Packed = new CustomerMessageType(11000, "已打包提醒", "已打包提醒");

    /// <summary>
    /// 实时提醒，已转运提醒
    /// </summary>
    public static readonly CustomerMessageType Transfered = new CustomerMessageType(13000, "已转运提醒", "已转运提醒");

    /// <summary>
    /// VIP升级推送
    /// </summary>
    public static readonly CustomerMessageType VipUpgrade = new CustomerMessageType(14000, "VIP升级推送", "VIP升级推送");

    /// <summary>
    /// 快递单签收图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoSign = new CustomerMessageType(16000, "签收图片推送", "签收图片推送");

    /// <summary>
    /// 快递单入库图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoCheckin = new CustomerMessageType(17000, "入库图片推送", "入库图片推送");

    /// <summary>
    /// 快递单入库视频推送
    /// </summary>
    public static readonly CustomerMessageType ExpressVideoCheckin = new CustomerMessageType(18000, "入库视频推送", "入库视频推送");

    /// <summary>
    /// 订单问题图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoIssue = new CustomerMessageType(19000, "问题图片推送", "问题图片推送");

    /// <summary>
    /// 订单中国损毁图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoCnDamage = new CustomerMessageType(20000, "中国损毁图片推送", "中国损毁图片推送");

    /// <summary>
    /// 退运图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoReturn = new CustomerMessageType(21000, "退运图片推送", "退运图片推送");

    /// <summary>
    /// 订单发货图片推送
    /// </summary>
    public static readonly CustomerMessageType OrderPhotoShip = new CustomerMessageType(22000, "订单发货图片推送", "订单发货图片推送");

    /// <summary>
    /// 账户余额变动提醒
    /// </summary>
    public static readonly CustomerMessageType AccountChange = new CustomerMessageType(23000, "账户余额变动提醒", "账户余额变动提醒");

    /// <summary>
    /// 订单交付图片推送
    /// </summary>
    public static readonly CustomerMessageType OrderPhotoDeliver = new CustomerMessageType(24000, "订单交付图片推送", "订单交付图片推送");

    /// <summary>
    /// 国际快递单号提醒
    /// </summary>
    public static readonly CustomerMessageType InternationalExpressTrackingNo = new CustomerMessageType(25000, "空运订单跟踪单号提醒", "空运订单跟踪单号提醒");

    /// <summary>
    /// 国际快递更新提醒
    /// </summary>
    public static readonly CustomerMessageType InternationalExpressStateUpdate = new CustomerMessageType(26000, "空运订单状态更新提醒", "空运订单状态更新提醒");

    /// <summary>
    /// 订单未支付提醒
    /// </summary>
    public static readonly CustomerMessageType OrderLongNoPay = new CustomerMessageType(27000, "订单未支付提醒", "订单未支付提醒");

    /// <summary>
    /// 未创建订单提醒
    /// </summary>
    public static readonly CustomerMessageType ExpressNoOrder = new CustomerMessageType(28000, "未创建订单提醒", "未创建订单提醒");

    /// <summary>
    /// 国内仓储即将超期提醒
    /// </summary>
    public static readonly CustomerMessageType CnStorageOverdue = new CustomerMessageType(29000, "国内仓储即将超期提醒", "国内仓储即将超期提醒");

    /// <summary>
    /// 空运包裹签收提醒
    /// </summary>
    public static readonly CustomerMessageType OrderDelivered = new CustomerMessageType(30000, "空运包裹签收提醒", "空运包裹签收提醒");

    // 这里改了后，记得客户端对应的类型也更新下

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageType None = new CustomerMessageType(0, "", "");

    private static readonly IReadOnlyList<CustomerMessageType> codeTypeList = new List<CustomerMessageType>() {
        Manual,
        AllCustomer,
        HelpMeMakeMoney,
        CustomerNoOrder,
        CouponFollow,
        NewClaim,
        MoneyEarned,
        ExpressCheckin,
        ExpressExpectEnter,
        Packed,
        Transfered,
        VipUpgrade,
        ExpressPhotoSign,
        ExpressPhotoCheckin,
        ExpressVideoCheckin,
        ExpressPhotoIssue,
        ExpressPhotoCnDamage,
        ExpressPhotoReturn,
        OrderPhotoShip,
        AccountChange,
        OrderPhotoDeliver,
        InternationalExpressTrackingNo,
        InternationalExpressStateUpdate,
        OrderLongNoPay,
        ExpressNoOrder,
        CnStorageOverdue,
        OrderDelivered,
    };

    private static readonly IDictionary<int, CustomerMessageType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageType? type)) {
            return type;
        }

        return None;
    }

    public static IList<CustomerMessageType> GetList() {
        return codeTypeList.ToList();
    }

    private CustomerMessageType(int id, string name, string description) : base(id, name, description) {
    }
}

public class CustomerMessageTypeHibernateType : CodeHibernateTypeBase<CustomerMessageType> {
    protected override CustomerMessageType GetCodeTypeById(int id) {
        return CustomerMessageType.GetById(id);
    }
}

public class CustomerMessageTypeModelBinder : CodeTypeModelBinderBase<CustomerMessageType> {
    protected override CustomerMessageType GetCodeTypeById(int id) {
        return CustomerMessageType.GetById(id);
    }
}
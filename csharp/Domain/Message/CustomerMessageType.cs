using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息类型
/// todo 把这里面的拼音改成英文的
/// </summary>
public class CustomerMessageType : CodeTypeBase
{
    /// <summary>
    /// 手动推送消息
    /// </summary>
    public static readonly CustomerMessageType Manual = new CustomerMessageType(100, "手动推送", "手动推送消息");

    /// <summary>
    /// 全员推送消息
    /// </summary>
    public static readonly CustomerMessageType AllMember = new CustomerMessageType(200, "全员推送", "全员推送消息");

    /// <summary>
    /// 定时提醒，帮我赚钱提醒
    /// </summary>
    public static readonly CustomerMessageType HelpMeMakeMoney = new CustomerMessageType(400, "帮我赚钱提醒", "帮我赚钱提醒");

    /// <summary>
    /// 定时提醒，未注册会员推送
    /// </summary>
    public static readonly CustomerMessageType NotRegister = new CustomerMessageType(500, "未注册会员推送", "未注册会员推送");

    /// <summary>
    /// 定时提醒，已注册未下单推送
    /// </summary>
    public static readonly CustomerMessageType NoOrder = new CustomerMessageType(600, "已注册未下单推送", "已注册未下单推送");

    /// <summary>
    /// 定时提醒，会员未关注公众号推送
    /// </summary>
    public static readonly CustomerMessageType HuiYuanWeiGuanZhuGongZhongHao = new CustomerMessageType(650, "会员未关注公众号推送", "会员未关注公众号推送");

    /// <summary>
    /// 定时提醒，取消关注公众号推送
    /// </summary>
    public static readonly CustomerMessageType QuXiaoGuanZhuGongZhongHao = new CustomerMessageType(700, "取消关注公众号推送", "取消关注公众号推送");

    /// <summary>
    /// 定时提醒，定时提交打包提醒
    /// </summary>
    public static readonly CustomerMessageType DingShiTiJiaoDaBao = new CustomerMessageType(800, "定时提交打包提醒", "定时提交打包提醒");

    /// <summary>
    /// 定时提醒，公共申领件处理提醒
    /// </summary>
    public static readonly CustomerMessageType GongGongShenLingJianChuLi = new CustomerMessageType(900, "公共申领件处理提醒", "公共申领件处理提醒");

    /// <summary>
    /// 实时提醒，我赚到的钱提醒
    /// </summary>
    public static readonly CustomerMessageType WoZhuanDaoDeQian = new CustomerMessageType(1000, "我赚到的钱提醒", "我赚到的钱提醒");

    /// <summary>
    /// 实时提醒，我的申领件提醒
    /// </summary>
    public static readonly CustomerMessageType WoDeShenLingJian = new CustomerMessageType(1200, "我的申领件提醒", "我的申领件提醒");

    /// <summary>
    /// 实时提醒，预报货值提醒
    /// </summary>
    public static readonly CustomerMessageType YuBaoHuoZhi = new CustomerMessageType(1210, "预报货值提醒", "预报货值提醒");

    /// <summary>
    /// 实时提醒，我的问题件提醒
    /// </summary>
    public static readonly CustomerMessageType WoDeWeiTiJian = new CustomerMessageType(1300, "我的问题件提醒", "我的问题件提醒");

    /// <summary>
    /// 实时提醒，已打包提醒
    /// </summary>
    public static readonly CustomerMessageType YiDaBao = new CustomerMessageType(1500, "已打包提醒", "已打包提醒");

    /// <summary>
    /// 实时提醒，空运订单已打包提醒
    /// </summary>
    public static readonly CustomerMessageType OrderAirYiDaBao = new CustomerMessageType(1550, "空运订单已打包提醒", "空运订单已打包提醒");

    /// <summary>
    /// 实时提醒，已转运提醒
    /// </summary>
    public static readonly CustomerMessageType YiZhuanYun = new CustomerMessageType(1600, "已转运提醒", "已转运提醒");

    /// <summary>
    /// VIP升级推送
    /// </summary>
    public static readonly CustomerMessageType VipUpgrade = new CustomerMessageType(2800, "VIP升级推送", "VIP升级推送");

    /// <summary>
    /// 在线客服
    /// </summary>
    public static readonly CustomerMessageType ZaiXianKeFu = new CustomerMessageType(2900, "在线客服", "在线客服");

    /// <summary>
    /// 收取GST推送
    /// </summary>
    public static readonly CustomerMessageType Gst = new CustomerMessageType(3000, "收取GST推送", "收取GST推送");

    /// <summary>
    /// 订单签收图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoSignIn = new CustomerMessageType(3100, "签收图片推送", "签收图片推送");

    /// <summary>
    /// 订单入库图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoCheckIn = new CustomerMessageType(3200, "入库图片推送", "入库图片推送");

    /// <summary>
    /// 订单入库视频推送
    /// </summary>
    public static readonly CustomerMessageType ExpressVideoCheckIn = new CustomerMessageType(3250, "入库视频推送", "入库视频推送");

    /// <summary>
    /// 订单问题图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoIssue = new CustomerMessageType(3300, "问题图片推送", "问题图片推送");

    /// <summary>
    /// 订单中国损毁图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoCnDamage = new CustomerMessageType(3400, "中国损毁图片推送", "中国损毁图片推送");

    /// <summary>
    /// 订单退运图片推送
    /// </summary>
    public static readonly CustomerMessageType ExpressPhotoReturn = new CustomerMessageType(3500, "退运图片推送", "退运图片推送");

    /// <summary>
    /// 订单发货图片推送
    /// </summary>
    public static readonly CustomerMessageType OrderPhotoShip = new CustomerMessageType(3550, "订单发货图片推送", "订单发货图片推送");

    /// <summary>
    /// 账户余额变动提醒
    /// </summary>
    public static readonly CustomerMessageType AccountChange = new CustomerMessageType(3700, "账户余额变动提醒", "账户余额变动提醒");

    /// <summary>
    /// 订单交付图片推送
    /// </summary>
    public static readonly CustomerMessageType OrderPhotoDeliver = new CustomerMessageType(4000, "订单交付图片推送", "订单交付图片推送");

    /// <summary>
    /// 国际快递单号提醒
    /// </summary>
    public static readonly CustomerMessageType InternationalExpressTrackingNo = new CustomerMessageType(4200, "空运订单跟踪单号提醒", "空运订单跟踪单号提醒");

    /// <summary>
    /// 国际快递更新提醒
    /// </summary>
    public static readonly CustomerMessageType InternationalExpressStateUpdate = new CustomerMessageType(4220, "空运订单状态更新提醒", "空运订单状态更新提醒");

    /// <summary>
    /// 订单未支付提醒
    /// </summary>
    public static readonly CustomerMessageType OrderNotPay = new CustomerMessageType(4300, "订单未支付提醒", "订单未支付提醒");

    /// <summary>
    /// 未创建订单提醒
    /// </summary>
    public static readonly CustomerMessageType OrderNotCreate = new CustomerMessageType(4400, "未创建订单提醒", "未创建订单提醒");

    /// <summary>
    /// 客服评价消息
    /// </summary>
    public static readonly CustomerMessageType CustomerChatRateMessage = new CustomerMessageType(4500, "客服评价消息", "客服评价消息");

    /// <summary>
    /// 国内仓储即将超期提醒
    /// </summary>
    public static readonly CustomerMessageType CnStorageOverdue = new CustomerMessageType(4600, "国内仓储即将超期提醒", "国内仓储即将超期提醒");

    /// <summary>
    /// 空运包裹签收提醒
    /// </summary>
    public static readonly CustomerMessageType OrderDeliveredAir = new CustomerMessageType(11000, "🧨🎁🧧🧧快联系空运客服领取红包啦！", "空运包裹签收提醒");

    /// <summary>
    /// 入库超大超重HOLD通知
    /// </summary>
    public static readonly CustomerMessageType CheckinExtraSizeWeightHoldTongZhi = new CustomerMessageType(14000, "入库超大超重HOLD通知", "入库超大超重HOLD通知");

    // 这里改了后，记得客户端对应的类型也更新下

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageType None = new CustomerMessageType(0, "", "");

    private static readonly IReadOnlyList<CustomerMessageType> codeTypeList = new List<CustomerMessageType>() {
            Manual,
            AllMember,
            HelpMeMakeMoney,
            NotRegister,
            NoOrder,
            HuiYuanWeiGuanZhuGongZhongHao,
            QuXiaoGuanZhuGongZhongHao,
            DingShiTiJiaoDaBao,
            GongGongShenLingJianChuLi,
            WoZhuanDaoDeQian,
            WoDeShenLingJian,
            YuBaoHuoZhi,
            WoDeWeiTiJian,
            YiDaBao,
            OrderAirYiDaBao,
            YiZhuanYun,
            VipUpgrade,
            ZaiXianKeFu,
            Gst,
            ExpressPhotoSignIn,
            ExpressPhotoCheckIn,
            ExpressVideoCheckIn,
            ExpressPhotoIssue,
            ExpressPhotoCnDamage,
            ExpressPhotoReturn,
            OrderPhotoShip,
            AccountChange,
            OrderPhotoDeliver,
            InternationalExpressTrackingNo,
            InternationalExpressStateUpdate,
            OrderNotPay,
            OrderNotCreate,
            CustomerChatRateMessage,
            CnStorageOverdue,
            OrderDeliveredAir,
            CheckinExtraSizeWeightHoldTongZhi,
        };

    private static readonly IDictionary<int, CustomerMessageType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageType? type))
        {
            return type;
        }

        return None;
    }

    public static IList<CustomerMessageType> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerMessageType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerMessageTypeHibernateType : CodeHibernateTypeBase<CustomerMessageType>
{
    protected override CustomerMessageType GetCodeTypeById(int id)
    {
        return CustomerMessageType.GetById(id);
    }
}

public class CustomerMessageTypeModelBinder : CodeTypeModelBinderBase<CustomerMessageType>
{
    protected override CustomerMessageType GetCodeTypeById(int id)
    {
        return CustomerMessageType.GetById(id);
    }
}

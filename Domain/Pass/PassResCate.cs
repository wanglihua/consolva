namespace shunda.Domain.Pass;

public class PassResCate {
    // 物流订单
    public const int LogisticsOrderCateId = 02000_00000;
    public static readonly PassResCateInfo LogisticsOrder = new PassResCateInfo() { Id = LogisticsOrderCateId, Name = "物流订单" };

    // 快递管理
    public const int ExpressCateId = 04000_00000;
    public static readonly PassResCateInfo Express = new PassResCateInfo() { Id = ExpressCateId, Name = "快递管理" };

    // 收货打包
    public const int ReceivePackCateId = 06000_00000;
    public static readonly PassResCateInfo ReceivePack = new PassResCateInfo() { Id = ReceivePackCateId, Name = "收货打包" };

    // 费用管理
    public const int FeeCateId = 08000_00000;
    public static readonly PassResCateInfo Fee = new PassResCateInfo() { Id = FeeCateId, Name = "费用管理" };

    // 客户管理
    public const int CustomerCateId = 10000_00000;
    public static readonly PassResCateInfo Customer = new PassResCateInfo() { Id = CustomerCateId, Name = "客户管理" };

    // 仓储管理
    public const int StorageCateId = 12000_00000;
    public static readonly PassResCateInfo Storage = new PassResCateInfo() { Id = StorageCateId, Name = "仓储管理" };

    // 物流设置
    public const int LogisticsCateId = 14000_00000;
    public static readonly PassResCateInfo Logistics = new PassResCateInfo() { Id = LogisticsCateId, Name = "物流设置" };

    // 物品管理
    public const int GoodsCateId = 16000_00000;
    public static readonly PassResCateInfo Goods = new PassResCateInfo() { Id = GoodsCateId, Name = "物品管理" };

    // 消息推送
    public const int CustomerMessageCateId = 18000_00000;
    public static readonly PassResCateInfo CustomerMessage = new PassResCateInfo() { Id = CustomerMessageCateId, Name = "消息推送" };

    // 系统配置
    public const int SettingCateId = 20000_00000;
    public static readonly PassResCateInfo Setting = new PassResCateInfo() { Id = SettingCateId, Name = "系统配置" };

    public static IList<PassResCateInfo> GetLst() {
        return new List<PassResCateInfo>() {
            LogisticsOrder,
            Express,
            ReceivePack,
            Fee,
            Customer,
            Storage,
            Logistics,
            Goods,
            CustomerMessage,
            Setting,
        };
    }
}
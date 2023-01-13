namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 收货打包
    /// </summary>
    public class ReceivePack
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 02000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "收货打包" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = ExpressCheckin, Name = "收货入库", CateIdForCache = CateId },
            new PassResInfo { Id = OrderPack, Name = "订单打包", CateIdForCache = CateId },
            new PassResInfo { Id = ExpressCheckinConfig, Name = "快递单入库配置", CateIdForCache = CateId },
            new PassResInfo { Id = OrderPackConfig, Name = "订单打包配置", CateIdForCache = CateId },
            new PassResInfo { Id = OrderStorageLoc, Name = "订单仓位", CateIdForCache = CateId },
            new PassResInfo { Id = PackReport, Name = "打包统计", CateIdForCache = CateId },
            new PassResInfo { Id = OrderShouldSubmitPack, Name = "催打包订单", CateIdForCache = CateId },
            new PassResInfo { Id = OrderPackSchedule, Name = "订单打包排班", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 收货入库
        /// </summary>
        public const int ExpressCheckin = CateId + 0100;

        /// <summary>
        /// 订单打包
        /// </summary>
        public const int OrderPack = CateId + 0200;

        /// <summary>
        /// 快递单入库配置
        /// </summary>
        public const int ExpressCheckinConfig = CateId + 0390;

        /// <summary>
        /// 订单打包配置
        /// </summary>
        public const int OrderPackConfig = CateId + 0400;

        /// <summary>
        /// 订单仓位
        /// </summary>
        public const int OrderStorageLoc = CateId + 0420;

        /// <summary>
        /// 打包统计
        /// </summary>
        public const int PackReport = CateId + 0500;

        /// <summary>
        /// 催打包订单
        /// </summary>
        public const int OrderShouldSubmitPack = CateId + 0600;

        /// <summary>
        /// 订单打包排班
        /// </summary>
        public const int OrderPackSchedule = CateId + 0800;
    }
}
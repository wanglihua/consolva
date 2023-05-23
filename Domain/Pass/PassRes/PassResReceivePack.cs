namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 收货打包
    /// </summary>
    public class ReceivePack {
        public const int CateId = PassResCate.ReceivePackCateId;
        public static PassResCateInfo Cate => PassResCate.ReceivePack;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = ExpressCheckin, Name = "收货入库", Cate = Cate },
            new PassResInfo { Id = OrderPack, Name = "订单打包", Cate = Cate },
            new PassResInfo { Id = ExpressCheckinConfig, Name = "快递单入库配置", Cate = Cate },
            new PassResInfo { Id = OrderPackConfig, Name = "订单打包配置", Cate = Cate },
            new PassResInfo { Id = OrderStorageLoc, Name = "订单仓位", Cate = Cate },
            new PassResInfo { Id = PackReport, Name = "打包统计", Cate = Cate },
            new PassResInfo { Id = OrderShouldSubmitPack, Name = "催打包订单", Cate = Cate },
            new PassResInfo { Id = OrderPackSchedule, Name = "订单打包排班", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 收货入库
        /// </summary>
        public const int ExpressCheckin = CateId + 0200;

        /// <summary>
        /// 订单打包
        /// </summary>
        public const int OrderPack = CateId + 0400;

        /// <summary>
        /// 快递单入库配置
        /// </summary>
        public const int ExpressCheckinConfig = CateId + 0600;

        /// <summary>
        /// 订单打包配置
        /// </summary>
        public const int OrderPackConfig = CateId + 0800;

        /// <summary>
        /// 订单仓位
        /// </summary>
        public const int OrderStorageLoc = CateId + 1000;

        /// <summary>
        /// 打包统计
        /// </summary>
        public const int PackReport = CateId + 1200;

        /// <summary>
        /// 催打包订单
        /// </summary>
        public const int OrderShouldSubmitPack = CateId + 1400;

        /// <summary>
        /// 订单打包排班
        /// </summary>
        public const int OrderPackSchedule = CateId + 1600;
    }
}
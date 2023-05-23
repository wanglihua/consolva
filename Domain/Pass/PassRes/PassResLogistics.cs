namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流设置
    /// </summary>
    public class Logistics {
        public const int CateId = PassResCate.LogisticsCateId;
        public static PassResCateInfo Cate => PassResCate.Logistics;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = LogisticsModeSetting, Name = "物流配置", Cate = Cate },
            new PassResInfo { Id = LogisticsFeeRate, Name = "通用费率", Cate = Cate },
            new PassResInfo { Id = LogisticsModeList, Name = "物流列表", Cate = Cate },
            new PassResInfo { Id = Insurance, Name = "保险", Cate = Cate },
            new PassResInfo { Id = Gst, Name = "GST", Cate = Cate },
            new PassResInfo { Id = ExpressReturnFeeRate, Name = "退运费率", Cate = Cate },
            new PassResInfo { Id = LogisticsRepackTypeSetting, Name = "打包方式设置", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 物流配置
        /// </summary>
        public const int LogisticsModeSetting = CateId + 0200;

        /// <summary>
        /// 通用费率
        /// </summary>
        public const int LogisticsFeeRate = CateId + 0400;

        /// <summary>
        /// 物流列表
        /// </summary>
        public const int LogisticsModeList = CateId + 0600;

        /// <summary>
        /// 保险
        /// </summary>
        public const int Insurance = CateId + 0800;

        /// <summary>
        /// GST
        /// </summary>
        public const int Gst = CateId + 1000;

        /// <summary>
        /// 退运费率
        /// </summary>
        public const int ExpressReturnFeeRate = CateId + 1200;

        /// <summary>
        /// 打包方式设置
        /// </summary>
        public const int LogisticsRepackTypeSetting = CateId + 1400;
    }
}
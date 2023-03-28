namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流设置
    /// </summary>
    public class Logistics {
        public const int CateId = PassResCate.LogisticsCateId;
        public static readonly PassResCateInfo Cate = PassResCate.Logistics;

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
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
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 物流配置
        /// </summary>
        public const int LogisticsModeSetting = CateId + 02000;

        /// <summary>
        /// 通用费率
        /// </summary>
        public const int LogisticsFeeRate = CateId + 04000;

        /// <summary>
        /// 物流列表
        /// </summary>
        public const int LogisticsModeList = CateId + 06000;

        /// <summary>
        /// 保险
        /// </summary>
        public const int Insurance = CateId + 08000;

        /// <summary>
        /// GST
        /// </summary>
        public const int Gst = CateId + 10000;

        /// <summary>
        /// 退运费率
        /// </summary>
        public const int ExpressReturnFeeRate = CateId + 12000;

        /// <summary>
        /// 打包方式设置
        /// </summary>
        public const int LogisticsRepackTypeSetting = CateId + 14000;
    }
}
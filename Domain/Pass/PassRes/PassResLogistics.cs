namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流设置
    /// </summary>
    public class Logistics {
        /// <summary>
        /// 物流设置类别
        /// </summary>
        public const int CateId = 10000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "物流设置" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = LogisticsFeeRate, Name = "通用费率", CateIdForCache = CateId },
            new PassResInfo { Id = LogisticsModeList, Name = "物流列表", CateIdForCache = CateId },
            new PassResInfo { Id = Insurance, Name = "保险", CateIdForCache = CateId },
            new PassResInfo { Id = Gst, Name = "GST", CateIdForCache = CateId },
            new PassResInfo { Id = ExpressReturnFeeRate, Name = "退运费率", CateIdForCache = CateId },
            new PassResInfo { Id = LogisticsRepackTypeSetting, Name = "打包方式设置", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 通用费率
        /// </summary>
        public const int LogisticsFeeRate = CateId + 02000;

        /// <summary>
        /// 物流列表
        /// </summary>
        public const int LogisticsModeList = CateId + 04000;

        /// <summary>
        /// 保险
        /// </summary>
        public const int Insurance = CateId + 06000;

        /// <summary>
        /// GST
        /// </summary>
        public const int Gst = CateId + 08000;

        /// <summary>
        /// 退运费率
        /// </summary>
        public const int ExpressReturnFeeRate = CateId + 10000;

        /// <summary>
        /// 打包方式设置
        /// </summary>
        public const int LogisticsRepackTypeSetting = CateId + 12000;
    }
}
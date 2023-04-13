namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流跟踪
    /// </summary>
    public class LogisticsTrack {
        public const int CateId = PassResCate.LogisticsTrackCateId;
        public static readonly PassResCateInfo Cate = PassResCate.LogisticsTrack;

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = SeventeenTrackCarrier, Name = "17track运输商", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 17track运输商
        /// </summary>
        public const int SeventeenTrackCarrier = CateId + 1800;
    }
}
namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流跟踪
    /// </summary>
    public class LogisticsTrack {
        public const int CateId = PassResCate.LogisticsTrackCateId;
        public static PassResCateInfo Cate => PassResCate.LogisticsTrack;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = SeventeenTrackNumber, Name = "17TRACK注册单号", Cate = Cate },
            new PassResInfo { Id = SeventeenTrackCarrier, Name = "17TRACK运输商", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 17TRACK注册单号
        /// </summary>
        public const int SeventeenTrackNumber = CateId + 1600;

        /// <summary>
        /// 17TRACK运输商
        /// </summary>
        public const int SeventeenTrackCarrier = CateId + 1800;
    }
}
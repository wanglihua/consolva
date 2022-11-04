namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 快递管理
    /// </summary>
    public class Express
    {
        /// <summary>
        /// 快递管理类别
        /// </summary>
        public const int CateId = 01100_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "快递管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ExpressRemarkPresetSelect, Name = "快递备注预设", CateIdForCache = CateId },
        };

        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 快递备注预设
        /// </summary>
        public const int ExpressRemarkPresetSelect = CateId + 10000;
    }
}
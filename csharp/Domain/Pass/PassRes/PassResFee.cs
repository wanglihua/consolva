namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 费用管理
    /// </summary>
    public class Fee
    {
        /// <summary>
        /// 费用管理类别
        /// </summary>
        public const int CateId = 06000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "费用管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ManualFeeReason, Name = "增减核销费用原因", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 增减核销费用原因
        /// </summary>
        public const int ManualFeeReason = CateId + 02000;
    }
}
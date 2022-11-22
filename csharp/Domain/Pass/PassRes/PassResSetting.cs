namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 09000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "系统配置" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ExchangeRate, Name = "汇率设置", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 汇率设置
        /// </summary>
        public const int ExchangeRate = CateId + 0700;
    }
}
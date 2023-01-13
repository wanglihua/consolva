namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 系统配置
    /// </summary>
    public class Setting {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 09000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "系统配置" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = ExchangeRate, Name = "汇率设置", CateIdForCache = CateId },
            new PassResInfo { Id = ClientAgreement, Name = "客户端协议", CateIdForCache = CateId },
            new PassResInfo { Id = ClientInstruction, Name = "客户端文本菜单", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 汇率设置
        /// </summary>
        public const int ExchangeRate = CateId + 02000;

        /// <summary>
        /// 客户端协议
        /// </summary>
        public const int ClientAgreement = CateId + 04000;

        /// <summary>
        /// 客户端文本菜单
        /// </summary>
        public const int ClientInstruction = CateId + 06000;
    }
}
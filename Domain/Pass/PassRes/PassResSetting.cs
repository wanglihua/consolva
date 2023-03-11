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
            new PassResInfo { Id = ClientNotice, Name = "客户端公告", Cate = Cate },
            new PassResInfo { Id = ExchangeRate, Name = "汇率设置", Cate = Cate },
            new PassResInfo { Id = ClientAgreement, Name = "客户端协议", Cate = Cate },
            new PassResInfo { Id = ClientInstruction, Name = "客户端文本菜单", Cate = Cate },
            new PassResInfo { Id = AutoMeasureMachine, Name = "自动秤设备", Cate = Cate },
            new PassResInfo { Id = AdvertBanner, Name = "广告横幅", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 客户端公告
        /// </summary>
        public const int ClientNotice = CateId + 02000;

        /// <summary>
        /// 汇率设置
        /// </summary>
        public const int ExchangeRate = CateId + 04000;

        /// <summary>
        /// 客户端协议
        /// </summary>
        public const int ClientAgreement = CateId + 06000;

        /// <summary>
        /// 客户端文本菜单
        /// </summary>
        public const int ClientInstruction = CateId + 08000;

        /// <summary>
        /// 自动秤设备
        /// </summary>
        public const int AutoMeasureMachine = CateId + 10000;

        /// <summary>
        /// 广告横幅
        /// </summary>
        public const int AdvertBanner = CateId + 12000;
    }
}
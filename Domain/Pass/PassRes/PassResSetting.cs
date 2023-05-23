namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 系统配置
    /// </summary>
    public class Setting {
        public const int CateId = PassResCate.SettingCateId;
        public static PassResCateInfo Cate => PassResCate.Setting;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = ClientNotice, Name = "客户端公告", Cate = Cate },
            new PassResInfo { Id = ExchangeRate, Name = "汇率设置", Cate = Cate },
            new PassResInfo { Id = ClientAgreement, Name = "客户端协议", Cate = Cate },
            new PassResInfo { Id = ClientInstruction, Name = "客户端文本菜单", Cate = Cate },
            new PassResInfo { Id = AutoMeasureMachine, Name = "自动秤设备", Cate = Cate },
            new PassResInfo { Id = AdvertBanner, Name = "客户端广告横幅", Cate = Cate },
            new PassResInfo { Id = ClientPopup, Name = "客户端弹窗", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 客户端公告
        /// </summary>
        public const int ClientNotice = CateId + 0200;

        /// <summary>
        /// 汇率设置
        /// </summary>
        public const int ExchangeRate = CateId + 0400;

        /// <summary>
        /// 客户端协议
        /// </summary>
        public const int ClientAgreement = CateId + 0600;

        /// <summary>
        /// 客户端文本菜单
        /// </summary>
        public const int ClientInstruction = CateId + 0800;

        /// <summary>
        /// 自动秤设备
        /// </summary>
        public const int AutoMeasureMachine = CateId + 1000;

        /// <summary>
        /// 客户端广告横幅
        /// </summary>
        public const int AdvertBanner = CateId + 1200;

        /// <summary>
        /// 客户端弹窗
        /// </summary>
        public const int ClientPopup = CateId + 1400;
    }
}
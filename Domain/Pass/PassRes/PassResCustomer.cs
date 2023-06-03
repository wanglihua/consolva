namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    ///会员管理
    /// </summary>
    public class Customer {
        private const int CateId = PassResCate.CustomerCateId;
        private static PassResCateInfo Cate => PassResCate.Customer;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = CustomerList, Name = "会员列表", Cate = Cate },
            new PassResInfo { Id = ImportTestCustomer, Name = "测试账号导入", Cate = Cate },
            new PassResInfo { Id = CustomerFreeze, Name = "会员冻结解冻", Cate = Cate },
            new PassResInfo { Id = CustomerResetPassword, Name = "会员重设密码", Cate = Cate },
            new PassResInfo { Id = CustomerCharacter, Name = "会员性质", Cate = Cate },
            new PassResInfo { Id = CustomerPromotion, Name = "客户推广", Cate = Cate },
            new PassResInfo { Id = SupperPassword, Name = "超级密码", Cate = Cate },
            new PassResInfo { Id = CustomerScoreRule, Name = "积分规则", Cate = Cate },
            new PassResInfo { Id = CustomerPromotionImage, Name = "推广图片", Cate = Cate },
            new PassResInfo { Id = CustomerAccountEntry, Name = "账户余额变动", Cate = Cate },
            new PassResInfo { Id = CustomerScoreLog, Name = "会员积分日志", Cate = Cate },
            new PassResInfo { Id = CustomerVipDiscount, Name = "VIP折扣设置", Cate = Cate },
            new PassResInfo { Id = CustomerSuperVipDiscount, Name = "超级VIP折扣", Cate = Cate },
            new PassResInfo { Id = CustomerChargeAwardSetting, Name = "客户充值奖励", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 会员列表
        /// </summary>
        public const int CustomerList = CateId + 0200;

        /// <summary>
        /// 测试账号导入
        /// </summary>
        public const int ImportTestCustomer = CateId + 0400;

        /// <summary>
        /// 会员冻结解冻
        /// </summary>
        public const int CustomerFreeze = CateId + 0600;

        /// <summary>
        /// 会员重设密码
        /// </summary>
        public const int CustomerResetPassword = CateId + 0800;

        /// <summary>
        /// 会员性质
        /// </summary>
        public const int CustomerCharacter = CateId + 10000;

        /// <summary>
        /// 客户推广
        /// </summary>
        public const int CustomerPromotion = CateId + 1200;

        /// <summary>
        /// 超级密码
        /// </summary>
        public const int SupperPassword = CateId + 1400;

        /// <summary>
        /// 积分规则
        /// </summary>
        public const int CustomerScoreRule = CateId + 1600;

        /// <summary>
        /// 推广图片
        /// </summary>
        public const int CustomerPromotionImage = CateId + 1800;

        /// <summary>
        /// 账户余额变动
        /// </summary>
        public const int CustomerAccountEntry = CateId + 2000;

        /// <summary>
        /// 会员积分日志
        /// </summary>
        public const int CustomerScoreLog = CateId + 2200;

        /// <summary>
        /// VIP折扣设置
        /// </summary>
        public const int CustomerVipDiscount = CateId + 2400;

        /// <summary>
        /// 超级VIP折扣
        /// </summary>
        public const int CustomerSuperVipDiscount = CateId + 2600;
        
        /// <summary>
        /// 客户充值奖励
        /// </summary>
        public const int CustomerChargeAwardSetting = CateId + 2800;
    }
}
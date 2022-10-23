namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    ///会员管理
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 会员管理类别
        /// </summary>
        public const int CateId = 07000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "会员管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = CustomerList, Name = "会员列表", CateIdForCache = CateId },
            new PassResInfo() { Id = ImportTestCustomer, Name = "测试账号导入", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerFreeze, Name = "会员冻结解冻", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerResetPassword, Name = "会员重设密码", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerCharacter, Name = "会员性质", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerPromotion, Name = "客户推广", CateIdForCache = CateId },
            new PassResInfo() { Id = SupperPassword, Name = "超级密码", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerScoreRule, Name = "积分规则", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerPromotionImage, Name = "推广图片", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerAccountEntry, Name = "账户余额变动", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerScoreLog, Name = "会员积分日志", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerVipDiscount, Name = "VIP折扣设置", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerSuperVipDiscount, Name = "超级VIP折扣", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 会员列表
        /// </summary>
        public const int CustomerList = CateId + 02000;

        /// <summary>
        /// 测试账号导入
        /// </summary>
        public const int ImportTestCustomer = CateId + 04000;

        /// <summary>
        /// 会员冻结解冻
        /// </summary>
        public const int CustomerFreeze = CateId + 06000;

        /// <summary>
        /// 会员重设密码
        /// </summary>
        public const int CustomerResetPassword = CateId + 08000;

        /// <summary>
        /// 会员性质
        /// </summary>
        public const int CustomerCharacter = CateId + 10000;

        /// <summary>
        /// 客户推广
        /// </summary>
        public const int CustomerPromotion = CateId + 12000;

        /// <summary>
        /// 超级密码
        /// </summary>
        public const int SupperPassword = CateId + 14000;

        /// <summary>
        /// 积分规则
        /// </summary>
        public const int CustomerScoreRule = CateId + 16000;

        /// <summary>
        /// 推广图片
        /// </summary>
        public const int CustomerPromotionImage = CateId + 18000;

        /// <summary>
        /// 账户余额变动
        /// </summary>
        public const int CustomerAccountEntry = CateId + 20000;

        /// <summary>
        /// 会员积分日志
        /// </summary>
        public const int CustomerScoreLog = CateId + 22000;

        /// <summary>
        /// VIP折扣设置
        /// </summary>
        public const int CustomerVipDiscount = CateId + 24000;

        /// <summary>
        /// 超级VIP折扣
        /// </summary>
        public const int CustomerSuperVipDiscount = CateId + 26000;
    }
}
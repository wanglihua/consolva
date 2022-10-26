namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 费用管理
    /// </summary>
    public class OrderFee
    {
        /// <summary>
        /// 费用管理类别
        /// </summary>
        public const int CateId = 06000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "费用管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ManualIncreaseFee, Name = "手动增加费用", CateIdForCache = CateId },
            new PassResInfo() { Id = ManualDecreaseFeeApply, Name = "手动减少费用申请", CateIdForCache = CateId },
            new PassResInfo() { Id = ManualDecreaseFeeApprove, Name = "手动减少费用审核", CateIdForCache = CateId },
            new PassResInfo() { Id = ManualDecreaseFee, Name = "手动减少费用", CateIdForCache = CateId },
            new PassResInfo() { Id = WriteOffFeeApply, Name = "费用核销申请", CateIdForCache = CateId },
            new PassResInfo() { Id = WriteOffFeeApprove, Name = "费用核销审核", CateIdForCache = CateId },
            new PassResInfo() { Id = WriteOffFee, Name = "费用核销", CateIdForCache = CateId },
            new PassResInfo() { Id = RefundApply, Name = "退款赔偿申请", CateIdForCache = CateId },
            new PassResInfo() { Id = RefundApprove, Name = "退款赔偿审核", CateIdForCache = CateId },
            new PassResInfo() { Id = Refund, Name = "退款赔偿", CateIdForCache = CateId },
            new PassResInfo() { Id = ArrearsFeeList, Name = "欠费清单", CateIdForCache = CateId },
            new PassResInfo() { Id = LogisticsOrderPay, Name = "实时收入", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerCharge, Name = "会员充值", CateIdForCache = CateId },
            new PassResInfo() { Id = CustomerChargeManualCharge, Name = "会员充值手动充值", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressReturnPay, Name = "退运支付", CateIdForCache = CateId },
            new PassResInfo() { Id = Coupon, Name = "优惠券", CateIdForCache = CateId },
            new PassResInfo() { Id = ManualFeeReason, Name = "增减核销费用原因", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();


        /// <summary>
        /// 手动增加费用
        /// </summary>
        public const int ManualIncreaseFee = CateId + 02000;

        /// <summary>
        /// 手动减少费用申请
        /// </summary>
        public const int ManualDecreaseFeeApply = CateId + 04000;

        /// <summary>
        /// 手动减少费用审核
        /// </summary>
        public const int ManualDecreaseFeeApprove = CateId + 06000;

        /// <summary>
        /// 手动减少费用
        /// </summary>
        public const int ManualDecreaseFee = CateId + 08000;

        /// <summary>
        /// 费用核销申请
        /// </summary>
        public const int WriteOffFeeApply = CateId + 10000;

        /// <summary>
        /// 费用核销审核
        /// </summary>
        public const int WriteOffFeeApprove = CateId + 12000;

        /// <summary>
        /// 费用核销
        /// </summary>
        public const int WriteOffFee = CateId + 14000;

        /// <summary>
        /// 退款赔偿申请
        /// </summary>
        public const int RefundApply = CateId + 16000;

        /// <summary>
        /// 退款赔偿审核
        /// </summary>
        public const int RefundApprove = CateId + 18000;

        /// <summary>
        /// 退款赔偿
        /// </summary>
        public const int Refund = CateId + 20000;

        /// <summary>
        /// 欠费清单
        /// </summary>
        public const int ArrearsFeeList = CateId + 22000;

        /// <summary>
        /// 实时收入
        /// </summary>
        public const int LogisticsOrderPay = CateId + 24000;

        /// <summary>
        /// 会员充值
        /// </summary>
        public const int CustomerCharge = CateId + 26000;

        /// <summary>
        /// 会员充值手动充值
        /// </summary>
        public const int CustomerChargeManualCharge = CateId + 28000;

        /// <summary>
        /// 退运支付
        /// </summary>
        public const int ExpressReturnPay = CateId + 30000;

        /// <summary>
        /// 优惠券
        /// </summary>
        public const int Coupon = CateId + 32000;

        /// <summary>
        /// 增减核销费用原因
        /// </summary>
        public const int ManualFeeReason = CateId + 34000;
    }
}
namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 费用管理
    /// </summary>
    public class Fee {
        public const int CateId = PassResCate.FeeCateId;
        public static PassResCateInfo Cate => PassResCate.Fee;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = ManualIncreaseFee, Name = "手动增加费用", Cate = Cate },
            new PassResInfo { Id = ManualDecreaseFeeApply, Name = "手动减少费用申请", Cate = Cate },
            new PassResInfo { Id = ManualDecreaseFeeApprove, Name = "手动减少费用审核", Cate = Cate },
            new PassResInfo { Id = ManualDecreaseFee, Name = "手动减少费用", Cate = Cate },
            new PassResInfo { Id = WriteOffFeeApply, Name = "费用核销申请", Cate = Cate },
            new PassResInfo { Id = WriteOffFeeApprove, Name = "费用核销审核", Cate = Cate },
            new PassResInfo { Id = WriteOffFee, Name = "费用核销", Cate = Cate },
            new PassResInfo { Id = ManualFeeReason, Name = "增减核销费用原因", Cate = Cate },
            new PassResInfo { Id = FeeRefundApply, Name = "退款赔偿申请", Cate = Cate },
            new PassResInfo { Id = FeeRefundApprove, Name = "退款赔偿审核", Cate = Cate },
            new PassResInfo { Id = FeeRefund, Name = "退款赔偿", Cate = Cate },
            new PassResInfo { Id = FeeRefundReason, Name = "退款赔偿原因", Cate = Cate },
            new PassResInfo { Id = ArrearsFeeList, Name = "欠费清单", Cate = Cate },
            new PassResInfo { Id = LogisticsOrderPay, Name = "实时收入", Cate = Cate },
            new PassResInfo { Id = CustomerCharge, Name = "会员充值", Cate = Cate },
            new PassResInfo { Id = CustomerChargeManualCharge, Name = "会员充值手动充值", Cate = Cate },
            new PassResInfo { Id = ManualChargeReason, Name = "手动充值原因", Cate = Cate },
            new PassResInfo { Id = ExpressReturnPay, Name = "退运支付", Cate = Cate },
            new PassResInfo { Id = Coupon, Name = "优惠券", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 手动增加费用
        /// </summary>
        public const int ManualIncreaseFee = CateId + 0200;

        /// <summary>
        /// 手动减少费用申请
        /// </summary>
        public const int ManualDecreaseFeeApply = CateId + 0400;

        /// <summary>
        /// 手动减少费用审核
        /// </summary>
        public const int ManualDecreaseFeeApprove = CateId + 0600;

        /// <summary>
        /// 手动减少费用
        /// </summary>
        public const int ManualDecreaseFee = CateId + 0800;

        /// <summary>
        /// 费用核销申请
        /// </summary>
        public const int WriteOffFeeApply = CateId + 1000;

        /// <summary>
        /// 费用核销审核
        /// </summary>
        public const int WriteOffFeeApprove = CateId + 1200;

        /// <summary>
        /// 费用核销
        /// </summary>
        public const int WriteOffFee = CateId + 1400;

        /// <summary>
        /// 增减核销费用原因
        /// </summary>
        public const int ManualFeeReason = CateId + 1600;

        /// <summary>
        /// 退款赔偿申请
        /// </summary>
        public const int FeeRefundApply = CateId + 1800;

        /// <summary>
        /// 退款赔偿审核
        /// </summary>
        public const int FeeRefundApprove = CateId + 2000;

        /// <summary>
        /// 退款赔偿
        /// </summary>
        public const int FeeRefund = CateId + 2200;

        /// <summary>
        /// 退款赔偿原因
        /// </summary>
        public const int FeeRefundReason = CateId + 2400;

        /// <summary>
        /// 欠费清单
        /// </summary>
        public const int ArrearsFeeList = CateId + 2600;

        /// <summary>
        /// 实时收入
        /// </summary>
        public const int LogisticsOrderPay = CateId + 2800;

        /// <summary>
        /// 会员充值
        /// </summary>
        public const int CustomerCharge = CateId + 3000;

        /// <summary>
        /// 会员充值手动充值
        /// </summary>
        public const int CustomerChargeManualCharge = CateId + 3200;

        /// <summary>
        /// 手动充值原因
        /// </summary>
        public const int ManualChargeReason = CateId + 3400;

        /// <summary>
        /// 退运支付
        /// </summary>
        public const int ExpressReturnPay = CateId + 3600;

        /// <summary>
        /// 优惠券
        /// </summary>
        public const int Coupon = CateId + 3800;
    }
}
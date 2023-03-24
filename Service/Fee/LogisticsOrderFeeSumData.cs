namespace shunda.Service.Fee; 

public class LogisticsOrderFeeSumData {
    
    public decimal FirstModeFee { get; set; }
    
    public decimal FirstModeFeeNoPromotion { get; set; }
    
    public decimal TotaAmount { get; set; }
    
    public decimal TotalAmountWithoutCouponDiscount { get; set; }
    
    /// <summary>
    /// 没有优惠券折扣，也没VIP折扣
    /// </summary>
    public decimal TotalAmountWithoutDiscount { get; set; }
    
    /// <summary>
    /// VIP和超级VIP折扣，这里取的是正数
    /// </summary>
    public decimal VipDiscountAmount { get; set; } 
}
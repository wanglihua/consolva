namespace shunda.Service.Fee;

public class LogisticsOrderPayDateStatItem {
    public DateTime Date { get; set; }

    public decimal AlipayAmount { get; set; }

    public decimal WechatAmount { get; set; }

    public decimal PaypalAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    public decimal WriteOffAmount { get; set; }

    public decimal TotalAmount { get; set; }
}
using shunda.Domain.Customer;

namespace shunda.Service.Customer; 

public class CustomerScoreExcelImportItem {
    public CustomerInfo Customer { get; set; } = new CustomerInfo();
    
    public decimal ValidScoreDelta { get; set; }
    
    public decimal TotalScoreDelta { get; set; }
}
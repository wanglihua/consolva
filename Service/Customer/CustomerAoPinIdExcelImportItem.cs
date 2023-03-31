using shunda.Domain.Customer;

namespace shunda.Service.Customer;

public class CustomerAoPinIdExcelImportItem {
    public int CustomerId { get; set; }

    public int AoPinId { get; set; }

    public CustomerInfo Customer { get; set; } = new();
}

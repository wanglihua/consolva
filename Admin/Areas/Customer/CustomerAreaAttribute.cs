using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Customer;

public class CustomerArea {
    public static string Name => "Customer";
}

public class CustomerAreaAttribute : AreaAttribute {
    public CustomerAreaAttribute() : base(CustomerArea.Name) {
    }
}
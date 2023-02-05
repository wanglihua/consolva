using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Customer;

public class CustomerArea {
    public static string Name => "Customer";
}

// 这个里面包含 地址，余额，积分，支付明细等等的

public class CustomerAreaAttribute : AreaAttribute {
    public CustomerAreaAttribute() : base(CustomerArea.Name) {
    }
}
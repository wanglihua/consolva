using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Customer;

// 这个里面包含 地址，余额，积分，支付明细等等的

public class CustomerAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Customer";

    public CustomerAreaAttribute() : base(Name)
    {
    }
}
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Customer;

public class CustomerAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Customer";

    public CustomerAreaAttribute() : base(Name)
    {
    }
}
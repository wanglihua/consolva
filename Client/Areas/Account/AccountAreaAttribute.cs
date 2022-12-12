using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Account;

// 这个里面包含 余额，积分，支付明细等等的

public class AccountAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Account";

    public AccountAreaAttribute() : base(Name)
    {
    }
}
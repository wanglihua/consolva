using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.ReceivePack;

public class ReceivePackAreaAttribute : AreaAttribute
{
    public static readonly string Name = "ReceivePack";

    public ReceivePackAreaAttribute() : base(Name)
    {
    }
}
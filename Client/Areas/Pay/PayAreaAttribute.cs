using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Pay;

public class PayArea {
    public static string Name => "Pay";
}

public class PayAreaAttribute : AreaAttribute {
    public PayAreaAttribute() : base(PayArea.Name) {
    }
}
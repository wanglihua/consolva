using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee;

public class FeeArea {
    public static string Name => "Fee";
}

public class FeeAreaAttribute : AreaAttribute {
    public FeeAreaAttribute() : base(FeeArea.Name) {
    }
}
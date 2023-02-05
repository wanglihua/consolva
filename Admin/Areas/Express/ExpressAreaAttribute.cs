using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Express;

public class ExpressArea {
    public static string Name => "Express";
}

public class ExpressAreaAttribute : AreaAttribute {
    public ExpressAreaAttribute() : base(ExpressArea.Name) {
    }
}
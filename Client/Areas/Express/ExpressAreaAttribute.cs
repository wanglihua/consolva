using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Express;

public class ExpressArea {
    public static string Name => "Express";
}

public class ExpressAreaAttribute : AreaAttribute {
    public ExpressAreaAttribute() : base(ExpressArea.Name) {
    }
}
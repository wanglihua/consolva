using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Pass;

public class PassArea {
    public static string Name => "Pass";
}

public class PassAreaAttribute : AreaAttribute {
    public PassAreaAttribute() : base(PassArea.Name) {
    }
}
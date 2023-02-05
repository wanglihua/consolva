using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.LogisticsOrder;

public class LogisticsOrderArea {
    public static string Name => "LogisticsOrder";
}

public class LogisticsOrderAreaAttribute : AreaAttribute {
    public LogisticsOrderAreaAttribute() : base(LogisticsOrderArea.Name) {
    }
}
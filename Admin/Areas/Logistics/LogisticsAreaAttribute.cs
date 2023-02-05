using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Logistics;

public class LogisticsArea {
    public static string Name => "Logistics";
}

public class LogisticsAreaAttribute : AreaAttribute {
    public LogisticsAreaAttribute() : base(LogisticsArea.Name) {
    }
}
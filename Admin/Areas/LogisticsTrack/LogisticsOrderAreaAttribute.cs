using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.LogisticsTrack;

public class LogisticsTrackArea {
    public static string Name => "LogisticsTrack";
}

public class LogisticsTrackAreaAttribute : AreaAttribute {
    public LogisticsTrackAreaAttribute() : base(LogisticsTrackArea.Name) {
    }
}
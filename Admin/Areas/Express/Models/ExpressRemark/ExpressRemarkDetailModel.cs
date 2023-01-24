using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Express.Models.ExpressRemark;

public class ExpressRemarkDetailModel : ExpressRemarkSaveModel {
    [BindNever]
    public string TrackingNo { get; set; } = string.Empty;
}
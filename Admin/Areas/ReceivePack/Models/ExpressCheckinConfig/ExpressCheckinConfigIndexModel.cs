using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckinConfig;

public class ExpressCheckinConfigIndexModel
{
    [BindNever]
    public IList<ExpressCheckinLabelConfigStatus> ExpressCheckinLabelStatusList { get; set; } = ExpressCheckinLabelConfigStatus.GetList();

    public ExpressCheckinConfigInfo ExpressCheckinConfig { get; set; } = new ();
}
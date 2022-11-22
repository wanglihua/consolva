using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckinConfig;

public class ExpressCheckinConfigPrintPreviewModel
{
    [BindNever]
    [ModelBinder(typeof(ExpressCheckinPrintTypeModelBinder))]
    public ExpressCheckinLabelType LabelType { get; set; } = ExpressCheckinLabelType.None;

#if DEBUG
    public readonly bool Debug = true;
#else
    public readonly bool Debug = false;
#endif
}
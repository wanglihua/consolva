using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckinConfig;

public class ExpressCheckinConfigPrintLabelModel {
    public ExpressCheckinLabelType LabelType { get; set; } = ExpressCheckinLabelType.None;

    public bool Preview { get; set; }

#if DEBUG
    public bool Debug { get; set; } = true;
#else
    public bool Debug { get; set; } = false;
#endif
}
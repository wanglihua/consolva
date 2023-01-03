using shunda.Core;
using shunda.Domain.Express;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListModel : ListModel {
    public ExpressReturnListModel(ListPager listPager) : base(listPager) {
    }

    public string ExpressReturnAlias = string.Empty;
    public string ExpressAlias { get; set; } = string.Empty;
    public string AddUserAlias { get; set; } = string.Empty;
    public string WarehouseAlias { get; set; } = string.Empty;
    public string CustomerAlias { get; set; } = string.Empty;
    public string ReturnUserAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<ExpressReturnInfo> ExpressReturnList { get; set; } = new List<ExpressReturnInfo>();
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Express.Models.ExpressReturn;

public class ExpressReturnDetailModel : ExpressReturnSaveModel {
    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();

    [BindNever]
    public IList<ExpressStatus> StatusList { get; set; } = new List<ExpressStatus>();

}
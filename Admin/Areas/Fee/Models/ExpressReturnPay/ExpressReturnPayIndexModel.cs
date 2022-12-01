using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ExpressReturnPay;

public class ExpressReturnPayIndexModel
{
    [BindNever]
    public IList<PayType> PayTypeList { get; set; } = PayType.GetList();

    [BindNever]
    public IList<PayStatus> StatusList { get; set; } = PayStatus.GetList();
}
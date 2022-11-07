using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.OrderFee.Models.ExpressReturnPay;

    public class ExpressReturnPayListModel : ListModel
    {
        public ExpressReturnPayListModel(ListPager listPager) : base(listPager)
        {
        }

        [BindNever]
        public IList<ExpressReturnPayInfo> ExpressReturnPayList { get; set; } = new List<ExpressReturnPayInfo>();
    }

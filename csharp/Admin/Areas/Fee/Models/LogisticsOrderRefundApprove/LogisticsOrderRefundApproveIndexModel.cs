using System.Collections.Generic;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApprove
{
    public class LogisticsOrderRefundApproveIndexModel
    {
        [BindNever]
        public IList<FeeRefundType> TypeList { get; set; }

        [BindNever]
        public IList<FeeRefundApplyStatus> StatusList { get; set; }
        
        [BindNever]
        public IList<UserInfo> ApplyUserList { get; set; }        
    }
}
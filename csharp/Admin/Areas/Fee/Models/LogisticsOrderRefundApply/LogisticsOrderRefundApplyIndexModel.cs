using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApply
{
    public class LogisticsOrderRefundApplyIndexModel
    {
        [BindNever]
        public IList<LogisticsOrderRefundType> TypeList { get; set; }

        [BindNever]
        public IList<LogisticsOrderRefundApplyStatus> StatusList { get; set; }

        [BindNever]
        public IList<ApplyDataViewScope> ScopeList { get; set; }
    }
}
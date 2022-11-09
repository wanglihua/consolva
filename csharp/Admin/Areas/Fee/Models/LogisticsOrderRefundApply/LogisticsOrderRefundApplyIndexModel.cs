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
        public IList<FeeRefundType> TypeList { get; set; }

        [BindNever]
        public IList<FeeRefundApplyStatus> StatusList { get; set; }

        [BindNever]
        public IList<ApplyDataViewScope> ScopeList { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.FeeRefundApply
{
    public class FeeRefundApplyIndexModel
    {
        [BindNever]
        public IList<FeeRefundType> TypeList { get; set; }

        [BindNever]
        public IList<FeeRefundApplyStatus> StatusList { get; set; }

        [BindNever]
        public IList<FeeApplyDataViewScope> ScopeList { get; set; }
    }
}
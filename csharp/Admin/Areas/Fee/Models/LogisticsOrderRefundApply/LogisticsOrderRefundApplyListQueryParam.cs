using System;
using Core;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApply
{
    public class LogisticsOrderRefundApplyListQueryParam : ListQueryParam
    {
        [ModelBinder(typeof(FeeRefundTypeModelBinder))]
        public FeeRefundType TypeQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        public string ReasonQuery { get; set; }

        [ModelBinder(typeof(FeeRefundApplyStatusModelBinder))]
        public FeeRefundApplyStatus StatusQuery { get; set; }

        [ModelBinder(typeof(ApplyDataViewScopeModelBinder))]
        public ApplyDataViewScope ViewScopeQuery { get; set; }
    }
}
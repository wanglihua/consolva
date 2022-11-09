using System;
using Core;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

namespace Web.Areas.Admin.Models.ManualFeeApply
{
    public class ManualFeeApplyListQueryParam : ListQueryParam
    {
        public int ManualReasonIdQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        [ModelBinder(typeof(ManualFeeApplyStatusModelBinder))]
        public ManualFeeApplyStatus StatusQuery { get; set; }

        [ModelBinder(typeof(FeeApplyDataViewScopeModelBinder))]
        public FeeApplyDataViewScope ViewScopeQuery { get; set; }
    }
}
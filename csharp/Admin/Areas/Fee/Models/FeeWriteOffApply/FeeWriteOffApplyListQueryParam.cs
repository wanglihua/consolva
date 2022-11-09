using System;
using Core;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;

namespace Web.Areas.Admin.Models.FeeWriteOffApply
{
    public class FeeWriteOffApplyListQueryParam : ListQueryParam
    {
        public int ManualReasonIdQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        [ModelBinder(typeof(FeeWriteOffApplyStatusModelBinder))]
        public FeeWriteOffApplyStatus StatusQuery { get; set; }

        [ModelBinder(typeof(ApplyDataViewScopeModelBinder))]
        public ApplyDataViewScope ViewScopeQuery { get; set; }
    }
}
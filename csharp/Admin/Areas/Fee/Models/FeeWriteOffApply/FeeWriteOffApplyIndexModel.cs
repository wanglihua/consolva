using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;

namespace Web.Areas.Admin.Models.FeeWriteOffApply
{
    public class FeeWriteOffApplyIndexModel
    {
        /// <summary>
        /// 核销费用原因列表
        /// </summary>
        [BindNever]
        public IList<ManualFeeReasonInfo> FeeReasonList { get; set; }

        [BindNever]
        public IList<FeeWriteOffApplyStatus> ApplyStatusList { get; set; }

        [BindNever]
        public IList<ApplyDataViewScope> ScopeList { get; set; }
    }
}
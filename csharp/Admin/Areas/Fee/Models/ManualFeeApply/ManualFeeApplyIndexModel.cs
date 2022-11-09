using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

namespace Web.Areas.Admin.Models.ManualFeeApply
{
    public class ManualFeeApplyIndexModel
    {
        /// <summary>
        /// 手工费用原因列表
        /// </summary>
        [BindNever]
        public IList<ManualFeeReasonInfo> FeeReasonList { get; set; }

        [BindNever]
        public IList<ManualFeeApplyStatus> ApplyStatusList { get; set; }
        
        [BindNever]
        public IList<ApplyDataViewScope> ScopeList { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderFeeWriteOffApprove;

namespace Web.Areas.Admin.Models.LogisticsOrderFeeWriteOffApprove
{
    public class LogisticsOrderFeeWriteOffApproveIndexModel
    {
        /// <summary>
        /// 核销费用原因列表
        /// </summary>
        [BindNever]
        public IList<ManualFeeReasonInfo> FeeReasonList { get; set; }

        [BindNever]
        public IList<FeeWriteOffApplyStatus> ApplyStatusList { get; set; }
        
        [BindNever]
        public IList<UserInfo> ApplyUserList { get; set; }        
    }
}
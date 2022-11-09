using System.Collections.Generic;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

namespace Web.Areas.Admin.Models.ManualFeeApprove
{
    public class ManualFeeApproveIndexModel
    {
        /// <summary>
        /// 手工费用原因列表
        /// </summary>
        [BindNever]
        public IList<ManualFeeReasonInfo> FeeReasonList { get; set; }

        [BindNever]
        public IList<ManualFeeApplyStatus> ApplyStatusList { get; set; }

        [BindNever]
        public IList<UserInfo> ApplyUserList { get; set; }
    }
}
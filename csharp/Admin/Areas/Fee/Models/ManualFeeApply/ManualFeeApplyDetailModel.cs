using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

namespace Web.Areas.Admin.Models.ManualFeeApply
{
    public class ManualFeeApplyDetailModel
    {
        public int ApplyId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Required(ErrorMessage = "订单编号不能为空！")]
        [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// 增减核销费用原因
        /// </summary>
        [Required(ErrorMessage = "减少费用原因不能为空！")]
        public virtual int ManualReasonId { get; set; }

        /// <summary>
        /// 手工费用原因列表
        /// </summary>
        [BindNever]
        public IList<ManualFeeReasonInfo> FeeReasonList { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(255, ErrorMessage = "描述长度不能大于255！")]
        public string Description { get; set; }
    }
}
using Core;
using Core.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Customer;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 订单退赔款申请
    /// </summary>
    public partial class LogisticsOrderRefundApplyInfo : Model
    {
        /// <summary>
        /// 物流订单
        /// </summary>
        [DisplayName("物流订单")]
        [Required(ErrorMessage = "物流订单不能为空！")]
        public virtual LogisticsOrderInfo Order { get; set; }

        /// <summary>
        /// 会员
        /// </summary>
        [DisplayName("会员")]
        [Required(ErrorMessage = "会员不能为空！")]
        public virtual CustomerInfo Customer { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DisplayName("类型")]
        [Required(ErrorMessage = "类型不能为空！")]
        [ModelBinder(typeof(LogisticsOrderRefundTypeModelBinder))]
        public virtual LogisticsOrderRefundType Type { get; set; }

        [Required(ErrorMessage = "退赔原因不能为空！")]
        public virtual FeeRefundReasonInfo Reason { get; set; }

        /// <summary>
        /// 退赔款原因
        /// </summary>
        [DisplayName("退赔款原因")]
        [Required(ErrorMessage = "退赔款原因不能为空！")]
        [MaxLength(300, ErrorMessage = "退赔款原因长度不能大于300！")]
        public virtual string ReasonContent { get; set; }

        /// <summary>
        /// 退赔金额
        /// </summary>
        [DisplayName("退赔金额")]
        [Required(ErrorMessage = "退赔金额不能为空！")]
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// 退赔支付方式
        /// </summary>
        [DisplayName("退赔支付方式")]
        [Required(ErrorMessage = "退赔支付方式不能为空！")]
        [ModelBinder(typeof(LogisticsOrderRefundPayTypeModelBinder))]
        public virtual LogisticsOrderRefundPayType PayType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DisplayName("状态")]
        [Required(ErrorMessage = "状态不能为空！")]
        [ModelBinder(typeof(LogisticsOrderRefundApplyStatusModelBinder))]
        public virtual LogisticsOrderRefundApplyStatus Status { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        [DisplayName("申请人")]
        [Required(ErrorMessage = "申请人不能为空！")]
        public virtual UserInfo ApplyUser { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [DisplayName("申请时间")]
        public virtual DateTime? ApplyTime { get; set; }

        /// <summary>
        /// 申请备注
        /// </summary>
        [DisplayName("申请备注")]
        [MaxLength(300, ErrorMessage = "申请备注长度不能大于300！")]
        public virtual string ApplyMemo { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DisplayName("审核人")]
        public virtual UserInfo ApproveUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DisplayName("审核时间")]
        public virtual DateTime? ApproveTime { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        [DisplayName("审核备注")]
        [MaxLength(300, ErrorMessage = "审核备注长度不能大于300！")]
        public virtual string ApproveMemo { get; set; }
    }
}
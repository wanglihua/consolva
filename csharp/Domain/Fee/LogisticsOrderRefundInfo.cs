using Core;
using Core.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 订单退赔款
    /// </summary>
    public class LogisticsOrderRefundInfo : Model
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
        [ModelBinder(typeof(LogisticsOrderRefundStatusModelBinder))]
        public virtual LogisticsOrderRefundStatus Status { get; set; }

        /// <summary>
        /// 对应申请
        /// </summary>
        [DisplayName("对应申请")]
        [Required(ErrorMessage = "对应申请不能为空！")]
        public virtual LogisticsOrderRefundApplyInfo Apply { get; set; }
    }
}
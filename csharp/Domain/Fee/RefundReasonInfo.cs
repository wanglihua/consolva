using Core;
using Core.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Domain.LogisticsOrder
{
    /// <summary>
    /// 退赔原因
    /// </summary>
    public class RefundReasonInfo : Model
    {
        /// <summary>
        /// 内容
        /// </summary>
        [DisplayName("内容")]
        [Required(ErrorMessage = "内容不能为空！")]
        [MaxLength(300, ErrorMessage = "内容长度不能大于300！")]
        public virtual string Content { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DisplayName("状态")]
        [Required(ErrorMessage = "状态不能为空！")]
        [ModelBinder(typeof(RefundReasonStatusModelBinder))]
        public virtual RefundReasonStatus Status { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端协议
/// 这个主要是订单创建时的协议
/// </summary>
public class ClientAgreementInfo : Entity {
    /// <summary>
    /// 标题，非空
    /// </summary>
    [Required(ErrorMessage = "标题不能为空！")]
    [MaxLength(200, ErrorMessage = "标题长度不能大于200！")]
    [JsonProperty("title")]
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容，非空
    /// </summary>
    [Required(ErrorMessage = "内容不能为空！")]
    [JsonProperty("content")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [MaxLength(50, ErrorMessage = "国家长度不能大于50！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 添加人，非空
    /// </summary>
    [Required(ErrorMessage = "添加人不能为空！")]
    [JsonProperty("creator")]
    public virtual UserInfo Creator { get; set; } = new UserInfo();

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ClientAgreementStatusModelBinder))]
    public virtual ClientAgreementStatus Status { get; set; } = ClientAgreementStatus.None;

    /// <summary>
    /// 发布时间，可空
    /// </summary>
    [JsonProperty("publishTime")]
    public virtual DateTime? PublishTime { get; set; } = null;

    /// <summary>
    /// 序号，非空
    /// </summary>
    [Required(ErrorMessage = "序号不能为空！")]
    [JsonProperty("orderNum")]
    public virtual int OrderNum { get; set; } = 0;
}
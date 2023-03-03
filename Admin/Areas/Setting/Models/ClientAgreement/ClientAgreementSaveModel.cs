using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientAgreement;

public class ClientAgreementSaveModel {
    [JsonProperty("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// 标题，非空
    /// </summary>
    [Required(ErrorMessage = "标题不能为空！")]
    [MaxLength(200, ErrorMessage = "标题长度不能大于200！")]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容，非空
    /// </summary>
    [Required(ErrorMessage = "内容不能为空！")]
    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public Country Country { get; set; } = Country.None;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ClientAgreementStatusModelBinder))]
    public ClientAgreementStatus Status { get; set; } = ClientAgreementStatus.None;

    /// <summary>
    /// 序号，非空
    /// </summary>
    [Required(ErrorMessage = "序号不能为空！")]
    [JsonProperty("orderNum")]
    public int OrderNum { get; set; } = 0;
}
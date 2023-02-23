using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.ExpressSizeWeightUpload;

// http://111.231.110.33:8088/ld_express/apiController.do?dzcscaleSH&orderNo=1&length=1&width=1&height=1&weight=1

public class ExpressSizeWeightUploadParam {
    [BindProperty(Name = "orderNo")]
    public string OrderNo { get; set; } = string.Empty;

    [BindProperty(Name = "length")]
    public decimal Length { get; set; }

    [BindProperty(Name = "width")]
    public decimal Width { get; set; }

    [JsonProperty("height")]
    public decimal Height { get; set; }

    [JsonProperty("weight")]
    public decimal Weight { get; set; }
}
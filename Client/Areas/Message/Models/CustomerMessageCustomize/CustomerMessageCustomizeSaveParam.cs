using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageCustomize;

public class CustomerMessageCustomizeSaveParam
{
    [Required(ErrorMessage = "消息类型不能为空！")]
    [JsonProperty("typeId")]
    public int TypeId { get; set; }

    [JsonProperty("send")]
    public bool Send { get; set; } = true;
}
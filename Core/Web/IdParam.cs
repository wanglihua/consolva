using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Core.Web;

public class IdParam
{
    [BindProperty(Name = "id")]
    [JsonProperty("id")]
    [Required(ErrorMessage = "id不能为空！")]
    public int Id { get; set; } = 0;
}
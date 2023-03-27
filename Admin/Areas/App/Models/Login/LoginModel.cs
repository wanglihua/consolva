using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.Login; 

public class LoginModel {
    [JsonProperty("userId")]
    public int UserId { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;
}

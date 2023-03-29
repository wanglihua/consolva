using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.Login;

public class LoginModel {
    [JsonProperty("userId")]
    public int UserId { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonProperty("canExpressQuery")]
    public bool CanExpressQuery { get; set; }

    [JsonProperty("canExpressOnShelf")]
    public bool CanExpressOnShelf { get; set; }

    [JsonProperty("canExpressWarehouseCheck")]
    public bool CanExpressWarehouseCheck { get; set; }

    [JsonProperty("canExpressOffShelf")]
    public bool CanExpressOffShelf { get; set; }

    [JsonProperty("canExpressReturnOffShelf")]
    public bool CanExpressReturnOffShelf { get; set; }

    [JsonProperty("canExpressDump")]
    public bool CanExpressDump { get; set; }

    [JsonProperty("canExpressPacking")]
    public bool CanExpressPacking { get; set; }

    [JsonProperty("canPackBoxQuery")]
    public bool CanPackBoxQuery { get; set; }

    [JsonProperty("canPackBoxShip")]
    public bool CanPackBoxShip { get; set; }

    [JsonProperty("canPhotoSignIn")]
    public bool CanPhotoSignIn { get; set; }

    [JsonProperty("canPhotoCheckIn")]
    public bool CanPhotoCheckIn { get; set; }

    [JsonProperty("canPhotoIssue")]
    public bool CanPhotoIssue { get; set; }

    [JsonProperty("canPhotoPack")]
    public bool CanPhotoPack { get; set; }

    [JsonProperty("canPhotoReturn")]
    public bool CanPhotoReturn { get; set; }

    [JsonProperty("canPhotoShip")]
    public bool CanPhotoShip { get; set; }

    [JsonProperty("canPhoto")]
    public bool CanPhoto { get; set; }
}
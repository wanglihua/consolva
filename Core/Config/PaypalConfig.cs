namespace shunda.Core.Config;

public class PaypalConfig {
    public static string EndPoint => "https://api.sandbox.paypal.com";
    public static string Mode => "sandbox";
    public static string ClientId => "ClientId";
    public static string ClientSecret => "ClientSecret";

    public static string ConnectionTimeout = "360000";

    public static string RequestRetries = "1";

    public static Dictionary<string, string> GetProperties() {
        return new Dictionary<string, string> {
            { "endPoint", EndPoint },
            { "mode", Mode },
            { "clientId", ClientId },
            { "clientSecret", ClientSecret },
            { "connectionTimeout", ConnectionTimeout },
            { "requestRetries", RequestRetries }
        };
    }
}
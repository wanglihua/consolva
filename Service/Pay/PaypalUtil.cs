using PayPal.Api;
using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.Pay;

public class PaypalUtil {
    /// <summary>
    /// OAuthTokenCredential对象有 access token 的缓存功能
    /// 所以做成静态成员非常合适
    /// </summary>
    private static OAuthTokenCredential oAuthTokenCredential = new OAuthTokenCredential(
        PaypalConfig.ClientId,
        PaypalConfig.ClientSecret,
        PaypalConfig.GetProperties()
    );

    /// <summary>
    /// 获取 access token 方法内已考虑缓存和获取效率
    /// </summary>
    /// <returns></returns>
    public static string GetAccessToken() {
        return oAuthTokenCredential.GetAccessToken();
    }

    public static APIContext GetApiContext(string accessToken = "") {
        // ### Api Context
        // Pass in a `APIContext` object to authenticate 
        // the call and to send a unique request id 
        // (that ensures idempotency). The SDK generates
        // a request id if you do not pass one explicitly. 

        if (string.IsNullOrWhiteSpace(accessToken)) {
            accessToken = GetAccessToken();
        }

        var apiContext = new APIContext(accessToken) {
            Config = PaypalConfig.GetProperties()
        };

        // Use this variant if you want to pass in a request id  
        // that is meaningful in your application, ideally 
        // a order id.
        // String requestId = Long.toString(System.nanoTime();
        // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

        return apiContext;
    }
}
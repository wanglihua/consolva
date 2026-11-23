namespace shunda.Core.Config;

public class AlipayConfig {
    // 这个是应用ID
    public static string AppId => "AppId";

    // 或者叫 pid, partner id，商户ID，一个商户ID可以关联多个应用ID
    public static string UId => "UId";

    // 支付宝中要开通相应的产品(电脑网站支付，手机网站支付)，创建网页应用，把网页应用和两个支付产品绑定。

    // 这个是本地用支付宝工具生成的一对公钥和私钥，其中公钥配置到支付宝，私钥配置到这里。要转成PKCS1格式
    public static string PrivateKey => "PrivateKey";

    public static string GatewayUrl => "https://openapi.alipaydev.com/gateway.do";

    // 这个是上传本地生成公钥到支付宝后，支付宝给出的支付宝公钥，是支付宝方生成的公钥，私钥应该是支付宝保存了
    public static string PublicKey => "PublicKey";

    public static string SignType => "RSA2";

    public static string CharSet => "UTF-8";
}
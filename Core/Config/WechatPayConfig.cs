namespace shunda.Core.Config;

public class WechatPayConfig {
    public static string MchId => "MchId";
    public static string Key => "Key"; // 微信商户号里面的API，KEY


    //=======【证书路径设置】=====================================
    /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
     * 1.证书文件不能放在web服务器虚拟目录，应放在有访问权限控制的目录中，防止被他人下载；
     * 2.建议将证书文件名改为复杂且不容易猜测的文件
     * 3.商户服务器要做好病毒和木马防护工作，不被非法侵入者窃取证书文件。
    */

    public static string SslCertPath => "";

    public static string SslCertPassword => "";


    //=======【支付结果通知url】=====================================
    /* 支付结果通知回调url，用于商户接收支付结果
    */

    // todo 或者可以不配置在这里

    public static string OrderPayNotifyUrl => "/client/pay/wx/notify";

    public static string ReturnPayNotifyUrl => "/client/express/return/pay/wx/notify";

    public static string ChargePayNotifyUrl => "/client/charge/wx/notify";

    //=======【商户系统后台机器IP】=====================================
    /* 此参数可手动配置也可在程序中自动获取
    */
    public static string Ip => "0.0.0.0";


    //=======【代理服务器设置】===================================
    /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
    */
    public static string ProxyUrl => "";


    //=======【上报信息配置】===================================
    /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
    */
    public static int ReportLevel => 0;


    //=======【日志级别】===================================
    /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
    */
    public static int LogLevel => 1;
}
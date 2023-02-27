using NLog;
using shunda.Core.Config;

namespace shunda.Core.WechatPay;

public class WechatPayApi {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    /**
        * 提交被扫支付API
        * 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
        * 由商户收银台或者商户后台调用该接口发起支付。
        * @param WxPayData inputObj 提交给被扫支付API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回调用结果，其他抛异常
        */
    public static WechatPayData Micropay(WechatPayData inputObj, int timeOut = 10) {
        string url = "https://api.mch.weixin.qq.com/pay/micropay";
        //检测必填参数
        if (!inputObj.IsSet("body")) {
            throw new WechatPayException("提交被扫支付API接口中，缺少必填参数body！");
        }

        if (!inputObj.IsSet("out_trade_no")) {
            throw new WechatPayException("提交被扫支付API接口中，缺少必填参数out_trade_no！");
        }

        if (!inputObj.IsSet("total_fee")) {
            throw new WechatPayException("提交被扫支付API接口中，缺少必填参数total_fee！");
        }

        if (!inputObj.IsSet("auth_code")) {
            throw new WechatPayException("提交被扫支付API接口中，缺少必填参数auth_code！");
        }

        inputObj.SetValue("spbill_create_ip", SiteConfig.SiteIP); //终端ip

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", "")); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名
        string xml = inputObj.ToXml();

        string response = HttpService.Post(xml, url, false, timeOut); //调用HTTP通信接口以提交数据到API

        //将xml格式的结果转换为对象以返回
        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 查询订单
        * @param WxPayData inputObj 提交给查询订单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回订单查询结果，其他抛异常
        */
    public static WechatPayData OrderQuery(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/pay/orderquery";
        //检测必填参数
        if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id")) {
            throw new WechatPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();
        string response = HttpService.Post(xml, url, false, timeOut); //调用HTTP通信接口提交数据

        //将xml格式的数据转化为对象以返回
        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 撤销订单API接口
        * @param WxPayData inputObj 提交给撤销订单API接口的参数，out_trade_no和transaction_id必填一个
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回API调用结果，其他抛异常
        */
    public static WechatPayData Reverse(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
        //检测必填参数
        if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id")) {
            throw new WechatPayException("撤销订单API接口中，参数out_trade_no和transaction_id必须填写一个！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();
        string response = HttpService.Post(xml, url, true, timeOut);

        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 申请退款
        * @param WxPayData inputObj 提交给申请退款API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回接口调用结果，其他抛异常
        */
    public static WechatPayData Refund(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
        //检测必填参数
        if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id")) {
            throw new WechatPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
        }

        if (!inputObj.IsSet("out_refund_no")) {
            throw new WechatPayException("退款申请接口中，缺少必填参数out_refund_no！");
        }

        if (!inputObj.IsSet("total_fee")) {
            throw new WechatPayException("退款申请接口中，缺少必填参数total_fee！");
        }

        if (!inputObj.IsSet("refund_fee")) {
            throw new WechatPayException("退款申请接口中，缺少必填参数refund_fee！");
        }

        if (!inputObj.IsSet("op_user_id")) {
            throw new WechatPayException("退款申请接口中，缺少必填参数op_user_id！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", "")); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();
        string response = HttpService.Post(xml, url, true, timeOut); //调用HTTP通信接口提交数据到API

        //将xml格式的结果转换为对象以返回
        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 查询退款
        * 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
        * 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        * out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
        * @param WxPayData inputObj 提交给查询退款API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
    public static WechatPayData RefundQuery(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/pay/refundquery";
        //检测必填参数
        if (!inputObj.IsSet("out_refund_no") && !inputObj.IsSet("out_trade_no") &&
            !inputObj.IsSet("transaction_id") && !inputObj.IsSet("refund_id")) {
            throw new WechatPayException("退款查询接口中，out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();
        string response = HttpService.Post(xml, url, false, timeOut); //调用HTTP通信接口以提交数据到API

        //将xml格式的结果转换为对象以返回
        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        * 下载对账单
        * @param WxPayData inputObj 提交给下载对账单API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
    public static WechatPayData DownloadBill(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/pay/downloadbill";
        //检测必填参数
        if (!inputObj.IsSet("bill_date")) {
            throw new WechatPayException("对账单接口中，缺少必填参数bill_date！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();

        log.Debug("WxPayApi", "DownloadBill request : " + xml);
        string response = HttpService.Post(xml, url, false, timeOut); //调用HTTP通信接口以提交数据到API
        log.Debug("WxPayApi", "DownloadBill result : " + response);

        WechatPayData result = new WechatPayData();
        //若接口调用失败会返回xml格式的结果
        if (response.Substring(0, 5) == "<xml>") {
            result.FromXml(response);
        }
        //接口调用成功则返回非xml格式的数据
        else {
            result.SetValue("result", response);
        }

        return result;
    }


    /**
        *
        * 转换短链接
        * 该接口主要用于扫码原生支付模式一中的二维码链接转成短链接(weixin://wxpay/s/XXXXXX)，
        * 减小二维码数据量，提升扫描速度和精确度。
        * @param WxPayData inputObj 提交给转换短连接API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
    public static WechatPayData ShortUrl(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/tools/shorturl";
        //检测必填参数
        if (!inputObj.IsSet("long_url")) {
            throw new WechatPayException("需要转换的URL，签名用原串，传输需URL encode！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名

        string xml = inputObj.ToXml();
        string response = HttpService.Post(xml, url, false, timeOut);

        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
    public static WechatPayData UnifiedOrder(WechatPayData data, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        //检测必填参数
        if (!data.IsSet("out_trade_no")) {
            throw new WechatPayException("缺少统一支付接口必填参数out_trade_no！");
        }

        if (!data.IsSet("body")) {
            throw new WechatPayException("缺少统一支付接口必填参数body！");
        }

        if (!data.IsSet("total_fee")) {
            throw new WechatPayException("缺少统一支付接口必填参数total_fee！");
        }

        if (!data.IsSet("trade_type")) {
            throw new WechatPayException("缺少统一支付接口必填参数trade_type！");
        }

        //关联参数
        string tradeType = data.GetValue("trade_type")?.ToString() ?? string.Empty; 
        if (tradeType == "JSAPI" && !data.IsSet("openid")) {
            throw new WechatPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
        }

        if (tradeType == "NATIVE" && !data.IsSet("product_id")) {
            throw new WechatPayException("统一支付接口中，缺少必填参数product_id！trade_type为NATIVE时，product_id为必填参数！");
        }

        //异步通知url未设置，则使用配置文件中的url
        if (!data.IsSet("notify_url")) {
            throw new WechatPayException("微信支付 notify_url 未设置！");
        }

        if (data.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        data.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        data.SetValue("spbill_create_ip", SiteConfig.SiteIP); //终端ip
        data.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        data.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型

        //签名
        data.SetValue("sign", data.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256));

        string xml = data.ToXml();
        string response = HttpService.Post(xml, url, false, timeOut);

        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 关闭订单
        * @param WxPayData inputObj 提交给关闭订单API的参数
        * @param int timeOut 接口超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
    public static WechatPayData CloseOrder(WechatPayData inputObj, int timeOut = 6) {
        string url = "https://api.mch.weixin.qq.com/pay/closeorder";
        //检测必填参数
        if (!inputObj.IsSet("out_trade_no")) {
            throw new WechatPayException("关闭订单接口中，out_trade_no必填！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名
        string xml = inputObj.ToXml();

        string response = HttpService.Post(xml, url, false, timeOut);

        WechatPayData result = new WechatPayData();
        result.FromXml(response);

        return result;
    }

    /**
        *
        * 测速上报接口实现
        * @param WxPayData inputObj 提交给测速上报接口的参数
        * @param int timeOut 测速上报接口超时时间
        * @throws WxPayException
        * @return 成功时返回测速上报接口返回的结果，其他抛异常
        */
    public static WechatPayData Report(WechatPayData inputObj, int timeOut = 1) {
        string url = "https://api.mch.weixin.qq.com/payitil/report";
        //检测必填参数
        if (!inputObj.IsSet("interface_url")) {
            throw new WechatPayException("接口URL，缺少必填参数interface_url！");
        }

        if (!inputObj.IsSet("return_code")) {
            throw new WechatPayException("返回状态码，缺少必填参数return_code！");
        }

        if (!inputObj.IsSet("result_code")) {
            throw new WechatPayException("业务结果，缺少必填参数result_code！");
        }

        if (!inputObj.IsSet("execute_time_")) {
            throw new WechatPayException("接口耗时，缺少必填参数execute_time_！");
        }

        if (inputObj.IsSet("appid") == false) {
            throw new WechatPayException("微信支付 appid 未设置！");
        }

        inputObj.SetValue("mch_id", WechatPayConfig.MchId); //商户号
        inputObj.SetValue("user_ip", SiteConfig.SiteIP); //终端ip
        inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss")); //商户上报时间
        inputObj.SetValue("nonce_str", GenerateNonceStr()); //随机字符串
        inputObj.SetValue("sign_type", WechatPayData.SIGN_TYPE_HMAC_SHA256); //签名类型
        inputObj.SetValue("sign", inputObj.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256)); //签名
        string xml = inputObj.ToXml();

        string response = HttpService.Post(xml, url, false, timeOut);

        WechatPayData result = new WechatPayData();
        result.FromXml(response);
        return result;
    }

    /**
        * 根据当前系统时间加随机序列来生成订单号
         * @return 订单号
        */
    public static string GenerateOutTradeNo() {
        var ran = new Random();
        return $"{WechatPayConfig.MchId}{DateTime.Now.ToString("yyyyMMddHHmmss")}{ran.Next(999)}";
    }

    /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
    public static string GenerateTimeStamp() {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
    public static string GenerateNonceStr() {
        RandomGenerator randomGenerator = new RandomGenerator();
        return randomGenerator.GetRandomUInt().ToString();
    }
}
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using NLog;
using shunda.Core.Config;

namespace shunda.Core.WechatPay;

/// <summary>
/// 微信支付协议接口数据类，所有的API接口通信都依赖这个数据结构，
/// 在调用接口之前先填充各个字段的值，然后进行接口通信，
/// 这样设计的好处是可扩展性强，用户可随意对协议进行更改而不用重新设计数据结构，
/// 还可以随意组合出不同的协议数据包，不用为每个协议设计一个数据包结构
/// </summary>
public class WechatPayData {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public const string SIGN_TYPE_MD5 = "MD5";
    public const string SIGN_TYPE_HMAC_SHA256 = "HMAC-SHA256";

    //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
    private SortedDictionary<string, object> values = new SortedDictionary<string, object>();

    /// <summary>
    /// 设置某个字段的值
    /// </summary>
    /// <param name="key">字段名</param>
    /// <param name="value">字段值</param>
    public void SetValue(string key, object value) {
        values[key] = value;
    }

    /// <summary>
    /// 根据字段名获取某个字段的值
    /// </summary>
    /// <param name="key">字段名</param>
    /// <returns>key对应的字段值</returns>
    public object? GetValue(string key) {
        values.TryGetValue(key, out object? o);
        return o;
    }

    /// <summary>
    /// 判断某个字段是否已设置
    /// </summary>
    /// <param name="key">字段名</param>
    /// <returns>若字段key已被设置，则返回true，否则返回false</returns>
    public bool IsSet(string key) {
        values.TryGetValue(key, out object? o);
        if (null != o) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// 将Dictionary转成xml
    /// </summary>
    /// <returns>经转换得到的xml串</returns>
    /// <exception cref="WechatPayException">WxPayException</exception>
    public string ToXml() {
        //数据为空时不能转化为xml格式
        if (0 == values.Count) {
            log.Error("WxPayData数据为空!");
            throw new WechatPayException("WxPayData数据为空!");
        }

        string xml = "<xml>";
        foreach (KeyValuePair<string, object> pair in values) {
            //字段值不能为null，会影响后续流程
            if (pair.Value == null) {
                log.Error("WxPayData内部含有值为null的字段!");
                throw new WechatPayException("WxPayData内部含有值为null的字段!");
            }

            if (pair.Value is int) {
                xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
            } else if (pair.Value is string) {
                xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
            } else //除了string和int类型不能含有其他数据类型
            {
                log.Error("WxPayData字段数据类型错误!");
                throw new WechatPayException("WxPayData字段数据类型错误!");
            }
        }

        xml += "</xml>";
        return xml;
    }

    /// <summary>
    /// 将xml转为WxPayData对象并返回对象内部的数据
    /// </summary>
    /// <param name="xml">待转换的xml串</param>
    /// <returns>经转换得到的Dictionary</returns>
    /// <exception cref="WechatPayException">WxPayException</exception>
    public SortedDictionary<string, object> FromXml(string xml) {
        if (string.IsNullOrEmpty(xml)) {
            log.Error("将空的xml串转换为WxPayData不合法!");
            throw new WechatPayException("将空的xml串转换为WxPayData不合法!");
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        XmlNode xmlNode = xmlDoc.FirstChild!; //获取到根节点<xml>
        XmlNodeList nodes = xmlNode.ChildNodes;
        foreach (XmlNode xn in nodes) {
            XmlElement xe = (XmlElement)xn;
            values[xe.Name] = xe.InnerText; //获取xml的键值对到WxPayData内部的数据中
        }

        try {
            //2015-06-29 错误是没有签名
            if (values["return_code"].ToString() != "SUCCESS") {
                // return values;
                log.Error(xml);
                throw new WechatPayException("数据返回代码 return_code 不是 SUCCESS！" + xml);
            }

            CheckSign(); //验证签名,不通过会抛异常
        }
        catch (WechatPayException ex) {
            throw new WechatPayException(ex.Message);
        }

        return values;
    }

    /// <summary>
    /// Dictionary格式转化成url参数格式
    /// </summary>
    /// <returns>url格式串, 该串不包含sign字段值</returns>
    /// <exception cref="WechatPayException"></exception>
    public string ToUrl() {
        string buff = "";
        foreach (KeyValuePair<string, object> pair in values) {
            if (pair.Value == null) {
                log.Error("WxPayData内部含有值为null的字段!");
                throw new WechatPayException("WxPayData内部含有值为null的字段!");
            }

            if (pair.Key != "sign" && pair.Value.ToString() != "") {
                buff += pair.Key + "=" + pair.Value + "&";
            }
        }

        buff = buff.Trim('&');
        return buff;
    }

    /// <summary>
    /// values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
    /// </summary>
    /// <returns></returns>
    /// <exception cref="WechatPayException"></exception>
    public string ToPrintStr() {
        string str = "";
        foreach (KeyValuePair<string, object> pair in values) {
            if (pair.Value == null) {
                log.Error("WxPayData内部含有值为null的字段!");
                throw new WechatPayException("WxPayData内部含有值为null的字段!");
            }


            str += string.Format("{0}={1}\n", pair.Key, pair.Value.ToString());
        }

        str = HttpUtility.HtmlEncode(str);
        log.Debug("Print in Web Page : " + str);
        return str;
    }

    /// <summary>
    /// 生成签名，详见签名生成算法
    /// </summary>
    /// <param name="signType">签名, sign字段不参加签名</param>
    /// <returns></returns>
    /// <exception cref="WechatPayException"></exception>
    public string MakeSign(string signType) {
        //转url格式
        string str = ToUrl();
        //在string后加入API KEY
        str += "&key=" + WechatPayConfig.Key;
        if (signType == SIGN_TYPE_MD5) {
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs) {
                sb.Append(b.ToString("X2"));
            }

            //所有字符转为大写
            return sb.ToString().ToUpper();
        } else if (signType == SIGN_TYPE_HMAC_SHA256) {
            return CalcHMACSHA256Hash(str, WechatPayConfig.Key).ToUpper();
        } else {
            throw new WechatPayException("sign_type 不合法");
        }
    }

    /// <summary>
    /// 生成签名，详见签名生成算法
    /// </summary>
    /// <returns>签名, sign字段不参加签名 SHA256</returns>
    public string MakeSign() {
        return MakeSign(SIGN_TYPE_HMAC_SHA256);
    }

    /// <summary>
    /// 检测签名是否正确
    /// </summary>
    /// <param name="signType"></param>
    /// <returns>正确返回true，错误抛异常</returns>
    /// <exception cref="WechatPayException"></exception>
    public bool CheckSign(string signType) {
        //如果没有设置签名，则跳过检测
        if (IsSet("sign") == false) {
            log.Error("WxPayData签名不存在!");
            throw new WechatPayException("WxPayData签名不存在!");
        }

        //如果设置了签名但是签名为空，则抛异常
        object? signObj = GetValue("sign");

        string sign = signObj?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sign)) {
            log.Error("WxPayData签名存在但不合法!");
            throw new WechatPayException("WxPayData签名存在但不合法!");
        }

        //在本地计算新的签名
        string calSign = MakeSign(signType);

        if (calSign == sign) {
            return true;
        }

        log.Error("WxPayData签名验证错误!");
        throw new WechatPayException("WxPayData签名验证错误!");
    }

    /// <summary>
    /// 检测签名是否正确
    /// </summary>
    /// <returns>正确返回true，错误抛异常</returns>
    public bool CheckSign() {
        return CheckSign(SIGN_TYPE_HMAC_SHA256);
    }

    /// <summary>
    /// 获取Dictionary
    /// </summary>
    /// <returns></returns>
    public SortedDictionary<string, object> GetValues() {
        return values;
    }

    private string CalcHMACSHA256Hash(string plaintext, string salt) {
        var encoding = Encoding.UTF8;
        byte[] baText2BeHashed = encoding.GetBytes(plaintext);
        byte[] baSalt = encoding.GetBytes(salt);

        HMACSHA256 hasher = new HMACSHA256(baSalt);
        byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);

        return string.Join("", baHashedText.ToList().Select(b => b.ToString("X2")).ToArray());
    }
}
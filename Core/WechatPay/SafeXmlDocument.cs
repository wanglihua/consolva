using System.Xml;

namespace shunda.Core.WechatPay;

public class SafeXmlDocument : XmlDocument {
    public SafeXmlDocument() {
        this.XmlResolver = null;
    }
}
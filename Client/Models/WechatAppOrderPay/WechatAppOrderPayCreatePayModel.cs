using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppOrderPay; 

public class WechatAppOrderPayCreatePayModel {

   [JsonProperty("timestamp")]
   public string TimeStamp { get; set; } = string.Empty;

}

/*

   'timestamp': '',
   'noncestr': '',
   'package': '',
   'signtype': 'md5',
   'paysign': '',

*/
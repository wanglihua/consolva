using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Message;
#if !DEBUG
using shunda.Domain;
using System.Net.Http.Json;
using shunda.Core.Config;
#endif

namespace shunda.Service.CustomerMessage
{
    [Service(ServiceType.Scoped)]
    public class SmsService
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private ISession session;
        private MobileService mobileService;
        private SiteErrorService siteErrorService;

        public SmsService(
            ISession session,
            MobileService mobileService,
            SiteErrorService siteErrorService)
        {
            this.session = session;
            this.mobileService = mobileService;
            this.siteErrorService = siteErrorService;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">不加国家代号的手机号码</param>
        /// <param name="message">message</param>
        /// <returns></returns>
        public SendSmsResult SendSms(string mobile, string message)
        {
            if (mobileService.IsCorrectMobile(mobile) == false)
            {
                var result = new SendSmsResult(SendSmsResult.MobileFormatError, "不是正确的电话号码格式！", "");

                AddSmsSendRecord(mobile, message, result);

                return result;
            }

            mobile = "61" + mobile;
            message = "【舜达集运】" + message;

#if DEBUG
            log.Trace("SendSms: ");
            log.Trace($"mobile: {mobile}");
            log.Trace($"message: {message}");

            var sendResult = new SendSmsResult(SendSmsResult.CodeSuccess, "发送成功！", "");
            AddSmsSendRecord(mobile, message, sendResult);

            return sendResult;

#else
            string senderId = SmsConfig.SenderId;
            string account = SmsConfig.Account;
            string password = SmsConfig.Password;

            string url = "http://intapi.253.com/send/json";

            HttpClient httpClient = new HttpClient();

            JsonContent jsonContent = JsonContent.Create(new SendSmsRequest(senderId, account, password, mobile, message));

            Task<HttpResponseMessage> taskResponseMessage = httpClient.PostAsync(url, jsonContent);

            taskResponseMessage.Wait();
            HttpResponseMessage responseMessage = taskResponseMessage.Result;

            if (responseMessage.IsSuccessStatusCode == false)
            {
                var sendResult = new SendSmsResult(SendSmsResult.CodeRequestFailed, "短信服务器请求未成功！", "");
                AddSmsSendRecord(mobile, message, sendResult);

                return sendResult;
            }

            Task<Stream> taskStream = responseMessage.Content.ReadAsStreamAsync();
            taskStream.Wait();
            Stream dataStream = taskStream.Result;
            StreamReader reader = new StreamReader(dataStream);
            string response = reader.ReadToEnd();

            SendSmsResult sendSmsResult = JsonConvert.DeserializeObject<SendSmsResult>(response);

            if (sendSmsResult.Code == SendSmsResult.CodeInsufficientBalance)
            {
                log.Error("短信发送接口账户余额不足，请紧急充值！");
                siteErrorService.AddSiteError(SiteErrorType.SmsSend, "短信发送接口账户余额不足，请紧急充值！");

                // 余额不足的话进行 邮件通知
                // smtpMailService.SendMail(smsConfig.NoticeEmailAddress, "账户余额不足", "253账户余额不足了！");
            }

            AddSmsSendRecord(mobile, message, sendSmsResult);

            if (sendSmsResult.Code != SendSmsResult.CodeSuccess)
            {
                throw new Exception("短信发送失败，请紧急联系管理员或客服！");
            }

            return sendSmsResult;
#endif
        }

        private void AddSmsSendRecord(string mobile, string message, SendSmsResult sendSmsResult)
        {
            var smsSendRecord = new SmsSendRecordInfo() {
                Mobile = mobile,
                Message = message,
                SendCode = sendSmsResult.Code,
                SendError = sendSmsResult.Error,
                SendMsgId = sendSmsResult.MessageId,
            };

            session.Save(smsSendRecord);
            session.Flush();
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">不加国家代号的手机号码</param>
        /// <param name="message">message</param>
        /// <returns></returns>
        public CustomerMessageStatus SendSmsMessage(string mobile, string message)
        {
            var result = SendSms(mobile, message);
            if (result.Code != SendSmsResult.CodeSuccess)
            {
                return CustomerMessageStatus.SendFail;
            }

            return CustomerMessageStatus.Sent;
        }
    }

    public class SendSmsRequest
    {
        public SendSmsRequest()
        {
        }

        public SendSmsRequest(string senderId, string account, string password, string mobile, string message)
        {
            this.SenderId = senderId;
            this.Account = account;
            this.Password = password;
            this.Mobile = mobile;
            this.Message = message;
        }

        [JsonProperty("senderId")]
        public string SenderId { get; set; } = string.Empty;

        [JsonProperty("account")]
        public string Account { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("mobile")]
        public string Mobile { get; set; } = string.Empty;

        [JsonProperty("msg")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 发送短信的结果
    ///
    /// 返回的 json 文本类似：
    /// {"code": "114", "error":"客户端IP错误", "msgid":""}
    /// </summary>
    public class SendSmsResult
    {
        /// <summary>
        /// 调用成功的代码
        /// </summary>
        public static readonly string CodeSuccess = "0";

        // insufficient balance

        /// <summary>
        /// 余额不足错误的代码
        /// </summary>
        public static readonly string CodeInsufficientBalance = "110";

        /// <summary>
        /// 客户端IP错误的代码
        /// </summary>
        public static readonly string CodeInvalidClientIp = "114";

        /// <summary>
        /// 请求不成功的代码
        /// 这个代码是本地定义的，不是服务器端的定义
        /// </summary>
        public static readonly string CodeRequestFailed = "-1";

        /// <summary>
        /// 手机格式不对，不是澳洲手机
        /// 这个代码是本地定义的，不是服务器端的定义
        /// </summary>
        public static readonly string MobileFormatError = "-2";

        public SendSmsResult()
        {
        }

        public SendSmsResult(string code, string error, string messageId)
        {
            this.Code = code;
            this.Error = error;
            this.MessageId = messageId;
        }

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("error")]
        public string Error { get; set; } = string.Empty;

        [JsonProperty("msgid")]
        public string MessageId { get; set; } = string.Empty;
    }
}

/*

    //Interface URL Used to send SMS
    const API_SEND_URL='http://intapi.253.com/send/json?';

    //Interface URL Used to Query SMS balance
    const API_BALANCE_QUERY_URL='http://intapi.253.com/balance/json?';

    const API_ACCOUNT='I1642791';//Get SMS Account  from  https://zz.253.com/site/login.html

    const API_PASSWORD='BF4keab3P';//Get SMS Password  from https://zz.253.com/site/login.html

    public function sendInternational( $mobile, $msg) {
        //创蓝接口参数
        $postArr = array (
        'account'  =>  self::API_ACCOUNT,
        'password' => self::API_PASSWORD,
        'msg' => $msg,
        'mobile' => $mobile,
        'senderId'=>'iepoch',
        );

        $result = $this->curlPost( self::API_SEND_URL , $postArr);
        return $result;
    }

*/
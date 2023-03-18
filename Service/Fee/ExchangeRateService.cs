using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Fee;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class ExchangeRateService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly HttpRequestService httpRequestService;
    private readonly SiteErrorService siteErrorService;

    public ExchangeRateService(ISession session,
        HttpRequestService httpRequestService,
        SiteErrorService siteErrorService) {
        this.session = session;
        this.httpRequestService = httpRequestService;
        this.siteErrorService = siteErrorService;
    }

    /// <summary>
    /// 转人民币未修正过的汇率
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public decimal GetRate(CurrencyType currency) {
        ExchangeRateInfo? exchangeRate = session.Query<ExchangeRateInfo>()
            .Where(e => e.FromCurrency == currency && e.ToCurrency == CurrencyType.CNY)
            .Take(1)
            .SingleOrDefault();

        if (exchangeRate == null) {
            throw new Exception("指定的汇率换算不存在");
        }

        return exchangeRate.Rate;
    }

    /// <summary>
    /// 转人民币修正过的最终汇率
    /// 修正过的最终汇率
    /// </summary>
    /// <returns></returns>
    public decimal GetFinalRate(CurrencyType currency) {
        ExchangeRateInfo? exchangeRate = session.Query<ExchangeRateInfo>()
            .Where(e => e.FromCurrency == currency && e.ToCurrency == CurrencyType.CNY)
            .Take(1)
            .SingleOrDefault();

        if (exchangeRate == null) {
            throw new Exception("指定的汇率换算不存在");
        }

        decimal audRate = exchangeRate.Rate + exchangeRate.RateFix;

        if (audRate <= 0) {
            throw new Exception("汇率不能小于或等于0，汇率值不合法！");
        }

        return audRate;
    }

    public void RefreshRate() {
        var currencyListWithoutCny = CurrencyType.GetList().Where(e => e != CurrencyType.CNY).ToList();

        foreach (var currency in currencyListWithoutCny) {
            RefreshSingleRate(currency, CurrencyType.CNY);
            // RefreshSingleRate(CurrencyType.CNY, currency);
        }
    }

    public void RefreshSingleRate(CurrencyType currencyTypeFrom, CurrencyType currencyTypeTo) {
        if (currencyTypeFrom == null || currencyTypeFrom == CurrencyType.None) {
            throw new ValidationException("汇率货币From不能为空！");
        }

        if (currencyTypeTo == null || currencyTypeTo == CurrencyType.None) {
            throw new ValidationException("汇率货币To不能为空！");
        }

        // 技术参考 https://market.aliyun.com/products/57000002/cmapi011221.html#sku=yuncode522100004

        string url = $"https://jisuhuilv.market.alicloudapi.com/exchange/convert?amount=1&from={currencyTypeFrom.Code}&to={currencyTypeTo.Code}";
        Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Authorization", string.Format("APPCODE {0}", ExchangeRateConfig.AppCode) },
        };

        RefreshRateResponse response = httpRequestService.GetJsonObject<RefreshRateResponse>(headers, url);

        if (response.Status != "0") {
            string errorMsg = "更新汇率出现错误:" + response.Msg;

            siteErrorService.AddSiteError(SiteErrorType.ExchangeRate, errorMsg);
            throw new Exception(errorMsg);
        }

        decimal rate = decimal.Parse(response.Result.Rate);

        ExchangeRateInfo? exchangeRate = session
            .Query<ExchangeRateInfo>()
            .Where(e => e.FromCurrency == currencyTypeFrom
                        && e.ToCurrency == currencyTypeTo)
            .Take(1)
            .SingleOrDefault();

        Transactional.Wrap(session, () => {
            if (exchangeRate == null) {
                exchangeRate = new ExchangeRateInfo {
                    FromCurrency = currencyTypeFrom,
                    ToCurrency = currencyTypeTo,
                    Rate = rate,
                    RateFix = 0m,
                    RefreshMark = 0
                };

                session.Save(exchangeRate);
            } else {
                exchangeRate.Rate = rate;
                exchangeRate.RefreshMark = (exchangeRate.RefreshMark + 1) % 1000000;

                session.Update(exchangeRate);
            }

            session.Flush();
        });
    }

    // {"status":"0","msg":"ok","result":{"from":"AUD","to":"CNY","fromname":"澳大利亚元","toname":"人民币","updatetime":"2019-01-04 20:40:00","rate":"4.8348","camount":"4.8348"}}
    private class RefreshRateResponse {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonProperty("result")]
        public Result Result { get; set; } = new Result();
    }

    private class Result {
        [JsonProperty("from")]
        public string From { get; set; } = string.Empty;

        [JsonProperty("to")]
        public string To { get; set; } = string.Empty;

        [JsonProperty("fromname")]
        public string FromName { get; set; } = string.Empty;

        [JsonProperty("toname")]
        public string ToName { get; set; } = string.Empty;

        [JsonProperty("updatetime")]
        public string UpdateTime { get; set; } = string.Empty;

        [JsonProperty("rate")]
        public string Rate { get; set; } = string.Empty;

        [JsonProperty("camount")]
        public string Amount { get; set; } = string.Empty;
    }
}


/*

     获取费率
     购买地址：https://market.aliyun.com/products/57000002/cmapi011221.html?spm=5176.2020520132.101.5.84dc72181Dn155#sku=yuncode522100004


    $host = "https://jisuhuilv.market.alicloudapi.com";
    $path = "/exchange/convert";
    $method = "GET";
    $appcode = "e5a706cd7a36488e87a68c818cef0b06";
    $headers = array();
    array_push($headers, "Authorization:APPCODE " . $appcode);
    $querys = "amount=1&from=AUD&to=CNY";
    $bodys = "";
    $url = $host . $path . "?" . $querys;

    $curl = curl_init();
    curl_setopt($curl, CURLOPT_CUSTOMREQUEST, $method);
    curl_setopt($curl, CURLOPT_URL, $url);
    curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);
    curl_setopt($curl, CURLOPT_FAILONERROR, false);
    curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($curl, CURLOPT_HEADER, false);

    if (1 == strpos("$".$host, "https://"))
    {
        curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, false);
    }
    $temp = curl_exec($curl);
    $temp = json_decode($temp);
    return $temp->result->rate;

}

 */
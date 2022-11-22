using NHibernate;
using NLog;
using shunda.Core.Service;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressCompanyService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public string GetExpressCompanyName(string trackingNo)
    {
        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            return string.Empty;
        }

        // 根据快递单公司快递单号规则，进行快递公司判断
        if (trackingNo.StartsWith("7") && trackingNo.Length == 15)
        {
            return "申通快递";
        }

        if (trackingNo.StartsWith("5") && trackingNo.Length == 15)
        {
            return "百世快递";
        }

        if (trackingNo.StartsWith("4") && trackingNo.Length == 13)
        {
            return "韵达快递";
        }

        if (trackingNo.StartsWith("SF") && trackingNo.Length == 15)
        {
            return "顺丰快递";
        }

        if (trackingNo.StartsWith("7") && trackingNo.Length == 14)
        {
            return "中通快递";
        }

        if (trackingNo.StartsWith("TT") && trackingNo.Length == 15)
        {
            return "天天快递";
        }

        if (trackingNo.StartsWith("YT") && trackingNo.Length == 15)
        {
            return "圆通速递";
        }

        if (trackingNo.StartsWith("9") && trackingNo.Length == 13)
        {
            return "中国邮政";
        }

        if (trackingNo.StartsWith("JD") && trackingNo.Length == 21)
        {
            return "京东快递";
        }

        if (trackingNo.StartsWith("61") && trackingNo.Length == 15)
        {
            return "丹鸟快递";
        }

        if (trackingNo.StartsWith("ZJS") && trackingNo.Length == 15)
        {
            return "宅急送";
        }

        if (trackingNo.StartsWith("DPK") && trackingNo.Length == 15)
        {
            return "德邦物流";
        }

        if (trackingNo.StartsWith("JT") && trackingNo.Length == 15)
        {
            return "极兔速递";
        }

        if (trackingNo.StartsWith("90") && trackingNo.Length == 12)
        {
            return "优速快递";
        }

        return "未知物流";
    }
}
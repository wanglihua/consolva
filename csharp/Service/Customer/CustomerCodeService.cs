using System.Text.RegularExpressions;
using shunda.Core.Service;

namespace shunda.Service.Customer;

// todo 考虑客户编号后缀，一个固定长度的后缀，用于客户ID方面的校验。两位三位四位都可以，生成是可以去掉不吉利的数字

[Service(ServiceType.Scoped)]
public class CustomerCodeService
{
    private static readonly Regex regexCustomerId = new Regex(@"[0-9]+", RegexOptions.Compiled);

    public int GetCustomerIdFromCustomerCode(string customerIdOrCode)
    {
        if (string.IsNullOrWhiteSpace(customerIdOrCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return 0;
        }

        MatchCollection matches = regexCustomerId.Matches(customerIdOrCode);

        IList<string> matchDigitList = new List<string>();
        foreach (Match match in matches)
        {
            matchDigitList.Add(match.Value);
        }

        if (matchDigitList.Count != 0)
        {
            string? maxLengthDigit = matchDigitList.OrderByDescending(e => e.Length).Take(1).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(maxLengthDigit) == false && int.TryParse(maxLengthDigit, out int customerId))
            {
                return customerId;
            }
        }

        return 0;
    }

    public int GetGuestIdFromGuestCode(string guestIdOrCode)
    {
        if (string.IsNullOrWhiteSpace(guestIdOrCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return 0;
        }

        MatchCollection matches = regexCustomerId.Matches(guestIdOrCode);

        IList<string> matchDigitList = new List<string>();
        foreach (Match match in matches)
        {
            matchDigitList.Add(match.Value);
        }

        if (matchDigitList.Count != 0)
        {
            string? maxLengthDigit = matchDigitList.OrderByDescending(e => e.Length).Take(1).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(maxLengthDigit) == false && int.TryParse(maxLengthDigit, out int guestId))
            {
                return guestId;
            }
        }

        return 0;
    }

    public string GetCustomerCode(int customerId)
    {
        return "SD" + customerId.ToString("#000");
    }
}
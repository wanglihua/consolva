using System.Text.RegularExpressions;
using shunda.Core.Service;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerCodeService
{
    private static readonly string digitsPattern = @"[0-9]+";

    public int GetCustomerIdFromCustomerCode(string customerIdOrCode)
    {
        if (string.IsNullOrWhiteSpace(customerIdOrCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return 0;
        }

        MatchCollection matches = Regex.Matches(customerIdOrCode, digitsPattern);

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

        MatchCollection matches = Regex.Matches(guestIdOrCode, digitsPattern);

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
}
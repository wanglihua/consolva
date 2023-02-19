using System.Text.RegularExpressions;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerCodeService {
    private readonly ISession session;
    private readonly CustomerCheckinCodeService customerCheckinCodeService;

    public CustomerCodeService(ISession session, CustomerCheckinCodeService customerCheckinCodeService) {
        this.session = session;
        this.customerCheckinCodeService = customerCheckinCodeService;
    }

    private static readonly Regex REGEX_CUSTOMER_ID = new Regex(@"[0-9]+", RegexOptions.Compiled);

    private static readonly int PARSE_FAIL_CUSTOMER_ID = 0;

    /// <summary>
    /// 解析类似 SDABCDE001, SDABCDE, SDQQABCDE, SDQQABCDE001 等的客户ID，可以有数字但必须要有校验码
    /// 实际如果写成这样也能识别的 SD001ABCDE, SDQQ001ABCDE
    /// </summary>
    /// <param name="customerIdOrCode"></param>
    /// <returns></returns>
    public int GetCustomerIdFromCustomerCode(string customerIdOrCode) {
        if (string.IsNullOrWhiteSpace(customerIdOrCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return 0;
        }

        customerIdOrCode = customerIdOrCode.ToUpper(); // 转成大写

        // 解析数字部分，如果含有多个数字部分，取长度最长的数字部分
        MatchCollection matches = REGEX_CUSTOMER_ID.Matches(customerIdOrCode);

        string maxLengthMatchValue = string.Empty;
        foreach (Match match in matches) {
            string matchValue = match.Value;
            if (matchValue.Length > maxLengthMatchValue.Length) {
                maxLengthMatchValue = matchValue;
            }
        }

        bool hasCustomerIdFromDigits = false;
        int customerIdFromDigits = 0;
        if (string.IsNullOrWhiteSpace(maxLengthMatchValue) == false) {
            hasCustomerIdFromDigits = int.TryParse(maxLengthMatchValue, out customerIdFromDigits);
        }

        // 处理最大数字部分剩下的部分
        string remain = customerIdOrCode;
        if (hasCustomerIdFromDigits == true) {
            remain = customerIdOrCode.Replace(maxLengthMatchValue, "");
        }

        if (string.IsNullOrWhiteSpace(remain)) {
            // throw new ValidationException("客户ID格式错误，暂不支持纯数字形式的客户ID！");
            return PARSE_FAIL_CUSTOMER_ID;
        }

        if (remain.StartsWith("SDQQ") == true) {
            remain = remain.Substring(4);
        }

        if (remain.StartsWith("SD") == true) {
            remain = remain.Substring(2);
        }

        if (string.IsNullOrWhiteSpace(remain) == true) {
            // throw new ValidationException("客户ID格式错误，信息缺失！");
            return PARSE_FAIL_CUSTOMER_ID;
        }

        remain = remain.ToLower(); // 数据库中是小写存储的

        if (customerCheckinCodeService.CheckCheckinCodeValid(remain) == false) {
            return PARSE_FAIL_CUSTOMER_ID;
        }

        var customerCheckinCode = session.Query<CustomerCheckinCodeInfo>()
            .Where(e => e.CheckinCode == remain)
            .Take(1)
            .SingleOrDefault();
        if (customerCheckinCode != null) {
            if (hasCustomerIdFromDigits == true && customerCheckinCode.Id != customerIdFromDigits) {
                // throw new ValidationException("客户ID格式错误，信息不匹配！");
                return PARSE_FAIL_CUSTOMER_ID;
            }

            return customerCheckinCode.Id;
        }

        return PARSE_FAIL_CUSTOMER_ID;
    }

    public int GetGuestIdFromGuestCode(string guestIdOrCode) {
        if (string.IsNullOrWhiteSpace(guestIdOrCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return PARSE_FAIL_CUSTOMER_ID;
        }

        MatchCollection matches = REGEX_CUSTOMER_ID.Matches(guestIdOrCode);

        IList<string> matchDigitList = new List<string>();
        foreach (Match match in matches) {
            matchDigitList.Add(match.Value);
        }

        if (matchDigitList.Count != 0) {
            string? maxLengthDigit = matchDigitList.OrderByDescending(e => e.Length).Take(1).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(maxLengthDigit) == false && int.TryParse(maxLengthDigit, out int guestId)) {
                return guestId;
            }
        }

        return PARSE_FAIL_CUSTOMER_ID;
    }

    public string GetCustomerCode(int customerId) {
        if (customerId <= 0) {
            return string.Empty;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        return $"SDQQ{customer.CheckinCode.ToUpper()}";
    }

    public string GetCustomerCodeWithNum(int customerId) {
        if (customerId <= 0) {
            return string.Empty;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        return $"SDQQ{customer.CheckinCode.ToUpper()}{customerId:#000}";
    }

    public string GetGuestCode(int guestId) {
        if (guestId >= 0) {
            return string.Empty;
        }

        return "G-" + guestId.ToString("#000");
    }
}
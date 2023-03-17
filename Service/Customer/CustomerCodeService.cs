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

    private static readonly Regex REGEX_CHECKIN_CODE = new Regex(@"[0-9]+", RegexOptions.Compiled);

    private static readonly int PARSE_FAIL_CUSTOMER_ID = 0;

    public int GetCustomerIdFromCustomerCode(string customerCode) {
        if (string.IsNullOrWhiteSpace(customerCode)) // 这个判断不能少，否则下面的 Regex.Matches 会报异常
        {
            return 0;
        }

        // 解析数字部分，如果含有多个数字部分，取长度最长的数字部分
        MatchCollection matches = REGEX_CHECKIN_CODE.Matches(customerCode);

        string maxLengthCheckiniCodeMatchValue = string.Empty;
        foreach (Match match in matches) {
            string matchValue = match.Value;
            if (matchValue.Length > maxLengthCheckiniCodeMatchValue.Length) {
                maxLengthCheckiniCodeMatchValue = matchValue;
            }
        }

        if (string.IsNullOrWhiteSpace(maxLengthCheckiniCodeMatchValue)) {
            return PARSE_FAIL_CUSTOMER_ID;
        }

        string checkinCode = maxLengthCheckiniCodeMatchValue;

        var customerCheckinCode = session.Query<CustomerCheckinCodeInfo>()
            .Where(e => e.CheckinCode == checkinCode)
            .Take(1)
            .SingleOrDefault();
        if (customerCheckinCode != null) {
            return customerCheckinCode.Id;
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

        return customer.CheckinCode;
    }

    public string GetCustomerCodeChinese(int customerId) {
        if (customerId <= 0) {
            return string.Empty;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        return customerCheckinCodeService.GetChineseCode(customer.CheckinCode);
    }
}
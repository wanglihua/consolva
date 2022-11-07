using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.Customer.Models.Customer;

public class CustomerDetailModel
{
    [BindNever]
    public CustomerInfo Customer { get; set; } = new CustomerInfo();

    [BindNever]
    public IList<LogisticsOrderInfo> CustomerOrderList { get; set; } = new List<LogisticsOrderInfo>();

    // 暂时注释

    // [BindNever]
    // public IList<LogisticsClaimGoodsInfo> LogisticsClaimGoodsList { get; set; }
    //
    // [BindNever]
    // public IList<LogisticsIssueGoodsInfo> LogisticsIssueGoodsList { get; set; }

    [BindNever]
    public IList<CustomerAccountEntryInfo> CustomerAccountEntryList { get; set; } = new List<CustomerAccountEntryInfo>();

    [BindNever]
    public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; } = new List<LogisticsOrderPayInfo>();

    [BindNever]
    public IList<CustomerScoreLogInfo> CustomerScoreLogList { get; set; } = new List<CustomerScoreLogInfo>();

    [BindNever]
    public IList<CustomerSendAddressInfo> CustomerSendAddressList { get; set; } = new List<CustomerSendAddressInfo>();

    [BindNever]
    public IList<CustomerReceiveAddressInfo> CustomerReceiveAddressList { get; set; } = new List<CustomerReceiveAddressInfo>();

    [BindNever]
    public bool CanFreezeCustomer { get; set; }

    [BindNever]
    public bool CanResetPassword { get; set; }

    [BindNever]
    public bool CanEditCharacter { get; set; }

    [BindNever]
    public IList<CustomerCharacterInfo> CustomerCharacterList { get; set; } = new List<CustomerCharacterInfo>();
}
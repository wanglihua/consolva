using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Service.Message.CustomerMessage;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerAccountService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CustomerMessageAccountChangeService customerMessageAccountChangeService;

    public CustomerAccountService(ISession session, CustomerMessageAccountChangeService customerMessageAccountChangeService)
    {
        this.session = session;
        this.customerMessageAccountChangeService = customerMessageAccountChangeService;
    }

    /// <summary>
    /// 账户余额变动
    /// </summary>
    /// <param name="changeType">变动类型</param>
    /// <param name="customerId">客户ID</param>
    /// <param name="amount">变动金额，可以是正的，也可以是负的</param>
    /// <param name="description">描述</param>
    public void Change(CustomerAccountChangeType changeType, int customerId, decimal amount, string description = "")
    {
        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的客户Id不存在！");
        }

        if (description == null)
        {
            throw new ValidationException("描述不能为NULL！");
        }

        decimal accountBefore = 0;
        decimal accountAfter = 0;

        accountBefore = customer.Account;
        customer.Account += amount; // 有可能是增加，也有可能是减少
        accountAfter = customer.Account;


        session.Update(customer);

        CustomerAccountEntryInfo accountEntry = new CustomerAccountEntryInfo();
        accountEntry.ChangeType = changeType;
        accountEntry.Amount = amount; // 有可能是增加，也有可能是减少
        accountEntry.AccountBefore = accountBefore;
        accountEntry.AccountAfter = accountAfter;
        accountEntry.Description = description;
        accountEntry.Customer = customer;

        session.Save(accountEntry);
        session.Flush();

        // 发送余额变动消息，区分海运和空运
        customerMessageAccountChangeService.SendMessage(customer, changeType.Name, accountEntry.Amount, accountEntry.AccountBefore, accountEntry.AccountAfter);
    }

    /// <summary>
    /// 代支付时的余额剩余最低金额
    /// 这个金额是不是应该跟注册或关注送的金额一致，定义到一起？
    /// </summary>
    private static readonly decimal PayAnotherMinRemainAccountAmount = 100m;

    /// <summary>
    /// 获得会员可用代支付余额
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public decimal GetPayForAnotherLimit(int customerId)
    {
        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的会员不存在！");
        }

        DateTime sometimeAgo = DateTime.Now.AddDays(-30);

        var accountEntryList = session.Query<CustomerAccountEntryInfo>()
            .Where(e => e.Customer.Id == customerId && e.CreateTime >= sometimeAgo)
            .ToList();

        decimal accountDelta = 0m;
        if (accountEntryList.Count != 0)
        {
            accountDelta = accountEntryList.Sum(e => e.Amount); // 这里的 Amount 可能是正的，也可能是负的 
        }

        decimal customerAccount = 0;

        customerAccount = customer.Account;

        decimal accountSometimeAgo = customerAccount - accountDelta; // accountDelta 可能是 正数，也可能是 负数

        // 计算 30 天内的最高余额
        decimal accountMax = accountSometimeAgo;
        decimal accountThatTime = accountSometimeAgo;
        foreach (var accountEntry in accountEntryList)
        {
            accountThatTime += accountEntry.Amount;

            if (accountThatTime > accountMax)
            {
                accountMax = accountThatTime;
            }
        }

        // 计算 30 天内的代支付额，取正数
        decimal payAnotherAmountBefore = 0m; // 取正数
        foreach (var accountEntry in accountEntryList)
        {
            if (accountEntry.ChangeType == CustomerAccountChangeType.PayAnother)
            {
                payAnotherAmountBefore += Math.Abs(accountEntry.Amount); // 取正数
            }
        }

        decimal payForAnotherAmountLimit = accountMax * 0.5m - payAnotherAmountBefore;
        // decimal payForAnotherAmountLimit = customerAccount * 0.5m; // 最多用余额的 50% , 这个方式有问题，付了一次后，下一次还有余额代支付

        if (payForAnotherAmountLimit > (customerAccount - PayAnotherMinRemainAccountAmount)) // 支付后 账户余额 不能少于 100
        {
            payForAnotherAmountLimit = customerAccount - PayAnotherMinRemainAccountAmount;
        }

        if (payForAnotherAmountLimit < 0)
        {
            payForAnotherAmountLimit = 0;
        }

        return payForAnotherAmountLimit;
    }
}
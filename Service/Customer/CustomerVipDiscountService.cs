using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerVipDiscountService {
    private readonly ISession session;

    public CustomerVipDiscountService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取会员VIP折扣列表
    /// 总的数量也不多，后面的操作都建立在这个获取的列表之上
    /// </summary>
    /// <returns></returns>
    public IList<CustomerVipDiscountInfo> GetCustomerVipDiscountList() {
        return session.Query<CustomerVipDiscountInfo>().OrderBy(e => e.Score).ToList();
    }

    public CustomerVipDiscountInfo? GetDiscount(int customerId, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        return null;
    }
}
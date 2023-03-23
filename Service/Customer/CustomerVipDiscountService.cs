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
    /// 检查客服所在VIP级别是否支持优惠券
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public bool CustomerCanCoupon(int customerId) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        CustomerVipRank vipRank = customer.VipRank;
        var vipDiscount = session.Query<CustomerVipDiscountInfo>()
            .Where(e => e.Rank == vipRank)
            .Take(1)
            .SingleOrDefault();

        if (vipDiscount != null && vipDiscount.CanCoupon == false) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取会员VIP折扣列表
    /// 总的数量也不多，后面的很多操作都建立在这个获取的列表之上
    /// todo 这里是不是可以加个缓存，可以时间长一点的缓存
    /// </summary>
    public IList<CustomerVipDiscountInfo> GetCustomerVipDiscountList() {
        return session.Query<CustomerVipDiscountInfo>().OrderBy(e => e.Score).ToList();
    }

    /// <summary>
    /// 获得会员VIP折扣
    /// </summary>
    /// <returns>不存在时，就返回 null</returns>
    public CustomerVipDiscountInfo? GetDiscount(int customerId) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        var vipRank = customer.VipRank;
        var discount = session.Query<CustomerVipDiscountInfo>()
            .Where(e => e.Rank == vipRank)
            .Take(1)
            .SingleOrDefault();

        return discount;
    }

    /// <summary>
    /// 获得会员VIP折扣
    /// </summary>
    /// <returns>不存在时，就返回 null</returns>
    public CustomerVipDiscountInfo? GetDiscount(int customerId, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        if (customerVipDiscountList.Count == 0) {
            return null;
        }

        var vipRank = customer.VipRank;
        var discount = customerVipDiscountList
            .Where(e => e.Rank == vipRank)
            .Take(1)
            .SingleOrDefault();

        return discount;
    }

    /// <summary>
    /// 获得会员VIP折扣
    /// </summary>
    /// <returns>不存在时，就返回 null</returns>
    public CustomerVipDiscountInfo? GetDiscount(decimal score, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        if (score <= 0) {
            return null;
        }

        if (customerVipDiscountList.Count == 0) {
            return null;
        }

        var discount = customerVipDiscountList
            .Where(e => e.Score <= score)
            .OrderByDescending(e => e.Score)
            .Take(1)
            .SingleOrDefault();

        return discount;
    }

    /// <summary>
    /// 获取下一级会员VIP折扣
    /// </summary>
    /// <returns>不存在时，就返回 null</returns>
    public CustomerVipDiscountInfo? GetNextLevelDiscount(int customerId, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        if (customerVipDiscountList.Count == 0) {
            return null;
        }

        return GetNextLevelDiscount(customer.TotalScore, customerVipDiscountList);
    }

    /// <summary>
    /// 获取积分对应的下一级VIP折扣
    /// </summary>
    /// <returns>不存在时，就返回 null</returns>
    public CustomerVipDiscountInfo? GetNextLevelDiscount(decimal score, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        if (customerVipDiscountList.Count == 0) {
            return null;
        }

        // 大于会员当前历史总积分的折扣设置中的最小的一个折扣设置
        var customerVipDiscount = customerVipDiscountList
            .Where(e => e.Score > score)
            .OrderBy(e => e.Score)
            .Take(1)
            .SingleOrDefault();

        return customerVipDiscount;
    }

    /// <summary>
    /// 新积分相对旧的积分，
    /// </summary>
    /// <param name="oldScore"></param>
    /// <param name="newScore"></param>
    /// <param name="customerVipDiscountList"></param>
    /// <returns></returns>
    public bool ShouldVipUpgrade(decimal oldScore, decimal newScore, IList<CustomerVipDiscountInfo> customerVipDiscountList) {
        if (customerVipDiscountList.Count == 0) {
            return false;
        }

        if (oldScore >= newScore) {
            return false;
        }

        var oldScoreCustomerVipDiscount = GetDiscount(oldScore, customerVipDiscountList);
        var newScoreCustomerVipDiscount = GetDiscount(newScore, customerVipDiscountList);

        // 原来没有VIP等级
        if (oldScoreCustomerVipDiscount == null && newScoreCustomerVipDiscount != null) {
            return true;
        }

        //说明已经是最高级别VIP了
        if (oldScoreCustomerVipDiscount != null && newScoreCustomerVipDiscount == null) {
            return true;
        }

        if ((oldScoreCustomerVipDiscount?.Score ?? 0m) < (newScoreCustomerVipDiscount?.Score ?? 0m)) {
            return true;
        } else {
            return false;
        }
    }

    public void UpdateAllCustomerVip() {
        session.CreateSQLQuery("CALL customer_update_vip_all()").List();
    }
}
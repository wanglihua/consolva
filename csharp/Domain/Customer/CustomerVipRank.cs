using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户VIP级别
/// </summary>
public class CustomerVipRank : CodeTypeBase
{
    /// <summary>
    /// VIP1, 1000
    /// </summary>
    public static readonly CustomerVipRank VIP1 = new CustomerVipRank(1000, "VIP1", "VIP1");

    /// <summary>
    /// VIP2, 2000
    /// </summary>
    public static readonly CustomerVipRank VIP2 = new CustomerVipRank(2000, "VIP2", "VIP2");

    /// <summary>
    /// VIP3, 3000
    /// </summary>
    public static readonly CustomerVipRank VIP3 = new CustomerVipRank(3000, "VIP3", "VIP3");

    /// <summary>
    /// VIP4, 4000
    /// </summary>
    public static readonly CustomerVipRank VIP4 = new CustomerVipRank(4000, "VIP4", "VIP4");

    /// <summary>
    /// VIP5, 5000
    /// </summary>
    public static readonly CustomerVipRank VIP5 = new CustomerVipRank(5000, "VIP5", "VIP5");

    /// <summary>
    /// VIP6, 6000
    /// </summary>
    public static readonly CustomerVipRank VIP6 = new CustomerVipRank(6000, "VIP6", "VIP6");

    /// <summary>
    /// VIP7, 7000
    /// </summary>
    public static readonly CustomerVipRank VIP7 = new CustomerVipRank(7000, "VIP7", "VIP7");

    /// <summary>
    /// VIP8, 8000
    /// </summary>
    public static readonly CustomerVipRank VIP8 = new CustomerVipRank(8000, "VIP8", "VIP8");

    /// <summary>
    /// VIP9, 9000
    /// </summary>
    public static readonly CustomerVipRank VIP9 = new CustomerVipRank(9000, "VIP9", "VIP9");

    /// <summary>
    /// VIP10, 10000
    /// </summary>
    public static readonly CustomerVipRank VIP10 = new CustomerVipRank(10000, "VIP10", "VIP10");

    /// <summary>
    /// VIP11, 11000
    /// </summary>
    public static readonly CustomerVipRank VIP11 = new CustomerVipRank(11000, "VIP11", "VIP11");

    /// <summary>
    /// VIP12, 12000
    /// </summary>
    public static readonly CustomerVipRank VIP12 = new CustomerVipRank(12000, "VIP12", "VIP12");

    /// <summary>
    /// VIP13, 13000
    /// </summary>
    public static readonly CustomerVipRank VIP13 = new CustomerVipRank(13000, "VIP13", "VIP13");

    /// <summary>
    /// VIP14, 14000
    /// </summary>
    public static readonly CustomerVipRank VIP14 = new CustomerVipRank(14000, "VIP14", "VIP14");

    /// <summary>
    /// VIP15, 15000
    /// </summary>
    public static readonly CustomerVipRank VIP15 = new CustomerVipRank(15000, "VIP15", "VIP15");

    /// <summary>
    /// VIP16, 16000
    /// </summary>
    public static readonly CustomerVipRank VIP16 = new CustomerVipRank(16000, "VIP16", "VIP16");

    /// <summary>
    /// VIP17, 17000
    /// </summary>
    public static readonly CustomerVipRank VIP17 = new CustomerVipRank(17000, "VIP17", "VIP17");

    /// <summary>
    /// VIP18, 18000
    /// </summary>
    public static readonly CustomerVipRank VIP18 = new CustomerVipRank(18000, "VIP18", "VIP18");

    /// <summary>
    /// VIP19, 19000
    /// </summary>
    public static readonly CustomerVipRank VIP19 = new CustomerVipRank(19000, "VIP19", "VIP19");

    /// <summary>
    /// VIP20, 20000
    /// </summary>
    public static readonly CustomerVipRank VIP20 = new CustomerVipRank(20000, "VIP20", "VIP20");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerVipRank None = new CustomerVipRank(0, "", "");

    private static readonly List<CustomerVipRank> codeTypeList = new List<CustomerVipRank>() {
        VIP1,
        VIP2,
        VIP3,
        VIP4,
        VIP5,
        VIP6,
        VIP7,
        VIP8,
        VIP9,
        VIP10,
        VIP11,
        VIP12,
        VIP13,
        VIP14,
        VIP15,
        VIP16,
        VIP17,
        VIP18,
        VIP19,
        VIP20,
    };

    private static readonly Dictionary<int, CustomerVipRank> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerVipRank GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerVipRank? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<CustomerVipRank> GetList()
    {
        return codeTypeList;
    }

    private CustomerVipRank(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerVipRankHibernateType : CodeHibernateTypeBase<CustomerVipRank>
{
    protected override CustomerVipRank GetCodeTypeById(int id)
    {
        return CustomerVipRank.GetById(id);
    }
}

public class CustomerVipRankModelBinder : CodeTypeModelBinderBase<CustomerVipRank>
{
    protected override CustomerVipRank GetCodeTypeById(int id)
    {
        return CustomerVipRank.GetById(id);
    }
}

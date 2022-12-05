using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户登录阶段
/// </summary>
public class CustomerLoginStage : CodeTypeBase
{
    /// <summary>
    /// 注册后登录
    /// 就是注册后直接自动登录
    /// </summary>
    public static readonly CustomerLoginStage LoginAfterRegister = new CustomerLoginStage(1000, "注册后登录", "注册后登录");

    /// <summary>
    /// 手工登录
    /// 就是手工录入用户名密码登录
    /// </summary>
    public static readonly CustomerLoginStage WebManualLogin = new CustomerLoginStage(2000, "手工登录", "手工登录");

    /// <summary>
    /// 微信公众号登录
    /// </summary>
    public static readonly CustomerLoginStage WechatOfficialAccountLogin = new CustomerLoginStage(3000, "微信公众号登录", "微信公众号登录");

    /// <summary>
    /// 微信小程序登录
    /// </summary>
    public static readonly CustomerLoginStage WechatAppLogin = new CustomerLoginStage(4000, "微信小程序登录", "微信小程序登录");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerLoginStage None = new CustomerLoginStage(0, "", "");

    private static readonly IReadOnlyList<CustomerLoginStage> codeTypeList = new List<CustomerLoginStage>() {
        LoginAfterRegister,
        WebManualLogin,
        WechatOfficialAccountLogin,
        WechatAppLogin,
    };

    private static readonly Dictionary<int, CustomerLoginStage> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerLoginStage GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerLoginStage? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerLoginStage> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerLoginStage(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerLoginStageHibernateType : CodeHibernateTypeBase<CustomerLoginStage>
{
    protected override CustomerLoginStage GetCodeTypeById(int id)
    {
        return CustomerLoginStage.GetById(id);
    }
}

public class CustomerLoginStageModelBinder : CodeTypeModelBinderBase<CustomerLoginStage>
{
    protected override CustomerLoginStage GetCodeTypeById(int id)
    {
        return CustomerLoginStage.GetById(id);
    }
}
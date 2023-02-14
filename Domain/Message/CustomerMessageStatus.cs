using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息发送状态
/// </summary>
public class CustomerMessageStatus : CodeTypeBase {
    /// <summary>
    /// 未发送，1000, 这个状态已经不常用
    /// </summary>
    public static readonly CustomerMessageStatus NotSend = new CustomerMessageStatus(1000, "未发送", "未发送");

    /// <summary>
    /// 发送失败, 2000
    /// </summary>
    public static readonly CustomerMessageStatus Fail = new CustomerMessageStatus(2000, "发送失败", "发送失败");

    /// <summary>
    /// 发送成功, 3000
    /// </summary>
    public static readonly CustomerMessageStatus Success = new CustomerMessageStatus(3000, "发送成功", "发送成功");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageStatus None = new CustomerMessageStatus(0, "", "");

    private static readonly IReadOnlyList<CustomerMessageStatus> codeTypeList = new List<CustomerMessageStatus>() {
        NotSend,
        Fail,
        Success,
    };

    private static readonly IDictionary<int, CustomerMessageStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerMessageStatus> GetList() {
        return codeTypeList.ToList();
    }

    private CustomerMessageStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class CustomerMessageStatusHibernateType : CodeHibernateTypeBase<CustomerMessageStatus> {
    protected override CustomerMessageStatus GetCodeTypeById(int id) {
        return CustomerMessageStatus.GetById(id);
    }
}

public class CustomerMessageStatusModelBinder : CodeTypeModelBinderBase<CustomerMessageStatus> {
    protected override CustomerMessageStatus GetCodeTypeById(int id) {
        return CustomerMessageStatus.GetById(id);
    }
}
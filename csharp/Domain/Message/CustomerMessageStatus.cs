using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息发送状态
/// </summary>
public class CustomerMessageStatus : CodeTypeBase
{
    /// <summary>
    /// 未发送
    /// </summary>
    public static readonly CustomerMessageStatus NotSend = new CustomerMessageStatus(100, "未发送", "未发送");

    /// <summary>
    /// 发送失败
    /// </summary>
    public static readonly CustomerMessageStatus SendFail = new CustomerMessageStatus(150, "发送失败", "发送失败");  // todo a 改成 Fail

    /// <summary>
    /// 已发送
    /// </summary>
    public static readonly CustomerMessageStatus Sent = new CustomerMessageStatus(200, "已发送", "已发送"); // todo a 改成 Success

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageStatus None = new CustomerMessageStatus(0, "", "");

    public static CustomerMessageStatus GetById(int id)
    {
        IList<CustomerMessageStatus> statusList = GetList();
        foreach (CustomerMessageStatus messageType in statusList)
        {
            if (messageType.Id == id)
            {
                return messageType;
            }
        }

        return None;
    }

    public static IList<CustomerMessageStatus> GetList()
    {
        IList<CustomerMessageStatus> statusList = new List<CustomerMessageStatus>() {
            NotSend,
            SendFail,
            Sent
        };

        return statusList;
    }

    private CustomerMessageStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerMessageStatusHibernateType : CodeHibernateTypeBase<CustomerMessageStatus>
{
    protected override CustomerMessageStatus GetCodeTypeById(int id)
    {
        return CustomerMessageStatus.GetById(id);
    }
}

public class CustomerMessageStatusModelBinder : CodeTypeModelBinderBase<CustomerMessageStatus>
{
    protected override CustomerMessageStatus GetCodeTypeById(int id)
    {
        return CustomerMessageStatus.GetById(id);
    }
}
using shunda.Core.CodeType;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端协议状态
/// </summary>
public class ClientAgreementStatus : CodeTypeBase {
    /// <summary>
    /// 未启用
    /// </summary>
    public static readonly ClientAgreementStatus Disabled = new ClientAgreementStatus(1000, "未启用", "未启用");

    /// <summary>
    /// 已启用
    /// </summary>
    public static readonly ClientAgreementStatus Enabled = new ClientAgreementStatus(2000, "已启用", "已启用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ClientAgreementStatus None = new ClientAgreementStatus(0, "", "");

    private static readonly IReadOnlyList<ClientAgreementStatus> codeTypeList = new List<ClientAgreementStatus>() {
        Disabled,
        Enabled
    };

    private static readonly IDictionary<int, ClientAgreementStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ClientAgreementStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ClientAgreementStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<ClientAgreementStatus> GetList() {
        return codeTypeList.ToList();
    }

    private ClientAgreementStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class ClientAgreementStatusHibernateType : CodeHibernateTypeBase<ClientAgreementStatus> {
    protected override ClientAgreementStatus GetCodeTypeById(int id) {
        return ClientAgreementStatus.GetById(id);
    }
}

public class ClientAgreementStatusModelBinder : CodeTypeModelBinderBase<ClientAgreementStatus> {
    protected override ClientAgreementStatus GetCodeTypeById(int id) {
        return ClientAgreementStatus.GetById(id);
    }
}
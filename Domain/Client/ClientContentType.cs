using shunda.Core.CodeType;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端内容类型
/// 主要用于客户端内容的查看记录
/// 比 ClientInstructionType 多了些条目
/// </summary>
public class ClientContentType : CodeTypeBase {
    /// <summary>
    /// 使用协议
    /// </summary>
    public static readonly ClientContentType UseAgreement = new ClientContentType(1000, "使用协议", "使用协议");

    /// <summary>
    /// 保险条款
    /// </summary>
    public static readonly ClientContentType InsuranceClause = new ClientContentType(2000, "保险条款", "保险条款");

    /// <summary>
    /// 运费说明
    /// </summary>
    public static readonly ClientContentType FreightDesc = new ClientContentType(3000, "运费说明", "运费说明");

    /// <summary>
    /// 下单流程
    /// </summary>
    public static readonly ClientContentType OrderProcess = new ClientContentType(4000, "下单流程", "下单流程");

    /// <summary>
    /// 优惠活动
    /// </summary>
    public static readonly ClientContentType Promotion = new ClientContentType(5000, "优惠活动", "优惠活动");

    /// <summary>
    /// 仓储说明
    /// </summary>
    public static readonly ClientContentType StorageInstruction = new ClientContentType(6000, "仓储说明", "仓储说明");

    /// <summary>
    /// 物品分类
    /// </summary>
    public static readonly ClientContentType GoodsType = new ClientContentType(7000, "物品分类", "物品分类");

    /// <summary>
    /// 客户推广
    /// </summary>
    public static readonly ClientContentType CustomerPromotion = new ClientContentType(8000, "客户推广", "客户推广");

    /// <summary>
    /// 仓库地址
    /// </summary>
    public static readonly ClientContentType WarehouseAddress = new ClientContentType(9000, "仓库地址", "仓库地址");

    /// <summary>
    /// 公告通知
    /// </summary>
    public static readonly ClientContentType Notice = new ClientContentType(10000, "公告通知", "公告通知");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ClientContentType None = new ClientContentType(0, "", "");

    private static readonly IReadOnlyList<ClientContentType> codeTypeList = new List<ClientContentType>() {
        UseAgreement,
        InsuranceClause,
        FreightDesc,
        OrderProcess,
        Promotion,
        StorageInstruction,
        GoodsType,
        CustomerPromotion,
        WarehouseAddress,
        Notice,
    };

    private static readonly IDictionary<int, ClientContentType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ClientContentType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ClientContentType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<ClientContentType> GetList() {
        return codeTypeList.ToList();
    }

    private ClientContentType(int id, string name, string description) : base(id, name, description) {
    }
}

public class ClientContentTypeHibernateType : CodeHibernateTypeBase<ClientContentType> {
    protected override ClientContentType GetCodeTypeById(int id) {
        return ClientContentType.GetById(id);
    }
}

public class ClientContentTypeModelBinder : CodeTypeModelBinderBase<ClientContentType> {
    protected override ClientContentType GetCodeTypeById(int id) {
        return ClientContentType.GetById(id);
    }
}
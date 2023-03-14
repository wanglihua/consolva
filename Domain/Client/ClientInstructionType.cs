using shunda.Core.CodeType;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端说明类型
/// </summary>
public class ClientInstructionType : CodeTypeBase {
    /// <summary>
    /// 使用协议
    /// todo  应该是不再使用了，后面去掉
    /// </summary>
    public static readonly ClientInstructionType UseAgreement = new ClientInstructionType(1000, "使用协议", "使用协议");

    /// <summary>
    /// 保险条款
    /// </summary>
    public static readonly ClientInstructionType InsuranceClause = new ClientInstructionType(2000, "保险条款", "保险条款");

    /// <summary>
    /// 运费说明
    /// </summary>
    public static readonly ClientInstructionType FreightDesc = new ClientInstructionType(3000, "运费说明", "运费说明");

    /// <summary>
    /// 下单流程
    /// todo  应该是不再使用了，后面去掉
    /// </summary>
    public static readonly ClientInstructionType OrderProcess = new ClientInstructionType(4000, "下单流程", "下单流程");

    /// <summary>
    /// 优惠活动
    /// </summary>
    public static readonly ClientInstructionType Promotion = new ClientInstructionType(5000, "优惠活动", "优惠活动");

    /// <summary>
    /// 仓储说明
    /// todo  应该是不再使用了，后面去掉
    /// </summary>
    public static readonly ClientInstructionType StorageInstruction = new ClientInstructionType(6000, "仓储说明", "仓储说明");

    /// <summary>
    /// 物品分类
    /// </summary>
    public static readonly ClientInstructionType GoodsType = new ClientInstructionType(7000, "物品分类", "物品分类");

    /// <summary>
    /// 关注公众号
    /// </summary>
    public static readonly ClientInstructionType FollowTip = new ClientInstructionType(8000, "关注公众号", "关注公众号");

    /// <summary>
    /// 客服二维码
    /// </summary>
    public static readonly ClientInstructionType CustomerService = new ClientInstructionType(9000, "客服二维码", "客服二维码");

    /// <summary>
    /// 简单五步轻松转运
    /// </summary>
    public static readonly ClientInstructionType OrderSimpleStep = new ClientInstructionType(10000, "简单五步轻松转运", "简单五步轻松转运");

    /// <summary>
    /// 新手指南
    /// </summary>
    public static readonly ClientInstructionType BeginnerGuide = new ClientInstructionType(11000, "新手指南", "新手指南");

    /// <summary>
    /// 常见问题
    /// </summary>
    public static readonly ClientInstructionType Faq = new ClientInstructionType(12000, "常见问题", "常见问题");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ClientInstructionType None = new ClientInstructionType(0, "", "");

    private static readonly IReadOnlyList<ClientInstructionType> codeTypeList = new List<ClientInstructionType>() {
        UseAgreement,
        InsuranceClause,
        FreightDesc,
        OrderProcess,
        Promotion,
        StorageInstruction,
        GoodsType,
        FollowTip,
        CustomerService,
        OrderSimpleStep,
        BeginnerGuide,
        Faq,
    };

    private static readonly IDictionary<int, ClientInstructionType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ClientInstructionType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ClientInstructionType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<ClientInstructionType> GetList() {
        return codeTypeList.ToList();
    }

    private ClientInstructionType(int id, string name, string description) : base(id, name, description) {
    }
}

public class ClientInstructionTypeHibernateType : CodeHibernateTypeBase<ClientInstructionType> {
    protected override ClientInstructionType GetCodeTypeById(int id) {
        return ClientInstructionType.GetById(id);
    }
}

public class ClientInstructionTypeModelBinder : CodeTypeModelBinderBase<ClientInstructionType> {
    protected override ClientInstructionType GetCodeTypeById(int id) {
        return ClientInstructionType.GetById(id);
    }
}
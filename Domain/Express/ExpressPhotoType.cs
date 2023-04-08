using shunda.Core.CodeType;

namespace shunda.Domain.Express;

public class ExpressPhotoType : CodeTypeBase {
    /// <summary>
    /// 签收照片
    /// </summary>
    public static readonly ExpressPhotoType Signin = new ExpressPhotoType(1000, "签收照片", "签收照片");

    /// <summary>
    /// 入库照片
    /// </summary>
    public static readonly ExpressPhotoType Checkin = new ExpressPhotoType(2000, "入库照片", "入库照片");

    /// <summary>
    /// 问题件照片
    /// </summary>
    public static readonly ExpressPhotoType Issue = new ExpressPhotoType(4000, "问题件照片", "问题件照片");

    /// <summary>
    /// 发货照片
    /// </summary>
    public static readonly ExpressPhotoType Ship = new ExpressPhotoType(5000, "发货照片", "发货照片");

    /// <summary>
    /// 中国损毁照片
    /// </summary>
    public static readonly ExpressPhotoType CnDamage = new ExpressPhotoType(6000, "中国损毁照片", "中国损毁照片");

    /// <summary>
    /// 退运照片
    /// </summary>
    public static readonly ExpressPhotoType Return = new ExpressPhotoType(7000, "退运照片", "退运照片");

    /// <summary>
    /// 客服核对照片
    /// </summary>
    public static readonly ExpressPhotoType CustomerServiceCheck = new ExpressPhotoType(9000, "客服核对照片", "客服核对照片");

    /// <summary>
    /// 客服售后照片
    /// </summary>
    public static readonly ExpressPhotoType CustomerServicePostSale = new ExpressPhotoType(10000, "客服售后照片", "客服售后照片");

    /// <summary>
    /// 客诉记录照片
    /// </summary>
    public static readonly ExpressPhotoType CustomerServiceAppeal = new ExpressPhotoType(11000, "客诉记录照片", "客诉记录照片");

    /// <summary>
    /// 交付照片
    /// </summary>
    public static readonly ExpressPhotoType Deliver = new ExpressPhotoType(88000, "交付照片", "交付照片");

    /// <summary>
    /// 派送失败照片
    /// </summary>
    public static readonly ExpressPhotoType DeliverFail = new ExpressPhotoType(89000, "派送失败照片", "派送失败照片");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressPhotoType None = new ExpressPhotoType(0, "", "");

    private static readonly IReadOnlyList<ExpressPhotoType> codeTypeList = new List<ExpressPhotoType>() {
        Signin,
        Checkin,
        Issue,
        Ship,
        CnDamage,
        Return,
        CustomerServiceCheck,
        CustomerServicePostSale,
        CustomerServiceAppeal,
        Deliver,
        DeliverFail,
    };

    private static readonly Dictionary<int, ExpressPhotoType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressPhotoType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ExpressPhotoType? codeType)) {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 返回所有图片类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<ExpressPhotoType> GetList() {
        return codeTypeList.ToList();
    }

    private ExpressPhotoType(int id, string name, string description) : base(id, name, description) {
    }
}

public class ExpressPhotoTypeHibernateType : CodeHibernateTypeBase<ExpressPhotoType> {
    protected override ExpressPhotoType GetCodeTypeById(int id) {
        return ExpressPhotoType.GetById(id);
    }
}

public class ExpressPhotoTypeModelBinder : CodeTypeModelBinderBase<ExpressPhotoType> {
    protected override ExpressPhotoType GetCodeTypeById(int id) {
        return ExpressPhotoType.GetById(id);
    }
}
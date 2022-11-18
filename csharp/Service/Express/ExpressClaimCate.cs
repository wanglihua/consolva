using shunda.Core.CodeType;

namespace shunda.Service.Express;

// todo a 改成 ExpressMenuCate
// 这里面的数字，不会保存到数据库，可以随意修改
public class ExpressClaimCate : CodeTypeBase
{
    public static readonly ExpressClaimCate All = new ExpressClaimCate(1000, "快递详情", "");

    public static readonly ExpressClaimCate ToOnShelf = new ExpressClaimCate(2000, "待上架", "");

    public static readonly ExpressClaimCate ToOnShelfSchedule = new ExpressClaimCate(3000, "上架待排班", "");

    public static readonly ExpressClaimCate OnShelf = new ExpressClaimCate(4000, "已上架", "");
    public static readonly ExpressClaimCate Picked = new ExpressClaimCate(5000, "已认领", "");
    public static readonly ExpressClaimCate NotPicked = new ExpressClaimCate(6000, "未认领", "");
    public static readonly ExpressClaimCate Expect = new ExpressClaimCate(7000, "已预报", "");
    public static readonly ExpressClaimCate NotPickedLong = new ExpressClaimCate(8000, "长期未认领", "");
    public static readonly ExpressClaimCate NotOrderLong = new ExpressClaimCate(9000, "长期未出库", "");

    public static readonly ExpressClaimCate NotPickedLongDumpSchedule = new ExpressClaimCate(10000, "长期未认领堆存排班", "");
    public static readonly ExpressClaimCate NotOrderLongDumpSchedule = new ExpressClaimCate(11000, "长期未出库堆存排班", "");

    public static readonly ExpressClaimCate Dumped = new ExpressClaimCate(12000, "已堆存", "");

    public static readonly ExpressClaimCate Archive = new ExpressClaimCate(13000, "历史归档", "");

    public static readonly ExpressClaimCate ToOffShelf = new ExpressClaimCate(14000, "待下架", "");

    public static readonly ExpressClaimCate ToOffShelfSchedule = new ExpressClaimCate(15000, "下架待排班", "");

    public static readonly ExpressClaimCate OffShelf = new ExpressClaimCate(16000, "已下架未入打包池", "");
    public static readonly ExpressClaimCate Packing = new ExpressClaimCate(17000, "待打包流动仓位", "");

    public static readonly ExpressClaimCate Return = new ExpressClaimCate(18000, "退运池", "");

    public static readonly ExpressClaimCate WoodFrame = new ExpressClaimCate(19000, "打木架", "");

    public static readonly ExpressClaimCate BarkClean = new ExpressClaimCate(20000, "清理树皮杂物", "");

    public static readonly ExpressClaimCate None = new ExpressClaimCate(0, "", "");

    private static readonly IReadOnlyList<ExpressClaimCate> codeTypeList = new List<ExpressClaimCate>() {
        All,

        ToOnShelf,
        ToOnShelfSchedule,

        OnShelf,
        Picked,
        NotPicked,
        Expect,
        NotPickedLong,
        NotOrderLong,

        Dumped,

        NotPickedLongDumpSchedule,
        NotOrderLongDumpSchedule,

        Archive,

        ToOffShelf,
        ToOffShelfSchedule,

        OffShelf,
        Packing,
        Return,

        WoodFrame,
        BarkClean,
    };

    private static readonly Dictionary<int, ExpressClaimCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    private ExpressClaimCate(int id, string name, string description) : base(id, name, description)
    {
    }

    public static ExpressClaimCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressClaimCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressClaimCate> GetList()
    {
        return codeTypeList.ToList();
    }
}
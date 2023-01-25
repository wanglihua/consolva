using shunda.Core.CodeType;

namespace shunda.Service.Express;

// 这里面的数字，不会保存到数据库，可以随意修改
public class ExpressListMenuCate : CodeTypeBase
{
    public static readonly ExpressListMenuCate All = new ExpressListMenuCate(1000, "快递详情", "");

    public static readonly ExpressListMenuCate ToOnShelf = new ExpressListMenuCate(2000, "待上架", "");

    public static readonly ExpressListMenuCate ToOnShelfSchedule = new ExpressListMenuCate(3000, "上架待排班", "");

    public static readonly ExpressListMenuCate OnShelf = new ExpressListMenuCate(4000, "已上架", "");
    public static readonly ExpressListMenuCate Picked = new ExpressListMenuCate(5000, "已认领", "");
    public static readonly ExpressListMenuCate NotPicked = new ExpressListMenuCate(6000, "未认领", "");
    public static readonly ExpressListMenuCate Expect = new ExpressListMenuCate(7000, "已预报", "");
    public static readonly ExpressListMenuCate NotPickedLong = new ExpressListMenuCate(8000, "长期未认领", "");
    public static readonly ExpressListMenuCate NotOrderLong = new ExpressListMenuCate(9000, "长期未出库", "");

    public static readonly ExpressListMenuCate NotPickedLongDumpSchedule = new ExpressListMenuCate(10000, "长期未认领堆存排班", "");
    public static readonly ExpressListMenuCate NotOrderLongDumpSchedule = new ExpressListMenuCate(11000, "长期未出库堆存排班", "");

    public static readonly ExpressListMenuCate Dumped = new ExpressListMenuCate(12000, "已堆存", "");

    public static readonly ExpressListMenuCate Archive = new ExpressListMenuCate(13000, "历史归档", "");

    public static readonly ExpressListMenuCate ToOffShelf = new ExpressListMenuCate(14000, "待下架", "");

    public static readonly ExpressListMenuCate ToOffShelfSchedule = new ExpressListMenuCate(15000, "下架待排班", "");

    public static readonly ExpressListMenuCate OffShelf = new ExpressListMenuCate(16000, "已下架未入打包池", "");
    public static readonly ExpressListMenuCate Packing = new ExpressListMenuCate(17000, "待打包流动仓位", "");

    public static readonly ExpressListMenuCate Return = new ExpressListMenuCate(18000, "退运池", "");

    public static readonly ExpressListMenuCate None = new ExpressListMenuCate(0, "", "");

    private static readonly IReadOnlyList<ExpressListMenuCate> codeTypeList = new List<ExpressListMenuCate>() {
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
    };

    private static readonly Dictionary<int, ExpressListMenuCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    private ExpressListMenuCate(int id, string name, string description) : base(id, name, description)
    {
    }

    public static ExpressListMenuCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressListMenuCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressListMenuCate> GetList()
    {
        return codeTypeList.ToList();
    }
}
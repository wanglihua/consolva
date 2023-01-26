using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackDisplayType : CodeTypeBase {
    /// <summary>
    /// 显示分配的和未排班的
    /// </summary>
    public static readonly LogisticsOrderPackDisplayType NotOthers = new LogisticsOrderPackDisplayType(1000, "显示分配的和未排班的", "显示分配的和未排班的");

    /// <summary>
    /// 只显示分配的
    /// </summary>
    public static readonly LogisticsOrderPackDisplayType OnlyMine = new LogisticsOrderPackDisplayType(2000, "只显示分配的", "只显示分配的");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderPackDisplayType None = new LogisticsOrderPackDisplayType(0, "", "");

    public static LogisticsOrderPackDisplayType GetById(int id) {
        IList<LogisticsOrderPackDisplayType> logisticsOrderPackSortTypeList = GetList();
        foreach (LogisticsOrderPackDisplayType logisticsOrderPackSortType in logisticsOrderPackSortTypeList) {
            if (logisticsOrderPackSortType.Id == id) {
                return logisticsOrderPackSortType;
            }
        }

        return None;
    }

    public static IList<LogisticsOrderPackDisplayType> GetList() {
        IList<LogisticsOrderPackDisplayType> logisticsOrderPackSortTypeList = new List<LogisticsOrderPackDisplayType>();
        logisticsOrderPackSortTypeList.Add(NotOthers);
        logisticsOrderPackSortTypeList.Add(OnlyMine);

        return logisticsOrderPackSortTypeList;
    }

    private LogisticsOrderPackDisplayType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderPackDisplayTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderPackDisplayType> {
    protected override LogisticsOrderPackDisplayType GetCodeTypeById(int id) {
        return LogisticsOrderPackDisplayType.GetById(id);
    }
}

public class LogisticsOrderPackDisplayTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderPackDisplayType> {
    protected override LogisticsOrderPackDisplayType GetCodeTypeById(int id) {
        return LogisticsOrderPackDisplayType.GetById(id);
    }
}
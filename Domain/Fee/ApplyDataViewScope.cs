using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 订单类型
/// </summary>
public class FeeApplyDataViewScope : CodeTypeBase
{
    /// <summary>
    /// 只看自己
    /// </summary>
    public static readonly FeeApplyDataViewScope MySelf = new FeeApplyDataViewScope(100, "只看自己", "只看自己");

    /// <summary>
    /// 查看全部
    /// </summary>
    public static readonly FeeApplyDataViewScope All = new FeeApplyDataViewScope(200, "查看全部", "查看全部");

    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeApplyDataViewScope None = new FeeApplyDataViewScope(0, "", "");


    public static FeeApplyDataViewScope GetById(int id)
    {
        IList<FeeApplyDataViewScope> scopeList = GetList();
        foreach (FeeApplyDataViewScope scope in scopeList)
        {
            if (scope.Id == id)
            {
                return scope;
            }
        }

        return None;
    }

    public static IList<FeeApplyDataViewScope> GetList()
    {
        return new List<FeeApplyDataViewScope>() {
            MySelf,
            All,
        };
    }

    private FeeApplyDataViewScope(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeApplyDataViewScopeHibernateType : CodeHibernateTypeBase<FeeApplyDataViewScope>
{
    protected override FeeApplyDataViewScope GetCodeTypeById(int id)
    {
        return FeeApplyDataViewScope.GetById(id);
    }
}

public class FeeApplyDataViewScopeModelBinder : CodeTypeModelBinderBase<FeeApplyDataViewScope>
{
    protected override FeeApplyDataViewScope GetCodeTypeById(int id)
    {
        return FeeApplyDataViewScope.GetById(id);
    }
}
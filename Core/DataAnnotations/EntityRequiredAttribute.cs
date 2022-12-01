using System.ComponentModel.DataAnnotations;

namespace shunda.Core.DataAnnotations;

/// <summary>
/// 用于标记一个成员实体不能为空，并且 成员实体的 id 也不能为 0
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class EntityRequiredAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (!(value is Entity))
        {
            return false;
        }

        Entity? model = value as Entity;
        if (model == null)
        {
            return false;
        }

        if (model.Id == 0)
        {
            return false;
        }

        return true;
    }
}
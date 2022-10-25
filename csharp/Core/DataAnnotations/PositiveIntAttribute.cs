using System.ComponentModel.DataAnnotations;

namespace shunda.Core.DataAnnotations;

/// <summary>
/// 用于标记一个 int 成员不能 小于 0
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PositiveIntAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (!(value is int))
        {
            return false;
        }

        int intValue = (int)value;

        if (intValue < 0)
        {
            return false;
        }

        return true;
    }
}
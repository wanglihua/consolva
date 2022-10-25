using System.ComponentModel.DataAnnotations;

namespace shunda.Core.DataAnnotations;

/// <summary>
/// 用于标记一个 decimal 成员不能 小于 0
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PositiveDecimalAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (!(value is decimal))
        {
            return false;
        }

        decimal decimalValue = (decimal)value;

        if (decimalValue < 0)
        {
            return false;
        }

        return true;
    }
}
using System.ComponentModel.DataAnnotations;
using shunda.Core.CodeType;

namespace shunda.Core.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CodeTypeRequiredAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (!(value is CodeTypeBase))
        {
            return false;
        }

        CodeTypeBase? codeType = value as CodeTypeBase;
        if (codeType == null)
        {
            return false;
        }

        if (codeType.Id == 0)
        {
            return false;
        }

        return true;
    }
}
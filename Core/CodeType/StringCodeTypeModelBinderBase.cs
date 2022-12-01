using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.CodeType;

public abstract class StringCodeTypeModelBinderBase<T> : IModelBinder where T : StringCodeTypeBase
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var code = valueProviderResult.FirstValue;

        T codeType = GetCodeTypeByCode(code ?? string.Empty);
        bindingContext.Result = ModelBindingResult.Success(codeType);
        return Task.CompletedTask;
    }

    protected abstract T GetCodeTypeByCode(string code);
}
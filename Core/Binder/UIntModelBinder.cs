using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Binder;

#nullable disable

public class UIntModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        // Try to fetch the value of the argument by name
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        string intStr = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(intStr))
        {
            return Task.CompletedTask;
        }

        if (!int.TryParse(intStr, out int intResult))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, string.Format("{0} 不是正确的整数类型！", intStr));
            return Task.CompletedTask;
        }

        if (intResult < 0)
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, string.Format("{0} 不是一个大于或等于零的整数！", intResult));
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(intResult);
        return Task.CompletedTask;
    }
}
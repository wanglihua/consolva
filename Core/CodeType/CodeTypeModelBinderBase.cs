using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.CodeType;

public abstract class CodeTypeModelBinderBase<T> : IModelBinder where T : CodeTypeBase
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

        var intStr = valueProviderResult.FirstValue;
        T codeType = GetCodeTypeById(0); // 如果字符串为空就取默认的 None 值
        if (string.IsNullOrWhiteSpace(intStr) == false)
        {
            if (int.TryParse(intStr, out int code) == false)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "代码格式不对，应该为整数类型！");
                return Task.CompletedTask;
            }

            codeType = GetCodeTypeById(code);
        }

        bindingContext.Result = ModelBindingResult.Success(codeType);
        return Task.CompletedTask;
    }

    protected abstract T GetCodeTypeById(int id);
}
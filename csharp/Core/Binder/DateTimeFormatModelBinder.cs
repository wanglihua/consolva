using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Binder;

public class DateTimeFormatModelBinder : IModelBinder
{
    private string dateTimeFormat;

    public DateTimeFormatModelBinder(string dateTimeFormat)
    {
        this.dateTimeFormat = dateTimeFormat;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        if (string.IsNullOrEmpty(this.dateTimeFormat))
        {
            throw new ArgumentException("指定的日期时间的格式不能为空！");
        }

        // Try to fetch the value of the argument by name
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var dateTimeStr = valueProviderResult.FirstValue;
        if (!DateTime.TryParseExact(dateTimeStr, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, string.Format("日期时间的格式不对，应该为 {0} 的类似格式！", DateTime.Now.ToString(dateTimeFormat)));
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(dateTime);
        return Task.CompletedTask;
    }
}
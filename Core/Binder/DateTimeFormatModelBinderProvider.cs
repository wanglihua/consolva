using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Binder;

#nullable disable

public class DateTimeFormatModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Type modelType = context.Metadata.ModelType;
        if (modelType == typeof(DateTime) || modelType == typeof(DateTime?))
        {
            string formatString = context.Metadata.EditFormatString; // 先看 EditFormatString
            if (string.IsNullOrEmpty(formatString))
            {
                formatString = context.Metadata.DisplayFormatString; // 再看 DisplayFormatString
                if (string.IsNullOrEmpty(formatString))
                {
                    return null;
                }
            }

            return new DateTimeFormatModelBinder(formatString);
        }

        return null;
    }
}


/*

https://stackoverflow.com/questions/41642800/change-default-format-for-datetime-parsing-in-asp-net-core

services.AddMvc(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeFormatModelBinderProvider());
});

*/
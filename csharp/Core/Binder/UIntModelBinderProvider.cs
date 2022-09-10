using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Binder;

#nullable disable

public class UIntModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Type modelType = context.Metadata.ModelType;
        if (modelType == typeof(uint) || modelType == typeof(uint?))
        {
            return new UIntModelBinder();
        }

        return null;
    }
} 
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Core.Binder;

#nullable disable

public class IntModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Type modelType = context.Metadata.ModelType;
        if (modelType == typeof(int) || modelType == typeof(int?))
        {
            return new IntModelBinder();
        }

        return null;
    }
}
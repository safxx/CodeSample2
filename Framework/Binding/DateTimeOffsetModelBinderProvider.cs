using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HighLoad.Framework.Binding
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.UnderlyingOrModelType == typeof(DateTime?) || context.Metadata.UnderlyingOrModelType == typeof(DateTime))
            {
                return new DateTimeModelBinder();
            }

            return null;
        }
    }
}
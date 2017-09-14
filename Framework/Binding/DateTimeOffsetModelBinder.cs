using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HighLoad.Framework.Binding
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;
                object model;
                if (string.IsNullOrEmpty(value))
                {
                    model = null;
                }
                else
                {
                    var success = false;
                    model = null;
                    if (value.Length < 12 && IsDigitsOnly(value))
                    {
                        if (long.TryParse(value, out long result))
                        {
                            model = DateTimeOffset.FromUnixTimeSeconds(result).LocalDateTime;
                            success = true;
                        }
                    }
                    if (!success)
                    {
                        bindingContext.ModelState.TryAddModelError(
                            bindingContext.ModelName,
                            new Exception("format exception"),
                            bindingContext.ModelMetadata);
                        return Task.CompletedTask;
                    }
                }
                //if model is null and type is not nullable
                //return a required field error
                if (model == null &&
                    !bindingContext.ModelMetadata
                        .IsReferenceOrNullableType)
                {
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        bindingContext.ModelMetadata
                            .ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                                valueProviderResult.ToString()));

                    return Task.CompletedTask;
                }
                else
                {
                    bindingContext.Result =
                        ModelBindingResult.Success(model);
                    return Task.CompletedTask;
                }
            }
            catch (Exception exception)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    exception,
                    bindingContext.ModelMetadata);


                return Task.CompletedTask;
            }
        }

        bool IsDigitsOnly(string str)
        {
            for (int i = 1; i < str.Length; i++)
            {
                var c = str[i];
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}

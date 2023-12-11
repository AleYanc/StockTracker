using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace StockTracker.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(propertyName);

            if (valueProvider == ValueProviderResult.None) return Task.CompletedTask;

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(propertyName, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}

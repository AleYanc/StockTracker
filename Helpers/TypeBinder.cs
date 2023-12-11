using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace StockTracker.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string propertyName = bindingContext.ModelName;
            ValueProviderResult valueProvider = bindingContext.ValueProvider.GetValue(propertyName);

            if (valueProvider == ValueProviderResult.None) return Task.CompletedTask;

            try
            {
                T deserializedValue = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sybon.Archive
{
    public class PermissionFilterAttributeBase : ActionFilterAttribute
    {
        protected static long GetValueFromContext(ActionContext context, string propertyName)
        {
            var model = context.ModelState[""]?.RawValue;
            if (context.ModelState[propertyName] != null)
                return long.Parse(context.ModelState[propertyName].AttemptedValue);
            var prop = model?.GetType().GetProperty(propertyName);
            
            // ReSharper disable once PossibleNullReferenceException
            return (long) prop.GetMethod?.Invoke(model, null);
        }

        protected static TService GetService<TService>(ActionExecutingContext context)
        {
            return (TService) context.HttpContext.RequestServices.GetService(typeof(TService));
        }
    }
}
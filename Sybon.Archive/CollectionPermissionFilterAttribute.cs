using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sybon.Auth.Client.Api;

namespace Sybon.Archive
{
    public class CollectionPermissionFilterAttribute : ActionFilterAttribute
    {
        public enum Type
        {
            Read,
            Write,
            ReadAndWrite
        }

        private readonly Type _type;
        private readonly string _id;

        public CollectionPermissionFilterAttribute(Type type, string id)
        {
            _type = type;
            _id = id;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!(context.Controller is ILogged controller))
                throw new Exception("Controller must derive from ILogged");
            var userId = controller.UserId;
            var permissionsApi = (IPermissionsApi) context.HttpContext.RequestServices.GetService(typeof(IPermissionsApi));
            var permission = permissionsApi.GetToCollection(userId, GetIdValue(context));
            if(!permission.Contains(_type.ToString()))
                context.Result = new UnauthorizedResult();
        }
        
        private long GetIdValue(ActionContext context)
        {
            var model = context.ModelState[""]?.RawValue;
            if (context.ModelState[_id] != null)
                return long.Parse(context.ModelState[_id].AttemptedValue);
            var prop = model?.GetType().GetProperty(_id);
            
            // ReSharper disable once PossibleNullReferenceException
            return (long) prop.GetMethod?.Invoke(model, null);
        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sybon.Auth.Client.Api;

namespace Sybon.Archive
{
    public class ProblemPermissionFilterAttribute : PermissionFilterAttributeBase
    {
        public enum Type
        {
            Read,
            Write,
            ReadAndWrite
        }

        private readonly Type _type;
        private readonly string _id;

        public ProblemPermissionFilterAttribute(Type type, string id)
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

            var haveAccess = GetService<IPermissionsApi>(context)
                .GetToProblem(controller.UserId, GetValueFromContext(context, _id))
                .Contains(_type.ToString());
            
            if(!haveAccess)
                context.Result = new UnauthorizedResult();
        }
    }
}
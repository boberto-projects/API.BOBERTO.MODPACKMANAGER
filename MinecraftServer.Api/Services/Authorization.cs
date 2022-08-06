using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace MinecraftServer.Api.Services
{
    public class Authorization : IAuthorization
    {
        private readonly IActionContextAccessor actionContextAccessor;

        public Authorization(IActionContextAccessor actionContextAccessor)
        {
            this.actionContextAccessor = actionContextAccessor;
        }

        public bool IsAuthorize()
        {
            return false;
        }

        bool AllowAttribute()
        {
            var methodAttribute = (actionContextAccessor.ActionContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.CustomAttributes;
            var controllerAttributes = (actionContextAccessor.ActionContext.ActionDescriptor as ControllerActionDescriptor).ControllerTypeInfo.CustomAttributes;
            Type type = typeof(AllowAnonymousAttribute);

            return AttributeExists(controllerAttributes, type) || AttributeExists(methodAttribute, type);
        }

        bool AttributeExists(IEnumerable<CustomAttributeData> attributes, Type type)
        {
            bool exists = attributes.Any(a => a.AttributeType.Equals(type));
            return exists;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFund.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MyFund.Authorization
{
    public class AuthorizeResourceAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Custom Authorization FilterAttribute, which checks whether a user satisfies a requirement to become authorized to access an IResource object
        /// </summary>
        /// <param name="requirementType">The Type of an IAuthorizationRequirement requirement</param>
        /// <param name="resourceType">The Type of an IResource object</param>
        /// <param name="resourceNavigationIdProperty">The name of the property field of the IResource object, that navigates to its parent</param>
        /// <example>[AuthorizeResource[(typeof(AuthorizationRequirement), typeof(IResource)]</example>
        public AuthorizeResourceAttribute(Type requirementType, Type resourceType, string resourceNavigationIdProperty = "")
            : base(typeof(AuthorizeResourceFilter))
        {
            Arguments = new object[] { requirementType, resourceType, resourceNavigationIdProperty };
        }

        private class AuthorizeResourceFilter : IAsyncActionFilter
        {
            private readonly IAuthorizationService _authorizationService;
            private readonly Type _requirementType;
            private readonly CrowdContext _context;
            private readonly Type _resourceType;
            private readonly string _resourceNavigationIdProperty;

            public AuthorizeResourceFilter(CrowdContext context, 
                                            IAuthorizationService authorizationService,
                                            Type requirementType, 
                                            Type resourceType,
                                            string resourceNavigationIdProperty)
            {
                _context = context;
                _authorizationService = authorizationService;
                _requirementType = requirementType;
                _resourceType = resourceType;
                _resourceNavigationIdProperty = resourceNavigationIdProperty;
            }

            /// <summary>
            /// Evaluates the requirement, as passed through the attribute. If resourceNavigationIdProperty has been set in the attribute instance, it explicitly loads the parent object.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="next"></param>
            /// <returns></returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var resourceId = context.ActionArguments.First().Value;

                var requirement = Activator.CreateInstance(_requirementType) as IAuthorizationRequirement;
                
                var resource = await _context.FindAsync(_resourceType, resourceId);

                if (!string.IsNullOrEmpty(_resourceNavigationIdProperty))
                {
                    await _context.Entry(resource).Reference(_resourceNavigationIdProperty).LoadAsync();
                }
                
                if (resource == null)
                {
                    context.Result = new BadRequestObjectResult(resource);
                    return;
                }
                
                var authorizationResult = await _authorizationService.AuthorizeAsync(context.HttpContext.User, resource, requirement);

                if (!authorizationResult.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                await next();
            }
        }
    }
}

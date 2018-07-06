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
    /// <summary>
    /// Custom Authorization FilterAttribute, which checks whether a user satisfies a requirement to become authorized to access an IResource object
    /// </summary>
    /// <param name="requirementType">The Type of an IAuthorizationRequirement requirement</param>
    /// <param name="resourceType">The Type of an IResource object</param>
    /// <param name="resourceNavigationIdProperty">The name of the property field of the IResource object, that navigates to its parent</param>
    /// <example>
    /// <code>
    /// [AuthorizeResource[(typeof(AuthorizationRequirement), typeof(IResource)]
    /// </code>
    /// </example>
    public class AuthorizeResourceAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Custom Authorization FilterAttribute, which checks whether a user satisfies a requirement to become authorized to access an IResource object
        /// </summary>
        /// <param name="requirementType">The Type of an IAuthorizationRequirement requirement</param>
        /// <param name="resourceType">The Type of an IResource object</param>
        /// <param name="resourceNavigationProperty">The name of the property field of the IResource object, that navigates to its parent</param>
        /// <example>
        /// <code>
        /// [AuthorizeResource[(typeof(AuthorizationRequirement), typeof(IResource)]
        /// </code>
        /// </example>
        public AuthorizeResourceAttribute(Type requirementType, Type resourceType, string resourceNavigationProperty = "")
            : base(typeof(AuthorizeResourceFilter))
        {
            Arguments = new object[] { requirementType, resourceType, resourceNavigationProperty };
        }

        private class AuthorizeResourceFilter : IAsyncActionFilter
        {
            private readonly IAuthorizationService _authorizationService;
            private readonly Type _requirementType;
            private readonly CrowdContext _context;
            private readonly Type _resourceType;
            private readonly string _resourceNavigationProperty;

            public AuthorizeResourceFilter(CrowdContext context, 
                                            IAuthorizationService authorizationService,
                                            Type requirementType, 
                                            Type resourceType,
                                            string resourceNavigationProperty)
            {
                _context = context;
                _authorizationService = authorizationService;
                _requirementType = requirementType;
                _resourceType = resourceType;
                _resourceNavigationProperty = resourceNavigationProperty;
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

                if (!string.IsNullOrEmpty(_resourceNavigationProperty))
                {
                    await _context.Entry(resource).Reference(_resourceNavigationProperty).LoadAsync();
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

using Microsoft.AspNetCore.Authorization;
using MyFund.Extensions;
using MyFund.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFund.Authorization
{
    public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ResourceOwnerRequirement requirement,
                                                       IResource resource)
        {
            if(!(context.Resource is IResource))
            {
                context.Fail();
            }

            if (resource?.GetResourceOwnerId() > 0
                && context.User.GetUserId() == resource.GetResourceOwnerId())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

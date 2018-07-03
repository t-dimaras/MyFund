using Microsoft.AspNetCore.Authorization;
using MyFund.Extensions;
using MyFund.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFund.Services
{
    public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ResourceOwnerRequirement requirement,
                                                       IResource resource)
        {
            if (context.User.GetUserId() == resource.GetResourceOwnerId())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

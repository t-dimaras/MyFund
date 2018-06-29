using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MyFund.Extensions
{
    public static class PrincipalExtensions
    {
        public static long? GetUserId(this ClaimsPrincipal userPrincipal)
        {
            if (!long.TryParse(userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out long userId))
            {
                return null;
            }
            return userId;
        }
    }
}

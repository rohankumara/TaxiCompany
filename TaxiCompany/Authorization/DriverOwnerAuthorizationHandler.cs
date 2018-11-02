using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiCompany.Models;

namespace TaxiCompany.Authorization
{
    public class DriverOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Driver>
    {
        UserManager<ApplicationUser> _userManager;

        public DriverOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                           OperationAuthorizationRequirement requirement, Driver resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.FromResult(0);
            }
            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.FromResult(0);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxiCompany.Models;

namespace TaxiCompany.Authorization
{
    public class AdminDriverAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Driver>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                           OperationAuthorizationRequirement requirement, Driver resource)
        {
            if (context.User == null)
            {
                return Task.FromResult(0);
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.FromResult(0);
            }

            //Admin can do anything..
            if (context.User.IsInRole(Constants.TaxiOfficeAdministratorsRole))
            {
                context.Succeed(requirement);
            }
            return Task.FromResult(0);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthorizationApp.Authorization
{
    public class VerificarHandler : AuthorizationHandler<VerificarRequrimiento>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, VerificarRequrimiento requirement)
        {
            var dateOfBirthClaim = context.User.FindFirst(
            c => c.Type == ClaimTypes.DateOfBirth);
          //  var dateOfBirthClaim = DateTime.Now.AddYears(-5);
            if (dateOfBirthClaim is null)
            {
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.Verificar)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

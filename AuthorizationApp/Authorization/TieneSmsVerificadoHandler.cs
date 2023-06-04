using Microsoft.AspNetCore.Authorization;

namespace AuthorizationApp.Authorization
{
    public class TieneSmsVerificadoHandler : AuthorizationHandler<TieneSmsVerificadoRequerimiento>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, TieneSmsVerificadoRequerimiento requirement)
        {
            var smsVerificadoClaim = context.User.FindFirst(
            c => c.Type == "SmsVerificado");
            if (smsVerificadoClaim is null)
            {
                return Task.CompletedTask;
            }

            var smsVerificado = Convert.ToBoolean(smsVerificadoClaim.Value);           

            if (smsVerificado == requirement.Verificar)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

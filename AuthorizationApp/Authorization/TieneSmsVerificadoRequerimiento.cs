using Microsoft.AspNetCore.Authorization;

namespace AuthorizationApp.Authorization
{
    public class TieneSmsVerificadoRequerimiento : IAuthorizationRequirement
    {
        public TieneSmsVerificadoRequerimiento() =>
            Verificar = true;

        public bool Verificar { get; }
    }
}

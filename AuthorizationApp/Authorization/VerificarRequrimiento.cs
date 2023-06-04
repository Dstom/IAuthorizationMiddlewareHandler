using Microsoft.AspNetCore.Authorization;

namespace AuthorizationApp.Authorization
{
    public class VerificarRequrimiento : IAuthorizationRequirement
    {
        public VerificarRequrimiento(int minimumAge) =>
             Verificar = minimumAge;

        public int Verificar { get; }
    }
}

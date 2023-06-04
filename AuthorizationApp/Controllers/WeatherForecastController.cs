using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        [Authorize(Policy = "SmsVerificado")]
        [HttpGet]
        public List<string> Get()
        {
            var users = new List<string>
            {
                "Satinder Singh",
                "Amit Sarna",
                "Amit Sarna",
                "Davin Jon"
            };

            return users;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] string smsVerificado)
        {
            // Else we generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes("312312312312321213321231213213321312321");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                  {
                        new Claim(ClaimTypes.Name, "ra"),
                        new Claim("SmsVerificado", smsVerificado),
                        //new Claim(ClaimTypes.DateOfBirth, DateTime.Now.AddYears(-Convert.ToInt32(age)).ToString()),

                  }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                     new SymmetricSecurityKey(tokenKey), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenasd = tokenHandler.WriteToken(token);
        
        
            return Ok(tokenasd);
        }
    }
}
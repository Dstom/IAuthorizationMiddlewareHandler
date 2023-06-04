using AuthorizationApp.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes("312312312312321213321231213213321312321");
   // o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.Name,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
       // ValidateIssuerSigningKey = true,
        //ValidIssuer = "https://codepedia.info",
        //ValidAudience = "odepedia.info",
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtLeast21", policy => policy.Requirements.Add(new VerificarRequrimiento(21)));
    options.AddPolicy("SmsVerificado", policy => policy.Requirements.Add(new TieneSmsVerificadoRequerimiento()));
});
builder.Services.AddSingleton<IAuthorizationHandler, VerificarHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, TieneSmsVerificadoHandler>();

builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, SampleAuthorizationMiddlewareResultHandler>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class SampleAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was forbidden and the resource had a specific requirement,
        // provide a custom 404 response.
        if (authorizeResult.Forbidden
            && authorizeResult.AuthorizationFailure!.FailedRequirements
                .OfType<TieneSmsVerificadoRequerimiento>().Any())
        {
            // Return a 404 to make it appear as if the resource doesn't exist.
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            var json = JsonSerializer.Serialize(
                new {Codigo = "06" ,Mensaje = "No autorizado para esta operacion" } // Switch this with your object
                );

            // Write to the response
            await context.Response.WriteAsync(json);
            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
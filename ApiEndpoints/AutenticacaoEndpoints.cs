using CatalogoApiMinimalAPI.Models;
using CatalogoApiMinimalAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogoApiMinimalAPI.ApiEndpoints
{
    public static class AutenticacaoEndpoints
    {
        public static void MapAutenticacaoEndpoints(this WebApplication app) {

            //Endpoint Login
            app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) => {
                if (user == null)
                {
                    return Results.BadRequest("Login Inválido");
                }
                if (user.UserName == "SamuelMBote" && user.Password == "mbote@123")
                {
                    string tokenstring = tokenService.GerarToken(app.Configuration["Jwt:Key"], app.Configuration["Jwt:Issuer"], app.Configuration["Jwt:Audience"], user);
                    return Results.Ok(new { token = tokenstring });
                }
                else
                {
                    return Results.BadRequest("Login Inválido");
                }
            }).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Autenticacao");
        }
    }
}

using CatalogoApiMinimalAPI.ApiEndpoints;
using CatalogoApiMinimalAPI.AppServicesExtensions;
using CatalogoApiMinimalAPI.Context;
using CatalogoApiMinimalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJWT();






WebApplication app = builder.Build();

//Definir os endpoints 
app.MapAutenticacaoEndpoints();

app.MapCategoriasEndpoints();

app.MapProdutosEndpoints();



// Configure the HTTP request pipeline.
var environment = app.Environment;

app.UseExceptionHandling(environment).UseSwaggerMiddleware().UseAppCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.Run();

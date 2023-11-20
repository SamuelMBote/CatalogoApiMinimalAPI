using CatalogoApiMinimalAPI.Context;
using CatalogoApiMinimalAPI.Models;
using CatalogoApiMinimalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => { 
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalogo Api", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { 
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        { 
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()

            }
    });
});


string? connString = builder.Configuration.GetConnectionString("SqlServerConnection");
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(connString));

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
WebApplication app = builder.Build();

//Endpoint Login
app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) => {
    if(user == null)
    {
        return Results.BadRequest("Login Inválido");
    }
    if(user.UserName == "SamuelMBote" && user.Password == "mbote@123")
    {
        string tokenstring = tokenService.GerarToken(app.Configuration["Jwt:Key"], app.Configuration["Jwt:Issuer"], app.Configuration["Jwt:Audience"], user);
        return Results.Ok(new { token = tokenstring });
    }
    else
    {
        return Results.BadRequest("Login Inválido");
    }
}).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Autenticacao");

//Definir os endpoints 
app.MapGet("/categorias", async( DataContext db) =>
{
    List<Categoria>? categorias = await db.Categorias.AsNoTracking().ToListAsync();
    if (categorias == null || categorias.Count == 0)
        return Results.NotFound("Não há categorias cadastradas");
    return Results.Ok(categorias);
}).RequireAuthorization();

app.MapGet("/categorias/{id:int}", async (int id, DataContext db) =>
{
    return await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound($"Categoria com id: ${id} não encontrada");
}).RequireAuthorization();

app.MapPost("/categorias", async (Categoria categoria, DataContext db) => {
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();
    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
}).RequireAuthorization();
app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, DataContext db) =>
{
    if (categoria.CategoriaId != id)
        return Results.BadRequest($"Id da URI diferente do id da categoria que deseja alterar");
    Categoria? categoriadb = await db.Categorias.FindAsync(id);
    if (categoriadb is null) return Results.NotFound($"Categoria com id: ${id} não encontrada");

    categoriadb.Nome = categoria.Nome;
    categoriadb.Descricao = categoria.Descricao;
    await db.SaveChangesAsync();
    return Results.Ok( categoriadb );
    
});
app.MapDelete("/categorias/{id:int}", async (int id, DataContext db) => {
    Categoria? categoria = await db.Categorias.FindAsync(id);
    if (categoria is null)
        return Results.NotFound($"Categoria com id: ${id} não encontrada");
    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/produtos", async (DataContext db) => {

    List<Produto>? produtos = await db.Produtos.AsNoTracking().ToListAsync();
    if (produtos == null || produtos.Count == 0)
        return Results.NotFound("Não há produtos cadastrados");
    return Results.Ok(produtos);
});

app.MapGet("/produtos/{id:int}", async (int id, DataContext db) => {
    return await db.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound($"Produto com id: ${id} não encontrado");
});

app.MapPost("/produtos", async (Produto produto, DataContext db) => {
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
});

app.MapPut("/produtos/{id:int}", async (int id, Produto produto, DataContext db) => {

    if (produto.ProdutoId != id)
        return Results.BadRequest($"Id da URI diferente do id do produto que deseja alterar");
    Produto? produtodb = await db.Produtos.FindAsync(id);
    if (produtodb is null) return Results.NotFound($"Produto com id: ${id} não encontradp");

    produtodb.Nome = produto.Nome;
    produtodb.Descricao = produto.Descricao;
    produtodb.Preco = produto.Preco;
    produtodb.Imagem = produto.Imagem;
    produtodb.DataCompra = produto.DataCompra;
    produtodb.Estoque = produto.Estoque;
    produtodb.CategoriaId = produto.CategoriaId;

    await db.SaveChangesAsync();
    return Results.Ok(produtodb);
});
app.MapDelete("/produtos/{id:int}", async (int id, DataContext db) => {

    Produto? produto = await db.Produtos.FindAsync(id);
    if (produto is null)
        return Results.NotFound($"Produto com id: ${id} não encontrado");
    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.Run();

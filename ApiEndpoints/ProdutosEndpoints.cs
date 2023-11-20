using CatalogoApiMinimalAPI.Context;
using CatalogoApiMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApiMinimalAPI.ApiEndpoints
{
    public static class ProdutosEndpoints
    {
        public static void MapProdutosEndpoints(this WebApplication app) {

            #region GetAll
            app.MapGet("/produtos", async (DataContext db) => {

                List<Produto>? produtos = await db.Produtos.AsNoTracking().ToListAsync();
                if (produtos == null || produtos.Count == 0)
                    return Results.NotFound("Não há produtos cadastrados");
                return Results.Ok(produtos)
            }).RequireAuthorization().WithTags("Produtos");
            #endregion

            #region GetById
            app.MapGet("/produtos/{id:int}", async (int id, DataContext db) => {
                return await db.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound($"Produto com id: ${id} não encontrado");
            }).RequireAuthorization().WithTags("Produtos");
            #endregion

            #region Post
            app.MapPost("/produtos", async (Produto produto, DataContext db) => {
                db.Produtos.Add(produto);
                await db.SaveChangesAsync();
                return Results.Created($"/produtos/{produto.ProdutoId}", produto);
            }).RequireAuthorization().WithTags("Produtos"); 
            #endregion

            #region Put
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
            }).RequireAuthorization().WithTags("Produtos");
            #endregion

            #region Delete
            app.MapDelete("/produtos/{id:int}", async (int id, DataContext db) => {

                Produto? produto = await db.Produtos.FindAsync(id);
                if (produto is null)
                    return Results.NotFound($"Produto com id: ${id} não encontrado");
                db.Produtos.Remove(produto);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization().WithTags("Produtos");
            #endregion
        }
    }
}

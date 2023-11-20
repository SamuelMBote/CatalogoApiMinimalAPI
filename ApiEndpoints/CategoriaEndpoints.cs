using CatalogoApiMinimalAPI.Context;
using CatalogoApiMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApiMinimalAPI.ApiEndpoints
{
    public static class CategoriaEndpoints
    {
        public static void MapCategoriasEndpoints(this WebApplication app)
        {
            #region GetAll
            app.MapGet("/categorias", async (DataContext db) =>
            {
                List<Categoria>? categorias = await db.Categorias.AsNoTracking().ToListAsync();
                if (categorias == null || categorias.Count == 0)
                    return Results.NotFound("Não há categorias cadastradas");
                return Results.Ok(categorias);
            }).RequireAuthorization().WithTags("Categorias");
            #endregion

            #region GetByID
            app.MapGet("/categorias/{id:int}", async (int id, DataContext db) =>
            {
                return await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound($"Categoria com id: ${id} não encontrada");
            }).RequireAuthorization().WithTags("Categorias");
            #endregion

            #region Post
            app.MapPost("/categorias", async (Categoria categoria, DataContext db) => {
                db.Categorias.Add(categoria);
                await db.SaveChangesAsync();
                return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
            }).RequireAuthorization().WithTags("Categorias");
            #endregion

            #region Put
            app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, DataContext db) =>
            {
                if (categoria.CategoriaId != id)
                    return Results.BadRequest($"Id da URI diferente do id da categoria que deseja alterar");
                Categoria? categoriadb = await db.Categorias.FindAsync(id);
                if (categoriadb is null) return Results.NotFound($"Categoria com id: ${id} não encontrada");

                categoriadb.Nome = categoria.Nome;
                categoriadb.Descricao = categoria.Descricao;
                await db.SaveChangesAsync();
                return Results.Ok(categoriadb);

            }).RequireAuthorization().WithTags("Categorias");
            #endregion

            #region Delete
            app.MapDelete("/categorias/{id:int}", async (int id, DataContext db) => {
                Categoria? categoria = await db.Categorias.FindAsync(id);
                if (categoria is null)
                    return Results.NotFound($"Categoria com id: ${id} não encontrada");
                db.Categorias.Remove(categoria);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization().WithTags("Categorias");
            #endregion

        }
    }
}

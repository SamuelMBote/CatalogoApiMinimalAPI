using CatalogoApiMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApiMinimalAPI.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Categoria>().HasKey(x => x.CategoriaId);
            modelBuilder.Entity<Categoria>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Categoria>().Property(x=>x.Descricao).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Produto>().HasKey(x => x.ProdutoId);
            modelBuilder.Entity<Produto>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Produto>().Property(x => x.Descricao).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<Produto>().Property(x => x.Imagem).HasMaxLength(100);
            modelBuilder.Entity<Produto>().Property(x => x.Preco).HasPrecision(14, 2);

            modelBuilder.Entity<Produto>().HasOne(x => x.Categoria).WithMany(x => x.Produtos).HasForeignKey(x => x.CategoriaId);
        }
    }
}

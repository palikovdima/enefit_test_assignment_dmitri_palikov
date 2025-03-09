using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext, IDataContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public Microsoft.EntityFrameworkCore.DbSet<Product> Products { get; set; } = null!;

        public Microsoft.EntityFrameworkCore.DbSet<Transaction> Transactions { get; set; } = null!;

        public Microsoft.EntityFrameworkCore.DbSet<TransactionProduct> TransactionProducts { get; set; } = null!;

        public new Microsoft.EntityFrameworkCore.DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

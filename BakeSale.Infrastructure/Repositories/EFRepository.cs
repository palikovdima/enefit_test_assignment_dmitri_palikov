using Domain.Exceptions;
using Domain.Entities.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected DbContext RepositoryDbContext;
        protected Microsoft.EntityFrameworkCore.DbSet<TEntity> RepositoryDbSet;

        public EFRepository(IDataContext dataContext)
        {
            RepositoryDbContext = dataContext as DbContext ?? throw new ArgumentNullException(nameof(dataContext));
            RepositoryDbSet = RepositoryDbContext.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> All() => RepositoryDbSet.ToList();
        public virtual async Task<IEnumerable<TEntity>> AllAsync() => await RepositoryDbSet.ToListAsync();
        public virtual TEntity? Find(params object[] id)
        {
            var entity = RepositoryDbSet.Find(id);
            if (entity == null)
            {
                throw new EntityNotFoundException($"Entity of type {typeof(TEntity).Name} with ID {string.Join(",", id)} not found.");
            }
            return entity;
        }
        public virtual async Task<TEntity> FindAsync(params object[] id)
        {
            var entity = await RepositoryDbSet.FindAsync(id);
            if (entity == null)
            {
                throw new EntityNotFoundException($"Entity of type {typeof(TEntity).Name} with ID {string.Join(",", id)} not found.");
            }
            return entity;
        }
        public virtual void Add(TEntity entity) => RepositoryDbSet.Add(entity);
        public virtual async Task AddAsync(TEntity entity) => await RepositoryDbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await RepositoryDbContext.Set<TEntity>().AddRangeAsync(entities);
        }
        public TEntity Update(TEntity entity) => RepositoryDbSet.Update(entity).Entity;
        public void Remove(TEntity entity) => RepositoryDbSet.Remove(entity);
        public void Remove(params object[] id)
        {
            var entity = RepositoryDbSet.Find(id);
            Remove(entity!);
        }

        public async Task<int> SaveChangesAsync() => await RepositoryDbContext.SaveChangesAsync();

    }
}

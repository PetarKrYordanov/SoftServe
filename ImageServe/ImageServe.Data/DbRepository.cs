using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ImageServe.Data.Common;
using ImageServe.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageServe.Data
{
    public class DbRepository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class
    {
        private readonly ImageServeDbContext context;
        private DbSet<TEntity> dbSet;

        public DbRepository(ImageServeDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<TEntity>();
        }

        public Task AddAsync(TEntity entity)
        {
            return this.dbSet.AddAsync(entity);
        }

        public IQueryable<TEntity> All()
        {
            return this.dbSet;
        }

        public void Delete(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }

        
    }
}

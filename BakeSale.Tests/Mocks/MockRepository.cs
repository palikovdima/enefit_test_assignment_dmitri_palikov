using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Mocks
{
    public static class MockRepository
    {
        public static Mock<ProductRepository> GetProductRepository(Type testClassType)
        {
            AppDbContext dbContext = FakeDatabase.InitilizeDbContext(testClassType);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var mockRepo = new Mock<ProductRepository>(dbContext) { CallBase = true };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns<Product>(async p =>
                {
                    await dbContext.Set<Product>().AddAsync(p);
                    await dbContext.SaveChangesAsync();
                });
            return mockRepo;
        }
        public static Mock<IRepository<TEntity>> GetRepository<TEntity>(Type testClassType) where TEntity : BaseEntity
        {
            return new Mock<IRepository<TEntity>>();
        }
    }
}

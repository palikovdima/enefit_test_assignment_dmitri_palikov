using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Infrastructure.Repositories.Product
{
    public class ProductRepository : EFRepository<Domain.Entities.Product>, IRepository<Domain.Entities.Product>
    {
        public ProductRepository(IDataContext dataContext) : base(dataContext) {}

        override
        public void Add(Domain.Entities.Product entity)
        {
            ValidateProduct(entity);
            base.Add(entity);
        }

        override
        public async Task AddAsync(Domain.Entities.Product entity) {
            ValidateProduct(entity);
            await base.AddAsync(entity);
        }

        private void ValidateProduct(Domain.Entities.Product entity)
        {
            if (entity.Quantity < 0)
            {
                throw new Domain.Exceptions.InvalidEntityException("Product quantity cannot be negative.");
            }
            if (entity.Price < 0)
            {
                throw new Domain.Exceptions.InvalidEntityException("Product price cannot be negative.");
            }
        }
    }
}

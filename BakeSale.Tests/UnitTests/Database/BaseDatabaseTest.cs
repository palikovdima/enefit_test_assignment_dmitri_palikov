using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Interfaces;
using Infrastructure.Repositories.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.UnitTests.Database
{
    public abstract class BaseDatabaseTest<TEntity> where TEntity : BaseEntity
    {
        protected readonly Mock<IRepository<TEntity>> _repositoryMock;
        protected readonly Mock<ILogger<BaseDatabaseTest<TEntity>>> _loggerMock;

        protected BaseDatabaseTest()
        {
            _repositoryMock = new Mock<IRepository<TEntity>>();
            _loggerMock = new Mock<ILogger<BaseDatabaseTest<TEntity>>>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Mocks
{
    public static class FakeDatabase
    {
        public static AppDbContext InitilizeDbContext(Type testClassType)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"{testClassType.Name}_Db")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            return new AppDbContext(options);
        }
    }
}

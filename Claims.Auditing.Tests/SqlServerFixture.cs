using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Claims.Auditing.Tests
{
    /// <summary>
    /// Spins up a single SQL Server container shared across all tests in a class,
    /// and applies migrations once so <see cref="AuditContext"/> tests can run against a real schema.
    /// </summary>
    public class SqlServerFixture : IAsyncLifetime
    {
        private readonly MsSqlBuilder _builder = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest");
        private MsSqlContainer? _container;

        public AuditContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AuditContext>()
                .UseSqlServer(_container!.GetConnectionString())
                .Options;

            return new AuditContext(options);
        }

        public async Task InitializeAsync()
        {
            _container = _builder.Build();
            await _container.StartAsync();

            using var context = CreateContext();
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            if (_container is not null)
            {
                await _container.DisposeAsync();
            }
        }
    }
}

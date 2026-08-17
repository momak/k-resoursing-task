using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Claims.Data.Tests
{
    /// <summary>
    /// Spins up a single MongoDB container shared across all tests in a class.
    /// </summary>
    public class MongoFixture : IAsyncLifetime
    {
        private readonly MongoDbBuilder _builder = new MongoDbBuilder("mongo:latest");
        private MongoDbContainer? _container;

        public ClaimsContext CreateContext()
        {
            var client = new MongoClient(_container!.GetConnectionString());
            var database = client.GetDatabase("claims_tests");

            var options = new DbContextOptionsBuilder<ClaimsContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options;

            return new ClaimsContext(options);
        }

        public async Task InitializeAsync()
        {
            _container = _builder.Build();
            await _container.StartAsync();
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

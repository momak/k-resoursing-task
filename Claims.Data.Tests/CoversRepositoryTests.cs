using Claims.Data.Repositories;

namespace Claims.Data.Tests
{
    public class CoversRepositoryTests : IClassFixture<MongoFixture>
    {
        private readonly MongoFixture _fixture;

        public CoversRepositoryTests(MongoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task AddAsync_PersistsCover()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new CoversRepository(context);

            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Type = CoverType.Yacht,
                Premium = 1500m
            };

            // Act
            await repository.AddAsync(cover);

            // Assert
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new CoversRepository(verifyContext);
            var persisted = await verifyRepository.GetByIdAsync(cover.Id);

            Assert.NotNull(persisted);
            Assert.Equal(cover.Premium, persisted!.Premium);
            Assert.Equal(cover.Type, persisted.Type);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new CoversRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPersistedCovers()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new CoversRepository(context);

            var coverA = new Cover { Id = Guid.NewGuid().ToString(), StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(10), Type = CoverType.Tanker, Premium = 1000m };
            var coverB = new Cover { Id = Guid.NewGuid().ToString(), StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(20), Type = CoverType.BulkCarrier, Premium = 2000m };

            await repository.AddAsync(coverA);
            await repository.AddAsync(coverB);

            // Act
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new CoversRepository(verifyContext);
            var all = await verifyRepository.GetAllAsync();

            // Assert
            Assert.Contains(all, c => c.Id == coverA.Id);
            Assert.Contains(all, c => c.Id == coverB.Id);
        }

        [Fact]
        public async Task DeleteAsync_RemovesCover()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new CoversRepository(context);

            var cover = new Cover { Id = Guid.NewGuid().ToString(), StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(5), Type = CoverType.PassengerShip, Premium = 500m };
            await repository.AddAsync(cover);

            // Act
            using var deleteContext = _fixture.CreateContext();
            var deleteRepository = new CoversRepository(deleteContext);
            var toDelete = await deleteRepository.GetByIdAsync(cover.Id);
            await deleteRepository.DeleteAsync(toDelete!);

            // Assert
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new CoversRepository(verifyContext);
            var result = await verifyRepository.GetByIdAsync(cover.Id);

            Assert.Null(result);
        }
    }
}

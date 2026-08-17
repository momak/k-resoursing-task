
using Claims.Data.Repositories;

namespace Claims.Data.Tests
{
    public class ClaimsRepositoryTests : IClassFixture<MongoFixture>
    {
        private readonly MongoFixture _fixture;

        public ClaimsRepositoryTests(MongoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task AddAsync_PersistsClaim()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new ClaimsRepository(context);

            var claim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = "cover-1",
                Name = "Test claim",
                Created = DateTime.UtcNow,
                Type = ClaimType.Collision,
                DamageCost = 500m
            };

            // Act
            await repository.AddAsync(claim);

            // Assert
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new ClaimsRepository(verifyContext);
            var persisted = await verifyRepository.GetByIdAsync(claim.Id);

            Assert.NotNull(persisted);
            Assert.Equal(claim.Name, persisted!.Name);
            Assert.Equal(claim.DamageCost, persisted.DamageCost);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new ClaimsRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPersistedClaims()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new ClaimsRepository(context);

            var claimA = new Claim { Id = Guid.NewGuid().ToString(), CoverId = "cover-1", Name = "A", Created = DateTime.UtcNow, Type = ClaimType.Fire, DamageCost = 100m };
            var claimB = new Claim { Id = Guid.NewGuid().ToString(), CoverId = "cover-1", Name = "B", Created = DateTime.UtcNow, Type = ClaimType.BadWeather, DamageCost = 200m };

            await repository.AddAsync(claimA);
            await repository.AddAsync(claimB);

            // Act
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new ClaimsRepository(verifyContext);
            var all = await verifyRepository.GetAllAsync();

            // Assert
            Assert.Contains(all, c => c.Id == claimA.Id);
            Assert.Contains(all, c => c.Id == claimB.Id);
        }

        [Fact]
        public async Task DeleteAsync_RemovesClaim()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new ClaimsRepository(context);

            var claim = new Claim { Id = Guid.NewGuid().ToString(), CoverId = "cover-1", Name = "ToDelete", Created = DateTime.UtcNow, Type = ClaimType.Grounding, DamageCost = 300m };
            await repository.AddAsync(claim);

            // Act
            using var deleteContext = _fixture.CreateContext();
            var deleteRepository = new ClaimsRepository(deleteContext);
            var toDelete = await deleteRepository.GetByIdAsync(claim.Id);
            await deleteRepository.DeleteAsync(toDelete!);

            // Assert
            using var verifyContext = _fixture.CreateContext();
            var verifyRepository = new ClaimsRepository(verifyContext);
            var result = await verifyRepository.GetByIdAsync(claim.Id);

            Assert.Null(result);
        }
    }
}

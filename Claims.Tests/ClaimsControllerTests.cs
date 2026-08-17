using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;


namespace Claims.Tests
{
    public class ClaimsControllerTests : IClassFixture<ClaimsApiFactory>
    {
        private readonly ClaimsApiFactory _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ClaimsControllerTests(ClaimsApiFactory factory)
        {
            _factory = factory;
            _jsonOptions = factory.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        }

        [Fact]
        public async Task Get_Claims_ReturnsOkWithClaimsFromService()
        {
            // Arrange
            var claims = new List<Claim>
        {
            new() { Id = "claim-1", CoverId = "cover-1", Name = "Test", Type = ClaimType.Collision, DamageCost = 500m }
        };
            _factory.ClaimsServiceMock.Setup(s => s.GetClaimsAsync()).ReturnsAsync(claims);

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Claims");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<Claim>>(_jsonOptions);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("claim-1", result[0].Id);
        }

        [Fact]
        public async Task Get_ClaimById_ReturnsOkWithClaim()
        {
            // Arrange
            var claim = new Claim { Id = "claim-1", CoverId = "cover-1", Name = "Test", Type = ClaimType.Fire, DamageCost = 500m };
            _factory.ClaimsServiceMock.Setup(s => s.GetClaimAsync("claim-1")).ReturnsAsync(claim);

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Claims/claim-1");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Claim>(_jsonOptions);

            Assert.NotNull(result);
            Assert.Equal("claim-1", result!.Id);
        }

        [Fact]
        public async Task Post_Claim_ReturnsOkWithCreatedClaim()
        {
            // Arrange
            var claim = new Claim
            {
                Id = "placeholder", // required by model binding; real value is server-generated and irrelevant here
                CoverId = "cover-1",
                Name = "New claim",
                Type = ClaimType.Collision,
                DamageCost = 500m
            };
            var created = new Claim { Id = "generated-id", CoverId = "cover-1", Name = "New claim", Type = ClaimType.Collision, DamageCost = 500m };
            _factory.ClaimsServiceMock.Setup(s => s.CreateClaimAsync(It.IsAny<Claim>())).ReturnsAsync(created);

            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/Claims", claim, _jsonOptions);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"Expected success but got {(int)response.StatusCode}: {body}");

            var result = await response.Content.ReadFromJsonAsync<Claim>(_jsonOptions);
            Assert.NotNull(result);
            Assert.Equal("generated-id", result!.Id);
        }

        [Fact]
        public async Task Delete_Claim_ReturnsOk()
        {
            // Arrange
            _factory.ClaimsServiceMock.Setup(s => s.DeleteClaimAsync("claim-1")).Returns(Task.CompletedTask);

            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/Claims/claim-1");

            // Assert
            response.EnsureSuccessStatusCode();
            _factory.ClaimsServiceMock.Verify(s => s.DeleteClaimAsync("claim-1"), Times.Once);
        }
    }
}

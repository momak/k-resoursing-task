using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Claims.Tests
{
    public class CoversControllerTests : IClassFixture<ClaimsApiFactory>
    {
        private readonly ClaimsApiFactory _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public CoversControllerTests(ClaimsApiFactory factory)
        {
            _factory = factory;
            _jsonOptions = factory.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        }

        [Fact]
        public async Task Get_Covers_ReturnsOkWithCoversFromService()
        {
            // Arrange
            var covers = new List<Cover>
            {
                new() { Id = "cover-1", Type = CoverType.Yacht, Premium = 1500m }
            };
            _factory.CoversServiceMock.Setup(s => s.GetCoversAsync()).ReturnsAsync(covers);

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Covers");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<Cover>>(_jsonOptions);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("cover-1", result[0].Id);
        }

        [Fact]
        public async Task Post_Cover_ReturnsOkWithCreatedCover()
        {
            // Arrange
            var cover = new Cover
            {
                Id = "placeholder",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Type = CoverType.Tanker
            };
            var created = new Cover { Id = "generated-id", Type = CoverType.Tanker, Premium = 2000m };
            _factory.CoversServiceMock.Setup(s => s.CreateCoverAsync(It.IsAny<Cover>())).ReturnsAsync(created);

            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/Covers", cover, _jsonOptions);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"Expected success but got {(int)response.StatusCode}: {body}");

            var result = await response.Content.ReadFromJsonAsync<Cover>(_jsonOptions);
            Assert.NotNull(result);
            Assert.Equal("generated-id", result!.Id);
            Assert.Equal(2000m, result.Premium);
        }

        [Fact]
        public async Task Delete_Cover_ReturnsOk()
        {
            // Arrange
            _factory.CoversServiceMock.Setup(s => s.DeleteCoverAsync("cover-1")).Returns(Task.CompletedTask);

            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/Covers/cover-1");

            // Assert
            response.EnsureSuccessStatusCode();
            _factory.CoversServiceMock.Verify(s => s.DeleteCoverAsync("cover-1"), Times.Once);
        }
    }
}

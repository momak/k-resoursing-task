using Claims.Services.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Claims.Tests
{
    public class ClaimsApiFactory : WebApplicationFactory<Program>
    {
        public Mock<IClaimsService> ClaimsServiceMock { get; } = new();
        public Mock<ICoversService> CoversServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IClaimsService>();
                services.AddSingleton(ClaimsServiceMock.Object);

                services.RemoveAll<ICoversService>();
                services.AddSingleton(CoversServiceMock.Object);
            });
        }
    }
}

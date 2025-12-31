using SharboAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SharboAPI.Tests.Fixtures;

public class ApiFactoryFixture : WebApplicationFactory<Program>
{
    public BehaviorFake Behavior { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IFirebaseService>();
            services.RemoveAll<IMemeService>();

            services.AddSingleton(Behavior);
            services.AddScoped<IMemeService, MemeServiceFake>();
        });
    }
}

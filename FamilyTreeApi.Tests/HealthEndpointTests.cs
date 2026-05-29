using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLib.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType.FullName?.Contains("FamilyTreeContext") == true ||
                d.ImplementationType?.FullName?.Contains("FamilyTreeContext") == true ||
                d.ImplementationInstance?.GetType().FullName?.Contains("FamilyTreeContext") == true ||
                d.ImplementationFactory?.Method.ReturnType.FullName?.Contains("FamilyTreeContext") == true
            ).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<FamilyTreeContext>(options =>
            {
                options.UseInMemoryDatabase("FamilyTreeApiTest");
            });
        });
    }
}

public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetRootReturnsHelloWorld()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal("Hello World!", content);
    }
}

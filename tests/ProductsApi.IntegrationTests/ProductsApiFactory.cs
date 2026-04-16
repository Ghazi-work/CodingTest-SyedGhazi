using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductsApi.Common.Persistence;

namespace ProductsApi.IntegrationTests;

public class ProductsApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ProductsDbContext>))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            services.AddDbContext<ProductsDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        });
    }
}

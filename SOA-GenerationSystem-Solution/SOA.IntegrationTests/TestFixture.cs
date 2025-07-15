using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SOA.UsersAPI;
using SOA.Persistence;

namespace SOA.IntegrationTests
{
    public class TestFixture
    {
        protected WebApplicationFactory<Program> Factory = null!;

        [SetUp]
        public void Setup()
        {
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("SOA_Users_DB");
                        });

                        var sp = services.BuildServiceProvider();

                        using var scope = sp.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        db.Database.EnsureCreated();
                    });
                });
        }
    }
}

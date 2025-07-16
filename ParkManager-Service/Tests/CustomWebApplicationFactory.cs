using Microsoft.AspNetCore.Mvc.Testing;
using ParkManager_Service.Data;

namespace ParkManager_Service.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Corrige o ContentRootPath, essencial para evitar DirectoryNotFoundException
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseEnvironment("Test");
        }

        public AppDbContext CreateDbContext()
        {
            var scopeFactory = Services.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }
    }
}

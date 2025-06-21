using Microsoft.AspNetCore.Mvc.Testing;

namespace ParkManager_Service.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Corrige o ContentRootPath, essencial para evitar DirectoryNotFoundException
            builder.UseContentRoot(Directory.GetCurrentDirectory());

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

            builder.UseEnvironment("Test");
        }
    }
}

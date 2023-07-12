using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using System.Data.Common;

namespace IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //The SUT's database context is registered in Program.cs.
            //The test app's builder.ConfigureServices callback is executed after the app's Program.cs code is executed.
            //To use a different database for the tests than the app's database, the app's database context must be replaced in
            //builder.ConfigureServices.
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApiContext>))!;

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection))!;

            services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<ApiContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }
}
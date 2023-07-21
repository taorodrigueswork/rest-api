namespace API;

public static class WebApplicationBuilder
{
    public static IHostBuilder ConfigureAppSettings(this IHostBuilder host)
    {
        host.ConfigureAppConfiguration((ctx, builder) =>
        {
            var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            builder.AddJsonFile("appsettings.json", false, true);
            builder.AddJsonFile($"appsettings.{enviroment}.json", true, true);
            builder.AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

            builder.AddEnvironmentVariables();
        });

        return host;
    }
}
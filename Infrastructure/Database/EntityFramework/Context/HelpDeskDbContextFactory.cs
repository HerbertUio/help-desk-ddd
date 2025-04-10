using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Database.EntityFramework.Context;

public class HelpDeskDbContextFactory : IDesignTimeDbContextFactory<HelpDeskDbContext>
{
    public HelpDeskDbContext CreateDbContext(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        // Asume que la API está un nivel arriba respecto a Infrastructure
        string basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Api"));

        if (!Directory.Exists(basePath)) // Fallback
        {
            basePath = Directory.GetCurrentDirectory();
        }

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            // Busca secretos asociados al proyecto API (reemplaza GUID si es diferente)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada.");
        }

        Console.WriteLine($"DbContextFactory using ConnectionString: {connectionString.Substring(0, Math.Max(0, connectionString.IndexOf("Password="))) + "Password=***"}");

        var optionsBuilder = new DbContextOptionsBuilder<HelpDeskDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new HelpDeskDbContext(optionsBuilder.Options);
    }
}
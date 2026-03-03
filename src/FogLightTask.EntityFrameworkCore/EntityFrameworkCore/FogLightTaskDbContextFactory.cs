using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FogLightTask.EntityFrameworkCore;

public class FogLightTaskDbContextFactory : IDesignTimeDbContextFactory<FogLightTaskDbContext>
{
    public FogLightTaskDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        FogLightTaskEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<FogLightTaskDbContext>()
            .UseSqlServer(configuration.GetConnectionString("ProductionDb"));
        
        return new FogLightTaskDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FogLightTask.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
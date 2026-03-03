using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FogLightTask.Data;
using Volo.Abp.DependencyInjection;

namespace FogLightTask.EntityFrameworkCore;

public class EntityFrameworkCoreFogLightTaskDbSchemaMigrator
    : IFogLightTaskDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFogLightTaskDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the FogLightTaskDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FogLightTaskDbContext>()
            .Database
            .MigrateAsync();
    }
}

using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FogLightTask.Data;

/* This is used if database provider does't define
 * IFogLightTaskDbSchemaMigrator implementation.
 */
public class NullFogLightTaskDbSchemaMigrator : IFogLightTaskDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

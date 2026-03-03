using System.Threading.Tasks;

namespace FogLightTask.Data;

public interface IFogLightTaskDbSchemaMigrator
{
    Task MigrateAsync();
}

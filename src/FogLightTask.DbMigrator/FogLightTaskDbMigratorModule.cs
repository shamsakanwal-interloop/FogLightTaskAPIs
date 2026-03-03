using FogLightTask.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FogLightTask.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FogLightTaskEntityFrameworkCoreModule),
    typeof(FogLightTaskApplicationContractsModule)
)]
public class FogLightTaskDbMigratorModule : AbpModule
{
}

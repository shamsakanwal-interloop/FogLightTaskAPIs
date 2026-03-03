using Volo.Abp.Modularity;

namespace FogLightTask;

[DependsOn(
    typeof(FogLightTaskDomainModule),
    typeof(FogLightTaskTestBaseModule)
)]
public class FogLightTaskDomainTestModule : AbpModule
{

}

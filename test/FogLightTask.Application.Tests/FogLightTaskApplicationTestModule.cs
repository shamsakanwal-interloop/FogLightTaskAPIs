using Volo.Abp.Modularity;

namespace FogLightTask;

[DependsOn(
    typeof(FogLightTaskApplicationModule),
    typeof(FogLightTaskDomainTestModule)
)]
public class FogLightTaskApplicationTestModule : AbpModule
{

}

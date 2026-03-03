using Volo.Abp.Modularity;

namespace FogLightTask;

public abstract class FogLightTaskApplicationTestBase<TStartupModule> : FogLightTaskTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

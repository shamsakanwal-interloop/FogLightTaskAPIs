using Volo.Abp.Modularity;

namespace FogLightTask;

/* Inherit from this class for your domain layer tests. */
public abstract class FogLightTaskDomainTestBase<TStartupModule> : FogLightTaskTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

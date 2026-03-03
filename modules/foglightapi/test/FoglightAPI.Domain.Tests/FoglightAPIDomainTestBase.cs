using Volo.Abp.Modularity;

namespace FoglightAPI;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class FoglightAPIDomainTestBase<TStartupModule> : FoglightAPITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

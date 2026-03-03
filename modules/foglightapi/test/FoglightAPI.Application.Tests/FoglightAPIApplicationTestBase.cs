using Volo.Abp.Modularity;

namespace FoglightAPI;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class FoglightAPIApplicationTestBase<TStartupModule> : FoglightAPITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

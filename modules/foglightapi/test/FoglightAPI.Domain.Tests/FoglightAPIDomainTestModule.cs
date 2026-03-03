using Volo.Abp.Modularity;

namespace FoglightAPI;

[DependsOn(
    typeof(FoglightAPIDomainModule),
    typeof(FoglightAPITestBaseModule)
)]
public class FoglightAPIDomainTestModule : AbpModule
{

}

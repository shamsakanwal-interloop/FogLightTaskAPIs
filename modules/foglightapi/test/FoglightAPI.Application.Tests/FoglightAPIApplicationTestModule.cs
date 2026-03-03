using Volo.Abp.Modularity;

namespace FoglightAPI;

[DependsOn(
    typeof(FoglightAPIApplicationModule),
    typeof(FoglightAPIDomainTestModule)
    )]
public class FoglightAPIApplicationTestModule : AbpModule
{

}

using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace FoglightAPI;

[DependsOn(
    typeof(FoglightAPIDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class FoglightAPIApplicationContractsModule : AbpModule
{

}

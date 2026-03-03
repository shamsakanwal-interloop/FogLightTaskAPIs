using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace FoglightAPI;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(FoglightAPIDomainSharedModule)
)]
public class FoglightAPIDomainModule : AbpModule
{

}

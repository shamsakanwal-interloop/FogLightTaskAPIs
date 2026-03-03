using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace FoglightAPI.EntityFrameworkCore;

[DependsOn(
    typeof(FoglightAPIDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class FoglightAPIEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<FoglightAPIDbContext>(options =>
        {
            options.AddDefaultRepositories<IFoglightAPIDbContext>(includeAllEntities: true);
            
            /* Add custom repositories here. Example:
            * options.AddRepository<Question, EfCoreQuestionRepository>();
            */
        });
    }
}

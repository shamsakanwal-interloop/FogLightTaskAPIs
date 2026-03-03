using Localization.Resources.AbpUi;
using FoglightAPI.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace FoglightAPI;

[DependsOn(
    typeof(FoglightAPIApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class FoglightAPIHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(FoglightAPIHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<FoglightAPIResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}

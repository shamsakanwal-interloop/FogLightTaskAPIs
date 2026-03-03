using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace FoglightAPI;

[DependsOn(
    typeof(FoglightAPIApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class FoglightAPIHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(FoglightAPIApplicationContractsModule).Assembly,
            FoglightAPIRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FoglightAPIHttpApiClientModule>();
        });

    }
}

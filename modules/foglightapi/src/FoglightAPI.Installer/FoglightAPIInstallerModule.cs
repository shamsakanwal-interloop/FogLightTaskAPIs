using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace FoglightAPI;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class FoglightAPIInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FoglightAPIInstallerModule>();
        });
    }
}

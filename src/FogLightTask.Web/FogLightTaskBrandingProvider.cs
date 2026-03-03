using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using FogLightTask.Localization;

namespace FogLightTask.Web;

[Dependency(ReplaceServices = true)]
public class FogLightTaskBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FogLightTaskResource> _localizer;

    public FogLightTaskBrandingProvider(IStringLocalizer<FogLightTaskResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}

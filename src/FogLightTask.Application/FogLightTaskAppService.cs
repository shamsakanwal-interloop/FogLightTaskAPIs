using FogLightTask.Localization;
using Volo.Abp.Application.Services;

namespace FogLightTask;

/* Inherit your application services from this class.
 */
public abstract class FogLightTaskAppService : ApplicationService
{
    protected FogLightTaskAppService()
    {
        LocalizationResource = typeof(FogLightTaskResource);
    }
}

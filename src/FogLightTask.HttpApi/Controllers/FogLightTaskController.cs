using FogLightTask.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FogLightTask.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FogLightTaskController : AbpControllerBase
{
    protected FogLightTaskController()
    {
        LocalizationResource = typeof(FogLightTaskResource);
    }
}

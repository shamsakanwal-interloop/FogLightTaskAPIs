using FogLightTask.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FogLightTask.Web.Pages;

public abstract class FogLightTaskPageModel : AbpPageModel
{
    protected FogLightTaskPageModel()
    {
        LocalizationResourceType = typeof(FogLightTaskResource);
    }
}

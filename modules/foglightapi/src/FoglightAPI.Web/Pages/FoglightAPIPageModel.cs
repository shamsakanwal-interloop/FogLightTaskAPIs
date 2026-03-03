using FoglightAPI.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FoglightAPI.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class FoglightAPIPageModel : AbpPageModel
{
    protected FoglightAPIPageModel()
    {
        LocalizationResourceType = typeof(FoglightAPIResource);
        ObjectMapperContext = typeof(FoglightAPIWebModule);
    }
}

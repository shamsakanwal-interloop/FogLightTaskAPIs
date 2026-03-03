using FoglightAPI.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FoglightAPI;

public abstract class FoglightAPIController : AbpControllerBase
{
    protected FoglightAPIController()
    {
        LocalizationResource = typeof(FoglightAPIResource);
    }
}

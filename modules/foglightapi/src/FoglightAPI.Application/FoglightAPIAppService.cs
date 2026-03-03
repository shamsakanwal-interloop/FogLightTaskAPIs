using FoglightAPI.Localization;
using Volo.Abp.Application.Services;

namespace FoglightAPI;

public abstract class FoglightAPIAppService : ApplicationService
{
    protected FoglightAPIAppService()
    {
        LocalizationResource = typeof(FoglightAPIResource);
        ObjectMapperContext = typeof(FoglightAPIApplicationModule);
    }
}

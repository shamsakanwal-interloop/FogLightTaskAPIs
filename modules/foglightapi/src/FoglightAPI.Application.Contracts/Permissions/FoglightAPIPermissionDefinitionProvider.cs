using FoglightAPI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FoglightAPI.Permissions;

public class FoglightAPIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FoglightAPIPermissions.GroupName, L("Permission:FoglightAPI"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FoglightAPIResource>(name);
    }
}

using FogLightTask.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace FogLightTask.Permissions;

public class FogLightTaskPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FogLightTaskPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(FogLightTaskPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FogLightTaskResource>(name);
    }
}

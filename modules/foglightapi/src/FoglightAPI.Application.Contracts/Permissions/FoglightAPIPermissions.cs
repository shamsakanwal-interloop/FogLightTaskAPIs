using Volo.Abp.Reflection;

namespace FoglightAPI.Permissions;

public class FoglightAPIPermissions
{
    public const string GroupName = "FoglightAPI";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FoglightAPIPermissions));
    }
}

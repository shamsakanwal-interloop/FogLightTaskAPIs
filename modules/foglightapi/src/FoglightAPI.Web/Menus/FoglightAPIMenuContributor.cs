using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace FoglightAPI.Web.Menus;

public class FoglightAPIMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(FoglightAPIMenus.Prefix, displayName: "FoglightAPI", "~/FoglightAPI", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}

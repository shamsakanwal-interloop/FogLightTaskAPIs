using Volo.Abp.Settings;

namespace FogLightTask.Settings;

public class FogLightTaskSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FogLightTaskSettings.MySetting1));
    }
}

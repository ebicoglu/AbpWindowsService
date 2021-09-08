using Volo.Abp.Settings;

namespace AbpWindowsService.Settings
{
    public class AbpWindowsServiceSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AbpWindowsServiceSettings.MySetting1));
        }
    }
}

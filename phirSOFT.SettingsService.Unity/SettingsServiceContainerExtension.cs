using Unity.Extension;
using Unity.Policy;

namespace phirSOFT.SettingsService.Unity
{
    internal class SettingsServiceContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(new SettingsServiceConstructorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(new SettingsServicePropertyPolicy());
        }
    }
}
using Unity.Extension;
using Unity.Policy;

namespace phirSOFT.SettingsService.Unity
{
    /// <summary>
    ///     Provides a container extension, that will enable resolve strategies for settings.
    ///     Add this extension to your container to make the <see cref="SettingValueAttribute" /> work.
    /// </summary>
    public class SettingsServiceContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(new SettingsServiceConstructorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(new SettingsServicePropertyPolicy());
        }
    }
}
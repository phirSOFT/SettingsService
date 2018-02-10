using CommonServiceLocator;
using phirSOFT.SettingsService.Prism.Unity.Views;
using Prism.Modularity;
using Unity;

namespace phirSOFT.SettingsService.Prism.Unity
{
    public class SettingsModule : IModule
    {
        public void Initialize()
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            container.RegisterType(typeof(object), typeof(SettingsBrowser), typeof(SettingsBrowser).Name);
        }
    }
}
using CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using phirSOFT.SettingsService.Prism.Unity.Views;
using Prism.Ioc;
using static Prism.Ioc.IContainerProviderExtensions;
using Prism.Modularity;
using Prism.Unity;
using Unity;
using static Prism.Unity.UnityExtensions;
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
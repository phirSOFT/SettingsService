using System;
using Nito.AsyncEx;
using Unity;
using Unity.Builder;
using Unity.Policy;

namespace phirSOFT.SettingsService.Unity
{
    internal class SettingsValueResover : IResolverPolicy
    {
        private readonly string _serviceInstance;
        private readonly string _settingsKey;
        private readonly Type _settingsType;

        public SettingsValueResover(string serviceInstance, string settingsKey, Type settingsType)
        {
            _serviceInstance = serviceInstance;
            _settingsKey = settingsKey;
            _settingsType = settingsType;
        }

        public object Resolve(IBuilderContext context)
        {
            var service = context.Container.Resolve<IReadOnlySettingsService>(_serviceInstance);
            return AsyncContext.Run(() => service.GetSettingAsync(_settingsKey, _settingsType));
        }
    }
}
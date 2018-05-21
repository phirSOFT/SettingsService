using System;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
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
            Task<object> resultTask = service.GetSettingAsync(_settingsKey, _settingsType);
            resultTask.WaitAndUnwrapException();
            return resultTask.Result;
        }
    }
}
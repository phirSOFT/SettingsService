using System.Linq;
using System.Reflection;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.Policy;

namespace phirSOFT.SettingsService.Unity
{
    class SettingsServiceConstructorPolicy : DefaultUnityConstructorSelectorPolicy
    {
        protected override IResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            SettingValueAttribute settingsAttribute = parameter.GetCustomAttributes(inherit: false).OfType<SettingValueAttribute>().FirstOrDefault();
            return settingsAttribute != null ? new SettingsValueResover(settingsAttribute.ServiceInstance, settingsAttribute.SettingKey, settingsAttribute.SettingType ?? parameter.ParameterType) : base.CreateResolver(parameter);
        }
    }
}
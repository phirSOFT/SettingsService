using System.Linq;
using System.Reflection;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;

namespace phirSOFT.SettingsService.Unity
{
    internal class SettingsServicePropertyPolicy : DefaultUnityPropertySelectorPolicy
    {
        protected override IResolverPolicy CreateResolver(PropertyInfo parameter)
        {
            SettingValueAttribute settingsAttribute = parameter.GetCustomAttributes(inherit: true)
                .OfType<SettingValueAttribute>().FirstOrDefault();
            return settingsAttribute != null
                ? new SettingsValueResover(settingsAttribute.ServiceInstance, settingsAttribute.SettingKey,
                    settingsAttribute.SettingType ?? parameter.PropertyType)
                : base.CreateResolver(parameter);
        }
    }
}
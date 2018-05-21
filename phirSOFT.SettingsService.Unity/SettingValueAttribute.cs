using System;

namespace phirSOFT.SettingsService.Unity
{
    /// <inheritdoc />
    /// <summary>
    ///     Use this attribute, to annotate, that the property or parameter is resolved from a unity container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class SettingValueAttribute : Attribute
    {
        public SettingValueAttribute(string settingKey, string serviceInstance = "", Type settingType = null)
        {
            SettingKey = settingKey;
            ServiceInstance = serviceInstance;
            SettingType = settingType;
        }

        public string ServiceInstance { get; }
        public string SettingKey { get; }
        public Type SettingType { get; }
    }
}
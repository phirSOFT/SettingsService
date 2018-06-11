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
        /// <inheritdoc />
        /// <summary>
        ///     Marks a property or a parameter in a way, that the unity container will resolve the value,
        ///     using a settings service.
        /// </summary>
        /// <param name="settingKey">The key of the setting</param>
        /// <param name="serviceInstance">The name of the settings service instance to use.</param>
        /// <param name="settingType">The type of the setting to resolve. If null the field type will be used.</param>
        public SettingValueAttribute(string settingKey, string serviceInstance = "", Type settingType = null)
        {
            SettingKey = settingKey;
            ServiceInstance = serviceInstance;
            SettingType = settingType;
        }

        /// <summary>
        ///     Gets the name of the settings service instance name to be used.
        /// </summary>
        public string ServiceInstance { get; }

        /// <summary>
        ///     Gets the key of the setting to be resolved.
        /// </summary>
        public string SettingKey { get; }

        /// <summary>
        ///     Gets the type of the setting to be resolved.
        /// </summary>
        public Type SettingType { get; }
    }
}
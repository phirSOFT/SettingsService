using System;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides the basic interface of a settings service to retrive and store settings.
    ///     All settings can be changed virtually and are only stored persistent, if <see cref="Store" /> is called.
    /// </summary>
    /// <remarks>
    ///     Though it might not required, most implementations will only accept serializable settings type.
    /// </remarks>
    public interface ISettingsService
    {
        /// <summary>
        ///     Gets the value of setting with a specific key.
        /// </summary>
        /// <typeparam name="T">The type of the setting to retrieve</typeparam>
        /// <param name="key">The key of the setting</param>
        /// <returns>The value of the setting, if its present in this service</returns>
        Task<T> GetSettingAsync<T>(string key);

        /// <summary>
        ///     Gets the value of setting with a specific key.
        /// </summary>
        /// <param name="key">The key of the setting</param>
        /// <param name="type">The type of the setting to retrieve</param>
        /// <returns>The value of the setting, if its present in this service</returns>
        Task<object> GetSettingAsync(string key, Type type);

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value to set.</param>
        Task SetSettingAsync<T>(string key, T value);

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <param name="key">The key of the setting</param>
        /// <param name="value">The value of the setting</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        Task SetSettingAsync(string key, object value);

        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">he value of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        Task SetSettingAsync(string key, object value, Type type);

        /// <summary>
        ///     Registers a setting in this service
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        /// <param name="key">The key of the setting</param>
        Task RegisterSettingAsync<T>(string key);

        /// <summary>
        ///     Registers a setting in this service
        /// </summary>
        /// <param name="key">The key of the setting</param>
        /// <param name="type">The type of the setting</param>
        Task RegisterSettingAsync(string key, Type type);

        /// <summary>
        ///     Registers a setting with a default value in this service .
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The value of this setting</param>
        Task RegisterSettingAsync<T>(string key, T defaultValue);

        /// <summary>
        ///     Registers a setting with a default value in this service
        /// </summary>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The default value of this setting</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        Task RegisterSettingAsync(string key, object defaultValue);

        /// <summary>
        ///     Registers a setting with a default value in this service
        /// </summary>
        /// <param name="key">The key of this setting</param>
        /// <param name="defaultValue">The default value of this setting</param>
        /// <param name="type">The type of the setting.</param>
        Task RegisterSettingAsync(string key, object defaultValue, Type type);

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        Task RegisterSettingAsync<T>(string key, T defaultValue, T initialValue);

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        /// <remarks>
        ///     The settings type will be determined automatically. This can lead to unexpected results.
        /// </remarks>
        Task RegisterSettingAsync(string key, object defaultValue, object initialValue);

        /// <summary>
        ///     Registers a setting with a default and an initial value in this service.
        /// </summary>
        /// <param name="key">The key of this setting.</param>
        /// <param name="defaultValue">The default value of this setting.</param>
        /// <param name="initialValue">The initial value of this setting.</param>
        /// <param name="type">The type of the setting.</param>
        Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type);

        /// <summary>
        ///     Unregisters a setting from this service.
        /// </summary>
        /// <param name="key">The key of the setting to unregister</param>
        Task UnregisterSettingAsync(string key);

        /// <summary>
        ///     Determines wheter a setting is present in this service.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>True, if the setting can be retrieved, false if not.</returns>
        Task<bool> IsRegisterdAsync(string key);

        /// <summary>
        ///     Stores all settings to disc.
        /// </summary>
        Task StoreAsync();

        /// <summary>
        ///     Discards all changes since the last <see cref="Store" /> call.
        /// </summary>
        Task DiscardAsync();
    }
}
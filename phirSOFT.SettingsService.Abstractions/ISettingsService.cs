using System;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides the basic interface of a settings service to retrive and store settings.
    ///     All settings can be changed virtually and are only stored persistent, if
    ///     <see cref="M:phirSOFT.SettingsService.Abstractions.ISettingsService.StoreAsync" /> is called.
    /// </summary>
    /// <remarks>
    ///     Though it might not required, most implementations will only accept serializable settings type.
    /// </remarks>
    public interface ISettingsService : IReadOnlySettingsService
    {
        /// <summary>
        ///     Sets a setting to a new value.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">he value of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        Task SetSettingAsync(string key, object value, Type type);

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
        ///     Stores all settings to disc.
        /// </summary>
        Task StoreAsync();

        /// <summary>
        ///     Discards all changes since the last <see cref="StoreAsync" /> call.
        /// </summary>
        Task DiscardAsync();
    }
}
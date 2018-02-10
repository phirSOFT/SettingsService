using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    /// <summary>
    /// Provides an interface for readonly settings.
    /// </summary>
    public interface IReadOnlySettingsService
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
        ///     Determines wheter a setting is present in this service.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>True, if the setting can be retrieved, false if not.</returns>
        Task<bool> IsRegisterdAsync(string key);

    }
}

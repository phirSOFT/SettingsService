using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc cref="ISettingsService" />
    /// <summary>
    ///     Provides a settings service that retrives settings from multiple sources.
    /// </summary>
    public class SettingsStack : Collection<IReadOnlySettingsService>, ISettingsService
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a instance aof a <see cref="T:phirSOFT.SettingsService.SettingsStack" />, with a given set of
        ///     <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService" />
        ///     s.
        /// </summary>
        /// <param name="settingsServices">The initial set of <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService" />s.</param>
        public SettingsStack(IEnumerable<IReadOnlySettingsService> settingsServices)
        {
            foreach (var readOnlySettingsService in settingsServices) Add(readOnlySettingsService);
        }

        /// <summary>
        ///     Creates a instance aof a <see cref="SettingsStack" />.
        /// </summary>
        public SettingsStack()
        {
        }

        /// <summary>
        ///     The settings service, that is used to write settings.
        /// </summary>
        public ISettingsService WritableService { get; set; }


        /// <inheritdoc />
        public async Task<object> GetSettingAsync(string key, Type type)
        {
            using (var enumerator = GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                        throw new KeyNotFoundException();
                } while (!await enumerator.Current.IsRegisterdAsync(key));

                return await enumerator.Current.GetSettingAsync(key, type);
            }
        }

        /// <inheritdoc />
        public Task SetSettingAsync(string key, object value, Type type)
        {
            return WritableService.SetSettingAsync(key, value, type);
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            return WritableService.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        /// <inheritdoc />
        public Task UnregisterSettingAsync(string key)
        {
            return WritableService.UnregisterSettingAsync(key);
        }

        /// <inheritdoc />
        public Task StoreAsync()
        {
            return WritableService.StoreAsync();
        }

        /// <inheritdoc />
        public Task DiscardAsync()
        {
            return WritableService.DiscardAsync();
        }

        /// <inheritdoc />
        public async Task<bool> IsRegisterdAsync(string key)
        {
            using (var enumerator = GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                        return false;
                } while (!await enumerator.Current.IsRegisterdAsync(key));

                return true;
            }
        }
    }
}
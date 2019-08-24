using System;
using System.Threading.Tasks;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc />
    public class ReadOnlySettingsService : IReadOnlySettingsService
    {
        private readonly IReadOnlySettingsService _service;

        /// <summary>
        ///     Wraps an <see cref="IReadOnlySettingsService" /> in an read only instance.
        /// </summary>
        /// <param name="service">The service to wrap</param>
        public ReadOnlySettingsService(IReadOnlySettingsService service)
        {
            _service = service;
        }

        /// <inheritdoc />
        public Task<object> GetSettingAsync(string key, Type type)
        {
            return _service.GetSettingAsync(key, type);
        }

        /// <inheritdoc />
        public Task<bool> IsRegisteredAsync(string key)
        {
            return _service.IsRegisteredAsync(key);
        }
    }
}
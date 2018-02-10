using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    public class ReadOnlySettingsService : IReadOnlySettingsService
    {
        private readonly IReadOnlySettingsService _service;

        public ReadOnlySettingsService(IReadOnlySettingsService service)
        {
            this._service = service;
        }

        public Task<T> GetSettingAsync<T>(string key)
        {
            return _service.GetSettingAsync<T>(key);
        }

        public Task<object> GetSettingAsync(string key, Type type)
        {
            return _service.GetSettingAsync(key, type);
        }

        public Task<bool> IsRegisterdAsync(string key)
        {
            return _service.IsRegisterdAsync(key);
        }
    }
}

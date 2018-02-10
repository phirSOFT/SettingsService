using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    public class SettingsStack : SettingsServiceBase, IList<IReadOnlySettingsService>
    {
        private readonly IList<IReadOnlySettingsService> _settingsServices;

        public SettingsStack(IEnumerable<IReadOnlySettingsService> settingsServices) : this()
        {
            foreach (var readOnlySettingsService in settingsServices)
            {
                _settingsServices.Add(readOnlySettingsService);
            }
        }

        public SettingsStack()
        {
            _settingsServices = new List<IReadOnlySettingsService>();
        }

        public ISettingsService WritableService { get; set; }

        public IEnumerator<IReadOnlySettingsService> GetEnumerator()
        {
            return _settingsServices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_settingsServices).GetEnumerator();
        }

        public void Add(IReadOnlySettingsService item)
        {
            _settingsServices.Add(item);
        }

        public void Clear()
        {
            _settingsServices.Clear();
        }

        public bool Contains(IReadOnlySettingsService item)
        {
            return _settingsServices.Contains(item);
        }

        public void CopyTo(IReadOnlySettingsService[] array, int arrayIndex)
        {
            _settingsServices.CopyTo(array, arrayIndex);
        }

        public bool Remove(IReadOnlySettingsService item)
        {
            return _settingsServices.Remove(item);
        }

        public int Count => _settingsServices.Count;

        public bool IsReadOnly => _settingsServices.IsReadOnly;

        public int IndexOf(IReadOnlySettingsService item)
        {
            return _settingsServices.IndexOf(item);
        }

        public void Insert(int index, IReadOnlySettingsService item)
        {
            _settingsServices.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _settingsServices.RemoveAt(index);
        }

        public IReadOnlySettingsService this[int index]
        {
            get => _settingsServices[index];
            set => _settingsServices[index] = value;
        }

        public override async Task<object> GetSettingAsync(string key, Type type)
        {
            using (var enumerator = _settingsServices.GetEnumerator())
            {

                do
                {
                    if (!enumerator.MoveNext())
                        throw new KeyNotFoundException();
                } while (!await enumerator.Current.IsRegisterdAsync(key));

                return await enumerator.Current.GetSettingAsync(key, type);
            }
        }

        public override Task SetSettingAsync(string key, object value, Type type)
        {
            return WritableService.SetSettingAsync(key, value, type);
        }

        public override Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            return WritableService.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        public override Task UnregisterSettingAsync(string key)
        {
            return WritableService.UnregisterSettingAsync(key);
        }

        public override Task StoreAsync()
        {
            return WritableService.StoreAsync();
        }

        public override Task DiscardAsync()
        {
            return WritableService.DiscardAsync();
        }

        public override async Task<bool> IsRegisterdAsync(string key)
        {
            using (var enumerator = _settingsServices.GetEnumerator())
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

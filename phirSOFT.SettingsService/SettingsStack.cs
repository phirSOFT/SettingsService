using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc cref="SettingsServiceBase" />
    /// <summary>
    ///     Provides a settings service that retrives settings from multiple sources.
    /// </summary>
    public class SettingsStack : SettingsServiceBase, IList<IReadOnlySettingsService>
    {
        private readonly IList<IReadOnlySettingsService> _settingsServices;

        /// <inheritdoc />
        /// <summary>
        ///     Creates a instance aof a <see cref="T:phirSOFT.SettingsService.SettingsStack" />, with a given set of
        ///     <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService" />
        ///     s.
        /// </summary>
        /// <param name="settingsServices">The initial set of <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService" />s.</param>
        public SettingsStack(IEnumerable<IReadOnlySettingsService> settingsServices) : this()
        {
            foreach (var readOnlySettingsService in settingsServices) _settingsServices.Add(readOnlySettingsService);
        }

        /// <summary>
        ///     Creates a instance aof a <see cref="SettingsStack" />.
        /// </summary>
        public SettingsStack()
        {
            _settingsServices = new List<IReadOnlySettingsService>();
        }

        /// <summary>
        ///     The settings service, that is used to write settings.
        /// </summary>
        public ISettingsService WritableService { get; set; }


        /// <inheritdoc />
        public IEnumerator<IReadOnlySettingsService> GetEnumerator()
        {
            return _settingsServices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _settingsServices).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(IReadOnlySettingsService item)
        {
            _settingsServices.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _settingsServices.Clear();
        }

        /// <inheritdoc />
        public bool Contains(IReadOnlySettingsService item)
        {
            return _settingsServices.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(IReadOnlySettingsService[] array, int arrayIndex)
        {
            _settingsServices.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(IReadOnlySettingsService item)
        {
            return _settingsServices.Remove(item);
        }

        /// <inheritdoc />
        public int Count => _settingsServices.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _settingsServices.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(IReadOnlySettingsService item)
        {
            return _settingsServices.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, IReadOnlySettingsService item)
        {
            _settingsServices.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            _settingsServices.RemoveAt(index);
        }

        /// <inheritdoc />
        public IReadOnlySettingsService this[int index]
        {
            get => _settingsServices[index];
            set => _settingsServices[index] = value;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override Task SetSettingAsync(string key, object value, Type type)
        {
            return WritableService.SetSettingAsync(key, value, type);
        }

        /// <inheritdoc />
        public override Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            return WritableService.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        /// <inheritdoc />
        public override Task UnregisterSettingAsync(string key)
        {
            return WritableService.UnregisterSettingAsync(key);
        }

        /// <inheritdoc />
        public override Task StoreAsync()
        {
            return WritableService.StoreAsync();
        }

        /// <inheritdoc />
        public override Task DiscardAsync()
        {
            return WritableService.DiscardAsync();
        }

        /// <inheritdoc />
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
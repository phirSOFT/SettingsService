using System;
using System.Reflection;
using System.Threading.Tasks;
using static phirSOFT.SettingsService.TypeHelper;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc />
    /// <summary>
    ///     Implements a settings service that merges the overloads to the most parameter taking overload.
    /// </summary>
    public abstract class SettingsServiceBase : ISettingsService
    {
        /// <inheritdoc />
        public async Task<T> GetSettingAsync<T>(string key)
        {
            return (T) await GetSettingAsync(key, typeof(T));
        }

        /// <inheritdoc />
        public abstract Task<object> GetSettingAsync(string key, Type type);

        /// <inheritdoc />
        public Task SetSettingAsync<T>(string key, T value)
        {
            return SetSettingAsync(key, value, typeof(T));
        }

        /// <inheritdoc />
        public Task SetSettingAsync(string key, object value)
        {
            return SetSettingAsync(key, value, value.GetType());
        }

        /// <inheritdoc />
        public abstract Task SetSettingAsync(string key, object value, Type type);

        /// <inheritdoc />
        public Task RegisterSettingAsync<T>(string key)
        {
            var type = typeof(T);
            var defaultValue = GetDefaultValue(type);

            return RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync(string key, Type type)
        {
            var defaultValue = GetDefaultValue(type);

            return RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync<T>(string key, T defaultValue)
        {
            return RegisterSettingAsync(key, defaultValue, defaultValue, typeof(T));
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync(string key, object defaultValue)
        {
            return RegisterSettingAsync(key, defaultValue, defaultValue, defaultValue.GetType());
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync(string key, object defaultValue, Type type)
        {
            return RegisterSettingAsync(key, defaultValue, defaultValue, type);
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync<T>(string key, T defaultValue, T initialValue)
        {
            return RegisterSettingAsync(key, defaultValue, initialValue, typeof(T));
        }

        /// <inheritdoc />
        public Task RegisterSettingAsync(string key, object defaultValue, object initialValue)
        {
            var defaultType = defaultValue.GetType().GetTypeInfo();
            var initialType = initialValue.GetType().GetTypeInfo();

            if (!AreAssignable(defaultType, initialType, out var type) &&
                // The types are not derived from each other, so we have to find a common base type
                // We doen't have to check for a common interface, because we can't deserialize an interface.
                // Maybe we can allow this for collection like interfaces, but that is a topic to cover later.
                !HaveCommonBaseType(initialType, defaultType, out type))
                throw new ArgumentException("defaultValue and intitialValue does not share a common base type");

            return RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        /// <inheritdoc />
        public abstract Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type);

        /// <inheritdoc />
        public abstract Task UnregisterSettingAsync(string key);

        /// <inheritdoc />
        public abstract Task<bool> IsRegisterdAsync(string key);

        /// <inheritdoc />
        public abstract Task StoreAsync();

        /// <inheritdoc />
        public abstract Task DiscardAsync();
    }
}
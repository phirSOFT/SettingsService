using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace phirSOFT.SettingsService
{
    using CacheEntry = AsyncLazy<(object value, Type type)>;

    /// <inheritdoc />
    /// <summary>
    ///     Implements a settings service, that will minimize the calls to the
    ///     inheriting settings service. All calls are managed thread safe.
    /// </summary>
    public abstract class CachedSettingsService : SettingsServiceBase
    {
        private readonly SortedSet<string> _changedKeys = new SortedSet<string>();
        private readonly SortedSet<string> _deletedKeys = new SortedSet<string>();

        private readonly ConcurrentDictionary<string, (object, Type)> _insertedKeys =
            new ConcurrentDictionary<string, (object, Type)>();

        private readonly AsyncReaderWriterLock _readerWriterLock = new AsyncReaderWriterLock();

        private readonly ConcurrentDictionary<string, CacheEntry> _valuesCache =
            new ConcurrentDictionary<string, CacheEntry>();

        /// <summary>
        ///     Gets wheter two settings can be unregistred at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentUnregister { get; }

        /// <summary>
        ///     Gets wheter two settings can be updateted at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentUpdate { get; }

        /// <summary>
        ///     Geths wheter to settings can be registred at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentRegister { get; }

        /// <inheritdoc />
        public override async Task<object> GetSettingAsync(string key, Type type)
        {
            using (var _ = await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                var cacheEntry = await _valuesCache.GetOrAdd(key,
                    internalKey => new CacheEntry(() => ConstructCacheEntry(internalKey, type)));

                return cacheEntry.value;
            }
        }

        private async Task<(object, Type)> ConstructCacheEntry(string key, Type type)
        {
            var value = await GetSettingInternalAsync(key, type).ConfigureAwait(false);
            return (value, type);
        }

        /// <summary>
        ///     Retrives the value of a setting from the actual settings service.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="type">The type of the setting</param>
        /// <returns>The setting with the given key.</returns>
        protected abstract Task<object> GetSettingInternalAsync(string key, Type type);


        /// <inheritdoc />
        public override async Task SetSettingAsync(string key, object value, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                var entry = new CacheEntry(() => Task.FromResult((value, type)));

#pragma warning disable 4014
                // we dont have to await this. The only thing to await is the entry itselves.
                _valuesCache.AddOrUpdate(key, entry, (__, ___) => entry);
#pragma warning restore 4014

                if (!_insertedKeys.ContainsKey(key))
                    _changedKeys.Add(key);
            }
        }

        /// <inheritdoc />
        public override async Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _deletedKeys.Remove(key);
                _insertedKeys.AddOrUpdate(key, (defaultValue, type), (_, __) => (defaultValue, type));
                await SetSettingAsync(key, initialValue, type);
            }
        }

        /// <inheritdoc />
        public override async Task UnregisterSettingAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _deletedKeys.Add(key);
                if (!_valuesCache.TryRemove(key, out _))
                    return;

                _changedKeys.Remove(key);
                _insertedKeys.TryRemove(key, out _);
            }
        }

        /// <inheritdoc />
        public override async Task<bool> IsRegisterdAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                return _valuesCache.ContainsKey(key) || await IsRegisterdInternalAsync(key);
            }
        }

        /// <inheritdoc />
        public override async Task StoreAsync()
        {
            using (await _readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                var runnningTasks = new List<Task>();
                foreach (var deletedKey in _deletedKeys)
                {
                    var task = UnregisterSettingInternalAsync(deletedKey);
                    if (SupportConcurrentUnregister)
                        runnningTasks.Add(task);
                    else
                        await task.ConfigureAwait(false);
                }

                _deletedKeys.Clear();

                if (SupportConcurrentUnregister)
                    await Task.WhenAll(runnningTasks).ConfigureAwait(false);

                runnningTasks.Clear();

                foreach (var key in _insertedKeys)
                {
                    var initialValue = _valuesCache[key.Key];
                    var task = RegisterSettingInternalAsync(key.Key, key.Value.Item1, (await initialValue).value,
                        key.Value.Item2);

                    if (SupportConcurrentRegister)
                        runnningTasks.Add(task);
                    else
                        await task.ConfigureAwait(false);
                }

                _insertedKeys.Clear();

                if (SupportConcurrentRegister)
                    await Task.WhenAll(runnningTasks).ConfigureAwait(false);

                runnningTasks.Clear();

                foreach (var key in _changedKeys)
                {
                    if (!_valuesCache.TryGetValue(key, out var setting))
                        continue;

                    var (value, type) = await setting.ConfigureAwait(false);
                    var task = SetSettingInternalAsync(key, value, type);


                    if (SupportConcurrentUpdate)
                        runnningTasks.Add(task);
                    else
                        await task.ConfigureAwait(false);
                }

                if (SupportConcurrentUpdate)
                    await Task.WhenAll(runnningTasks).ConfigureAwait(false);

                await StoreInternalAsync();
            }
        }

        /// <inheritdoc />
        public override async Task DiscardAsync()
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                _insertedKeys.Clear();
                _changedKeys.Clear();
                _deletedKeys.Clear();
                _valuesCache.Clear();
            }
        }

        /// <summary>
        ///     Performs the actual store operation.
        /// </summary>
        /// <returns>A task that completes, when all settings are stored.</returns>
        protected abstract Task StoreInternalAsync();

        /// <summary>
        ///     Performs the actual settings registration.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="defaultValue">The default value of the key.</param>
        /// <param name="initialValue">The intial value of the key.</param>
        /// <param name="type">The type of the property.</param>
        /// <returns>A task, that finished when the new settings has been registred.</returns>
        protected abstract Task RegisterSettingInternalAsync(string key,
            object defaultValue, object initialValue, Type type);

        /// <summary>
        ///     Performs the actual unregister
        /// </summary>
        /// <param name="key">The key of the property to unregister</param>
        /// <returns>A task, that finished when the new settings has been unregistred.</returns>
        protected abstract Task UnregisterSettingInternalAsync(string key);

        /// <summary>
        ///     Gets wheter a setting with the given key in known to the actual settings service.
        /// </summary>
        /// <returns>A task containing the query result.</returns>
        protected abstract Task<bool> IsRegisterdInternalAsync(string key);

        /// <summary>
        ///     Performs the actual set operation.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The new value of the setting</param>
        /// <param name="type">The type of the setting.</param>
        /// <returns>A task that finished, when the value has been set.</returns>
        protected abstract Task SetSettingInternalAsync(string key, object value, Type type);
    }
}
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

        protected abstract bool SupportConcurrentUnregister { get; }
        protected abstract bool SupportConcurrentUpdate { get; }
        protected abstract bool SupportConcurrentRegister { get; }

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

        protected abstract Task<object> GetSettingInternalAsync(string key, Type type);

        public override async Task SetSettingAsync(string key, object value, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                var entry = new CacheEntry(() => Task.FromResult((value, type)));

#pragma warning disable 4014
                // we dont have to await this. The only thing to await is the enty itselves.
                _valuesCache.AddOrUpdate(key, entry, (__, ___) => entry);
#pragma warning restore 4014

                if (!_insertedKeys.ContainsKey(key))
                    _changedKeys.Add(key);
            }
        }

        public override async Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _deletedKeys.Remove(key);
                _insertedKeys.AddOrUpdate(key, (defaultValue, type), (_, __) => (defaultValue, type));
                await SetSettingAsync(key, initialValue, type);
            }
        }

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

        public override async Task<bool> IsRegisterdAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                return _valuesCache.ContainsKey(key) || await IsRegisterdInternalAsync();
            }
        }

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
                    var task = RegisterSettingInternalAsync(key, key.Value.Item1, (await initialValue).value,
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

        protected abstract Task StoreInternalAsync();

        protected abstract Task RegisterSettingInternalAsync(KeyValuePair<string, (object, Type)> key,
            object valueItem1, object initialValue, Type valueItem2);

        protected abstract Task UnregisterSettingInternalAsync(string key);

        protected abstract Task<bool> IsRegisterdInternalAsync();

        protected abstract Task SetSettingInternalAsync(string key, object value, Type type);
    }
}
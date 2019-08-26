// <copyright file="CachedSettingsService.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nito.AsyncEx;
using phirSOFT.SettingsService.Abstractions;
using CacheEntry = Nito.AsyncEx.AsyncLazy<(object value, System.Type type)>;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc/>
    /// <summary>
    ///     Implements a settings service, that will minimize the calls to the
    ///     inheriting settings service. All calls are managed thread safe.
    /// </summary>
    [PublicAPI]
    public abstract class CachedSettingsService : ISettingsService
    {
        private readonly SortedSet<string> _changedKeys = new SortedSet<string>();
        private readonly SortedSet<string> _deletedKeys = new SortedSet<string>();

        private readonly ConcurrentDictionary<string, (object, Type)> _insertedKeys =
            new ConcurrentDictionary<string, (object, Type)>();

        private readonly AsyncReaderWriterLock _readerWriterLock = new AsyncReaderWriterLock();

        private readonly ConcurrentDictionary<string, CacheEntry> _valuesCache =
            new ConcurrentDictionary<string, CacheEntry>();

        /// <summary>
        ///     Gets a value indicating whether settings can be registered at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentRegister { get; }

        /// <summary>
        ///     Gets a value indicating whether two settings can be unregistered at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentUnregister { get; }

        /// <summary>
        ///     Gets a value indicating whether two settings can be updated at the same time.
        /// </summary>
        protected abstract bool SupportConcurrentUpdate { get; }

        /// <inheritdoc/>
        public async Task<object> GetSettingAsync(string key, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                (object value, Type type) cacheEntry = await _valuesCache.GetOrAdd(
                    key,
                    internalKey => new CacheEntry(() => ConstructCacheEntry(internalKey, type)));

                return cacheEntry.value;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsRegisteredAsync(string key)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
                return _valuesCache.ContainsKey(key) || await IsRegisteredInternalAsync(key);
        }

        /// <inheritdoc/>
        public async Task DiscardAsync()
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                _insertedKeys.Clear();
                _changedKeys.Clear();
                _deletedKeys.Clear();
                _valuesCache.Clear();
            }
        }

        /// <inheritdoc/>
        public async Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _deletedKeys.Remove(key);
                _insertedKeys.AddOrUpdate(key, (defaultValue, type), (_, __) => (defaultValue, type));
                await SetSettingAsync(key, initialValue, type);
            }
        }

        /// <inheritdoc/>
        public async Task SetSettingAsync(string key, object value, Type type)
        {
            using (await _readerWriterLock.ReaderLockAsync().ConfigureAwait(false))
            {
                var entry = new CacheEntry(() => Task.FromResult((value, type)));

#pragma warning disable 4014, IDISP013, SA1313

                // we don't have to await this. The only thing to await is the entry itself.
                _valuesCache.AddOrUpdate(key, entry, (__, ___) => entry);
#pragma warning restore 4014, IDISP013, SA1313

                if (!_insertedKeys.ContainsKey(key))
                    _changedKeys.Add(key);
            }
        }

        /// <inheritdoc/>
        public async Task StoreAsync()
        {
            using (await _readerWriterLock.WriterLockAsync().ConfigureAwait(false))
            {
                var runnningTasks = new List<Task>();
                foreach (string deletedKey in _deletedKeys)
                {
                    Task task = UnregisterSettingInternalAsync(deletedKey);
                    if (SupportConcurrentUnregister)
                        runnningTasks.Add(task);
                    else
                        await task.ConfigureAwait(false);
                }

                _deletedKeys.Clear();

                if (SupportConcurrentUnregister)
                    await Task.WhenAll(runnningTasks).ConfigureAwait(false);

                runnningTasks.Clear();

                foreach (KeyValuePair<string, (object, Type)> key in _insertedKeys)
                {
                    CacheEntry initialValue = _valuesCache[key.Key];
                    Task task = RegisterSettingInternalAsync(
                        key.Key,
                        key.Value.Item1,
                        (await initialValue).value,
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

                foreach (string key in _changedKeys)
                {
                    if (!_valuesCache.TryGetValue(key, out CacheEntry setting))
                        continue;

                    (object value, Type type) = await setting.ConfigureAwait(false);
                    Task task = SetSettingInternalAsync(key, value, type);

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

        /// <inheritdoc/>
        public async Task UnregisterSettingAsync(string key)
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

        /// <summary>
        ///     Retrieves the value of a setting from the actual settings service.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <returns>The setting with the given key.</returns>
        [ItemCanBeNull]
        protected abstract Task<object> GetSettingInternalAsync([NotNull] string key, [NotNull] Type type);

        /// <summary>
        ///     Gets whether a setting with the given key in known to the actual settings service.
        /// </summary>
        /// <param name="key">The key of the setting, to determine, whether it is registered.</param>
        /// <returns>A task containing the query result.</returns>
        protected abstract Task<bool> IsRegisteredInternalAsync([NotNull] string key);

        /// <summary>
        ///     Performs the actual settings registration.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="defaultValue">The default value of the key.</param>
        /// <param name="initialValue">The intial value of the key.</param>
        /// <param name="type">The type of the property.</param>
        /// <returns>A task, that finished when the new settings has been registred.</returns>
        protected abstract Task RegisterSettingInternalAsync(
            [NotNull] string key,
            [CanBeNull] object defaultValue,
            [CanBeNull] object initialValue,
            [NotNull] Type type);

        /// <summary>
        ///     Performs the actual set operation.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The new value of the setting.</param>
        /// <param name="type">The type of the setting.</param>
        /// <returns>A task that finished, when the value has been set.</returns>
        protected abstract Task SetSettingInternalAsync(
            [NotNull] string key,
            [CanBeNull] object value,
            [NotNull] Type type);

        /// <summary>
        ///     Performs the actual store operation.
        /// </summary>
        /// <returns>A task that completes, when all settings are stored.</returns>
        protected abstract Task StoreInternalAsync();

        /// <summary>
        ///     Performs the actual unregister.
        /// </summary>
        /// <param name="key">The key of the property to unregister.</param>
        /// <returns>A task, that finished when the new settings has been unregistred.</returns>
        protected abstract Task UnregisterSettingInternalAsync([NotNull] string key);

        private async Task<(object, Type)> ConstructCacheEntry([NotNull] string key, [NotNull] Type type)
        {
            object value = await GetSettingInternalAsync(key, type).ConfigureAwait(false);
            return (value, type);
        }
    }
}

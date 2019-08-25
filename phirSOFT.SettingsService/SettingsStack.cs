// <copyright file="SettingsStack.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc cref="ISettingsService"/>
    /// <summary>
    ///     Provides a settings service that retrives settings from multiple sources.
    /// </summary>
    public class SettingsStack : Collection<IReadOnlySettingsService>, ISettingsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsStack"/> class with a given set of
        ///     <see cref="IReadOnlySettingsService"/>s.
        /// </summary>
        /// <param name="settingsServices">The initial set of <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService"/>s.</param>
        public SettingsStack(IEnumerable<IReadOnlySettingsService> settingsServices)
        {
            foreach (IReadOnlySettingsService readOnlySettingsService in settingsServices)
            {
                Add(readOnlySettingsService);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsStack"/> class.
        /// </summary>
        public SettingsStack()
        {
        }

        /// <summary>
        ///     Gets or sets the <see cref="ISettingsService"/>, that is used to write settings.
        /// </summary>
        public ISettingsService WritableService { get; set; }

        /// <inheritdoc/>
        public async Task<object> GetSettingAsync(string key, Type type)
        {
            using (IEnumerator<IReadOnlySettingsService> enumerator = GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                    {
                        throw new KeyNotFoundException();
                    }
                }
                while (!await enumerator.Current.IsRegisteredAsync(key));

                return await enumerator.Current.GetSettingAsync(key, type);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsRegisteredAsync(string key)
        {
            using (IEnumerator<IReadOnlySettingsService> enumerator = GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }
                }
                while (!await enumerator.Current.IsRegisteredAsync(key));

                return true;
            }
        }

        /// <inheritdoc/>
        public Task DiscardAsync()
        {
            return WritableService.DiscardAsync();
        }

        /// <inheritdoc/>
        public Task RegisterSettingAsync(string key, object defaultValue, object initialValue, Type type)
        {
            return WritableService.RegisterSettingAsync(key, defaultValue, initialValue, type);
        }

        /// <inheritdoc/>
        public Task SetSettingAsync(string key, object value, Type type)
        {
            return WritableService.SetSettingAsync(key, value, type);
        }

        /// <inheritdoc/>
        public Task StoreAsync()
        {
            return WritableService.StoreAsync();
        }

        /// <inheritdoc/>
        public Task UnregisterSettingAsync(string key)
        {
            return WritableService.UnregisterSettingAsync(key);
        }
    }
}

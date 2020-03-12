// <copyright file="ReadOnlySettingsStack.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JetBrains.Annotations;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides a <see cref="IReadOnlySettingsService"/>, that retrieves it settings from multiple sources. The first
    ///     <see cref="IReadOnlySettingsService"/>, that contains the setting, will be used.
    /// </summary>
    [PublicAPI]
    public class ReadOnlySettingsStack : Collection<IReadOnlySettingsService>, IReadOnlySettingsService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlySettingsStack"/> class with a given set of
        ///     <see cref="IReadOnlySettingsService"/>s.
        /// </summary>
        /// <param name="settingsServices">The initial set of <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService"/>s.</param>
        public ReadOnlySettingsStack([NotNull] [ItemNotNull] IEnumerable<IReadOnlySettingsService> settingsServices)
        {
            if (settingsServices == null)
                throw new ArgumentNullException(nameof(settingsServices));

            foreach (IReadOnlySettingsService readOnlySettingsService in settingsServices)
                Add(readOnlySettingsService);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlySettingsStack"/> class.
        /// </summary>
        public ReadOnlySettingsStack()
        {
        }

        /// <inheritdoc/>
        public async Task<object> GetSettingAsync(string key, Type type)
        {
            IReadOnlySettingsService settingsService = await TryGetSettingService(key).ConfigureAwait(false);

            if (settingsService == null)
            {
                throw new KeyNotFoundException();
            }

            return await settingsService.GetSettingAsync(key, type).ConfigureAwait(false);

        }

        /// <inheritdoc/>
        public async Task<bool> IsRegisteredAsync(string key)
        {
            return await TryGetSettingService(key) != null;
        }

        /// <summary>
        ///     Tries to find the <see cref="IReadOnlySettingsService"/>, that has a <paramref name="key"/> registered.
        /// </summary>
        /// <param name="key">The key to find a settings service for.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation. The result of the <see cref="Task"/> will be a
        ///     <see cref="IReadOnlySettingsService"/>, that contains the requested key, or <see langword="null"/>, if not
        ///     <see cref="IReadOnlySettingsService"/> contained the <paramref name="key"/>.
        /// </returns>
        [ItemCanBeNull]
        protected async Task<IReadOnlySettingsService> TryGetSettingService(string key)
        {
            using (IEnumerator<IReadOnlySettingsService> enumerator = GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                        return null;
                }
                while (!((enumerator.Current != null) && await enumerator.Current.IsRegisteredAsync(key)));

                return enumerator.Current;
            }
        }
    }
}

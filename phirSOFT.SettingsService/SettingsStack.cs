// <copyright file="SettingsStack.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    /// <inheritdoc cref="ISettingsService"/>
    /// <summary>
    ///     Provides a settings service that retrives settings from multiple sources.
    /// </summary>
    [PublicAPI]
    public class SettingsStack : ReadOnlySettingsStack, ISettingsService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsStack"/> class with a given set of
        ///     <see cref="IReadOnlySettingsService"/>s.
        /// </summary>
        /// <param name="writableSettingsService">The <see cref="ISettingsService"/>, that changes should be written to.</param>
        /// <param name="settingsServices">The initial set of <see cref="T:phirSOFT.SettingsService.IReadOnlySettingsService"/>s.</param>
        public SettingsStack(
            [NotNull] ISettingsService writableSettingsService,
            [NotNull] [ItemNotNull] IEnumerable<IReadOnlySettingsService> settingsServices)
            : base(settingsServices)
        {
            WritableService = writableSettingsService;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsStack"/> class.
        /// </summary>
        /// <param name="writableSettingsService">The <see cref="ISettingsService"/>, that changes should be written to.</param>
        public SettingsStack([NotNull] ISettingsService writableSettingsService)
            : this(writableSettingsService, Enumerable.Empty<IReadOnlySettingsService>())
        {
        }

        /// <summary>
        ///     Gets or sets the <see cref="ISettingsService"/>, that is used to write settings.
        /// </summary>
        [NotNull]
        public ISettingsService WritableService { get; set; }

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

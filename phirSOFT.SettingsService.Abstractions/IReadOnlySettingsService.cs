// <copyright file="IReadOnlySettingsService.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    ///     Provides an interface for readonly settings.
    /// </summary>
    public interface IReadOnlySettingsService
    {
        /// <summary>
        ///     Gets the value of setting with a specific key.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="type">The type of the setting to retrieve.</param>
        /// <returns>The value of the setting, if its present in this service.</returns>
        Task<object> GetSettingAsync(string key, Type type);

        /// <summary>
        ///     Determines whether a setting is present in this service.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>True, if the setting can be retrieved, false if not.</returns>
        Task<bool> IsRegisteredAsync(string key);
    }
}

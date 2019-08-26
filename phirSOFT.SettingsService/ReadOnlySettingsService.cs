// <copyright file="ReadOnlySettingsService.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Wraps an <see cref="IReadOnlySettingsService"/> in a read only instance.
    /// </summary>
    public class ReadOnlySettingsService : IReadOnlySettingsService
    {
        private readonly IReadOnlySettingsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySettingsService"/> class.
        /// </summary>
        /// <param name="service">The service to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is <see langword="null"/>.</exception>
        public ReadOnlySettingsService([NotNull] IReadOnlySettingsService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc/>
        public Task<object> GetSettingAsync(string key, Type type)
        {
            return _service.GetSettingAsync(key, type);
        }

        /// <inheritdoc/>
        public Task<bool> IsRegisteredAsync(string key)
        {
            return _service.IsRegisteredAsync(key);
        }
    }
}

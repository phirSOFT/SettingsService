// <copyright file="ISettingMigration.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    /// Provides a definition how to migrate a set of settings.
    /// </summary>
    public interface ISettingMigration
    {
        /// <summary>
        ///     Migrates the setting set from an older version of the settings set.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> to migrate.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the migration. </param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task MigrateUpAsync(ISettingsService settingsService, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Removes the changes made by this <see cref="ISettingMigration"/> from a <see cref="ISettingsService"/>.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> to migrate.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the migration. </param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task MigrateDownAsync(ISettingsService settingsService, CancellationToken cancellationToken = default);
    }
}

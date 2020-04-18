// <copyright file="IMigrationRunner.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    ///     Provides a service, that can run multiple <see cref="ISettingMigration"/> on a <see cref="ISettingsService"/>.
    /// </summary>
    public interface IMigrationRunner
    {
        /// <summary>
        ///     Gets all <see cref="IMigrationRunner"/> of a collection, that are applied to a <see cref="ISettingsService"/>.
        /// </summary>
        /// <param name="settingsService">
        ///     The <see cref="IReadOnlySettingsService"/>, that should be inspected for the
        ///     <paramref name="migrations"/>.
        /// </param>
        /// <param name="migrations">The collection of <see cref="IMigrationRunner"/> to filter.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task<IReadOnlyCollection<IMigrationRunner>> FilterAppliedAsync(
            IReadOnlySettingsService settingsService,
            IEnumerable<IMigrationRunner> migrations,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Initializes a <see cref="ISettingsService"/> to be migrated by this <see cref="IMigrationRunner"/>.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> to be managed by this <see cref="IMigrationRunner"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task InitializeSettingsServiceAsync(
            ISettingsService settingsService,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Determines whether a <paramref name="migration"/> was applied to a <paramref name="settingsService"/>.
        /// </summary>
        /// <param name="settingsService">
        ///     The <see cref="IReadOnlySettingsService"/>, that should be inspected for the
        ///     <paramref name="migration"/>.
        /// </param>
        /// <param name="migration">The <see cref="ISettingMigration"/> to search for.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task<bool> IsAppliedAsync(
            IReadOnlySettingsService settingsService,
            ISettingMigration migration,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Applies all migrations to a <see cref="ISettingsService"/>, that are not already applied.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/>, that should be migrated.</param>
        /// <param name="migrations">The <see cref="ISettingMigration"/>s to apply.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task MigrateSettingsAsync(
            ISettingsService settingsService,
            IReadOnlyCollection<ISettingMigration> migrations,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Migrates the settings of a <see cref="ISettingsService"/> so that <paramref name="targetMigration"/> is the last
        ///     applied migration.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/>, that should be migrated.</param>
        /// <param name="migrations">The <see cref="ISettingMigration"/>s to apply.</param>
        /// <param name="targetMigration">
        ///     The migrations, that should be the last applied <see cref="ISettingMigration"/> of the
        ///     <paramref name="settingsService"/>.
        /// </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        /// <remarks>
        ///     <para>
        ///         This rollback some <see cref="ISettingMigration"/> from the <paramref name="settingsService"/>.
        ///     </para>
        /// </remarks>
        Task MigrateSettingsAsync(
            ISettingsService settingsService,
            IReadOnlyCollection<ISettingMigration> migrations,
            ISettingMigration targetMigration,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Removes all information stored by this <see cref="IMigrationRunner"/> from a <see cref="ISettingsService"/>.
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingMigration"/>, this runner should be removed from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task RemoveFromSettingsServiceAsync(
            ISettingsService settingsService,
            CancellationToken cancellationToken = default);
    }
}

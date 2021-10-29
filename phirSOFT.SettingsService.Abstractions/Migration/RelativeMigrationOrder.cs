// <copyright file="RelativeMigrationOrder.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    /// Determines in which order two <see cref="ISettingMigration"/> should be applied relative to each other.
    /// </summary>
    public enum RelativeMigrationOrder
    {
        /// <summary>
        /// The annotated <see cref="ISettingMigration"/> should be applied before the referenced.
        /// </summary>
        Before = -1,

        /// <summary>
        /// The annotated <see cref="ISettingMigration"/> should be applied after the referenced.
        /// </summary>
        After = 1,

        /// <summary>
        /// The annoated <see cref="ISettingMigration"/> can be applied independently of the referenced.
        /// </summary>
        Unrelated = 0,
    }
}

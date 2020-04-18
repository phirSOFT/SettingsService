// <copyright file="SettingSetAttribute.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    ///     Associates a setting set with a <see cref="ISettingMigration"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SettingMigrationAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingMigrationAttribute"/> class.
        /// </summary>
        /// <param name="migrationKey">A distinct identifier for the associated <see cref="ISettingMigration"/>.</param>
        /// <param name="settingSet">The name of the associated setting set.</param>
        public SettingMigrationAttribute(string migrationKey, string? settingSet = null)
        {
            MigrationKey = migrationKey;
            SettingSet = settingSet;
        }

        /// <summary>
        ///     Gets a value indicating whether the associated <see cref="ISettingMigration"/> should be applied.
        /// </summary>
        public bool Enabled { get; } = true;

        /// <summary>
        ///     Gets a distinct identifier for the associated <see cref="ISettingMigration"/>.
        /// </summary>
        public string MigrationKey { get; }

        /// <summary>
        ///     Gets the name of the associated setting set.
        /// </summary>
        public string? SettingSet { get; }
    }
}

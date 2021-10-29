// <copyright file="AbsoluteMigrationOrder.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reflection;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    ///     Sets an absolute order in which <see cref="ISettingMigration"/> should be applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AbsoluteMigrationOrderAttribute : MigrationOrderBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AbsoluteMigrationOrderAttribute"/> class.
        /// </summary>
        /// <param name="migrationOrder">The index, when this migration should be applied.</param>
        public AbsoluteMigrationOrderAttribute(long migrationOrder)
        {
            MigrationOrder = migrationOrder;
        }

        /// <summary>
        ///     Gets the 0 based index of the migration.
        /// </summary>
        public long MigrationOrder { get; }

        /// <inheritdoc />
        public override bool TryDetermineExecutionOrder(ISettingMigration otherMigration, out bool beforeOther)
        {
            if (otherMigration == null)
            {
                throw new ArgumentNullException(nameof(otherMigration));
            }

            if (otherMigration.GetType().GetTypeInfo().GetCustomAttribute<AbsoluteMigrationOrder>() is { }
                otherAbsoluteMigrationOrder)
            {
                beforeOther = MigrationOrder - otherAbsoluteMigrationOrder.MigrationOrder > 0;
                return true;
            }

            beforeOther = default;
            return false;
        }
    }
}

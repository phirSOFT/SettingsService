// <copyright file="RelativeMigrationOrderAttribute.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reflection;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    /// Defines a relative order in which to <see cref="ISettingMigration"/> should be applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RelativeMigrationOrderAttribute : MigrationOrderBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeMigrationOrderAttribute"/> class.
        /// </summary>
        /// <param name="relativeOrder">The order the annotated <see cref="ISettingMigration"/> is applied relative to <paramref name="otherMigration"/>.</param>
        /// <param name="otherMigration">A <see cref="ISettingMigration"/> that should be applied in relative order to the annotated <see cref="ISettingMigration"/>.</param>
        public RelativeMigrationOrderAttribute(RelativeMigrationOrder relativeOrder, Type otherMigration)
            : this(relativeOrder, otherMigration?.GetType().GetTypeInfo().GetCustomAttribute<SettingMigrationAttribute>().MigrationKey ?? throw new ArgumentNullException())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeMigrationOrderAttribute"/> class.
        /// </summary>
        /// <param name="relativeOrder">The order the annotated <see cref="ISettingMigration"/> is applied relative to <paramref name="otherMigrationKey"/>.</param>
        /// <param name="otherMigrationKey">A migration key of the other <see cref="ISettingMigration"/> that should be applied in relative order to the annotated <see cref="ISettingMigration"/>.</param>
        public RelativeMigrationOrderAttribute(RelativeMigrationOrder relativeOrder, string otherMigrationKey)
        {
            RelativeOrder = relativeOrder;
            OtherMigrationKey = otherMigrationKey;
        }

        /// <summary>
        /// Gets the type of a <see cref="ISettingMigration"/> that should be applied relative to the annotated <see cref="ISettingMigration"/>.
        /// </summary>
        public string OtherMigrationKey { get; }

        /// <summary>
        /// Gets the relative order between the annotated <see cref="ISettingMigration"/> and the migration with the <see cref="OtherMigrationKey"/>.
        /// </summary>
        public RelativeMigrationOrder RelativeOrder { get; }

        /// <inheritdoc />
        public override bool TryDetermineExecutionOrder(ISettingMigration otherMigration, out bool beforeOther)
        {
            string otherMigrationKey = otherMigration.GetType()
                .GetTypeInfo()
                .GetCustomAttribute<SettingMigrationAttribute>()
                .MigrationKey;

            if (StringComparer.Ordinal.Equals(OtherMigrationKey, otherMigrationKey))
            {
                beforeOther = RelativeOrder == RelativeMigrationOrder.Before;
                return true;
            }

            beforeOther = default;
            return false;
        }
    }
}

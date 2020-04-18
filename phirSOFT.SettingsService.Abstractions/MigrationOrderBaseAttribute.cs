// <copyright file="MigrationOrderBaseAttribute.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace phirSOFT.SettingsService.Abstractions
{
    /// <summary>
    /// Provides information, to order the execution of <see cref="ISettingMigration"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MigrationOrderBaseAttribute : Attribute
    {
        /// <summary>
        ///     Tries to determine the execution order of the annotated <see cref="ISettingsService"/> and an <paramref name="otherMigration"/>.
        /// </summary>
        /// <param name="otherMigration">The <see cref="IMigrationRunner"/> to determine the relative execution order to.</param>
        /// <param name="beforeOther">A value indicating, whether the annotated <see cref="ISettingMigration"/> should be executed before <paramref name="otherMigration"/>.</param>
        /// <returns>A value indicating whether an execution order could be determined.</returns>
        public abstract bool TryDetermineExecutionOrder(ISettingMigration otherMigration, out bool beforeOther);
    }
}

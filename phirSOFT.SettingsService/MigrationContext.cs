// <copyright file="MigrationContext.cs" company="phirSOFT">
// Copyright (c) phirSOFT. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using phirSOFT.SettingsService.Abstractions;
using phirSOFT.TopologicalComparison;

namespace phirSOFT.SettingsService
{
    /// <summary>
    ///     Provides context information of the ordering of <see cref="IMigrationRunner"/>s.
    /// </summary>
    public readonly struct MigrationContext : ITopologicalComparable<MigrationContext>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MigrationContext"/> struct.
        /// </summary>
        /// <param name="migrationRunner">The <see cref="MigrationRunner"/> the context applies to.</param>
        /// <param name="settingsSet">
        ///     The name of the migration set, the runner applies to or <see langword="null"/> if it applies
        ///     to the default context.
        /// </param>
        /// <param name="migrationOrder">
        ///     The absolute migration order of the <paramref name="migrationRunner"/> or
        ///     <see langword="null"/> if unknown.
        /// </param>
        /// <param name="beforeRunners">
        ///     A collection of <see cref="IMigrationRunner"/> that should be run after
        ///     <paramref name="migrationRunner"/>.
        /// </param>
        /// <param name="afterRunners">
        ///     A collection of <see cref="IMigrationRunner"/> that should be run before
        ///     <paramref name="migrationRunner"/>.
        /// </param>
        public MigrationContext(
            IMigrationRunner migrationRunner,
            string? settingsSet,
            long? migrationOrder,
            IReadOnlyCollection<Type> beforeRunners,
            IReadOnlyCollection<Type> afterRunners)
        {
            SettingsSet = settingsSet;
            MigrationRunner = migrationRunner;
            BeforeRunners = beforeRunners;
            AfterRunners = afterRunners;
            MigrationOrder = migrationOrder;
        }

        /// <summary>
        ///     Gets a collection of <see cref="IMigrationRunner"/>s, that should be executed before this runner.
        /// </summary>
        public IReadOnlyCollection<Type> AfterRunners { get; }

        /// <summary>
        ///     Gets a collection of <see cref="IMigrationRunner"/>s, that should be executed after this runner.
        /// </summary>
        public IReadOnlyCollection<Type> BeforeRunners { get; }

        /// <summary>
        ///     Gets the execution order of the <see cref="MigrationRunner"/> or <see langword="null"/>, if unknown.
        /// </summary>
        public long? MigrationOrder { get; }

        /// <summary>
        ///     Gets the <see cref="IMigrationRunner"/> this context was generated for.
        /// </summary>
        public IMigrationRunner MigrationRunner { get; }

        /// <summary>
        ///     Gets the name if the setting set, that is migrated by the <see cref="MigrationRunner"/> or <see langword="null"/>
        ///     for the default setting set.
        /// </summary>
        public string? SettingsSet { get; }

        /// <summary>
        ///     Creates a new <see cref="MigrationContext"/> for a <see cref="IMigrationRunner"/>.
        /// </summary>
        /// <param name="migrationRunner">The <see cref="IMigrationRunner"/> to create a <see cref="MigrationContext"/> for.</param>
        /// <returns>A <see cref="MigrationContext"/> for the <see cref="IMigrationRunner"/>.</returns>
        public static MigrationContext Create(IMigrationRunner migrationRunner)
        {
            TypeInfo typeInfo = migrationRunner.GetType().GetTypeInfo();
            string? settingSetName = typeInfo.GetCustomAttribute<SettingSetAttribute>()?.SettingSet;
            long? migrationOrder = typeInfo.GetCustomAttribute<AbsoluteMigrationOrder>()?.MigrationOrder;

            ImmutableHashSet<Type>.Builder afterBuilder = ImmutableHashSet.CreateBuilder<Type>();
            ImmutableHashSet<Type>.Builder beforeBuilder = ImmutableHashSet.CreateBuilder<Type>();

            foreach (RelativeMigrationOrderAttribute relativeMigrationOrderAttribute in typeInfo
                .GetCustomAttributes<RelativeMigrationOrderAttribute>())
            {
                switch (relativeMigrationOrderAttribute.RelativeOrder)
                {
                    case RelativeMigrationOrder.Before:
                        beforeBuilder.Add(relativeMigrationOrderAttribute.OtherMigration);
                        break;
                    case RelativeMigrationOrder.After:
                        afterBuilder.Add(relativeMigrationOrderAttribute.OtherMigration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            ImmutableHashSet<Type> beforeRunners = beforeBuilder.ToImmutable();
            ImmutableHashSet<Type> afterRunners = afterBuilder.ToImmutable();

            ImmutableHashSet<Type> illegalDependencies = beforeRunners.Intersect(afterRunners);
            if (illegalDependencies.Any())
            {
                throw new InvalidOperationException(
                    "Illegal dependency declaration. There are dependencies that should be run before and after the migration.")
                {
#pragma warning disable SA1012, SA1013 // Opening braces should be spaced correctly
                    Data = { { "invalidDependencies", illegalDependencies } },
#pragma warning restore SA1012, SA1013 // Opening braces should be spaced correctly
                };
            }

            return new MigrationContext(
                migrationRunner,
                settingSetName,
                migrationOrder,
                beforeRunners,
                afterRunners);
        }

        /// <inheritdoc/>
        public int CompareTo(MigrationContext other)
        {
            if (MigrationOrder.HasValue && other.MigrationOrder.HasValue)
            {
                return Math.Sign(MigrationOrder.Value - other.MigrationOrder.Value);
            }

            TypeInfo otherType = other.GetType().GetTypeInfo();

            if (BeforeRunners.Any(runner => CanAssign(runner, otherType)))
            {
                return -1;
            }

            if (AfterRunners.Any(runner => CanAssign(runner, otherType)))
            {
                return 1;
            }

            TypeInfo thisType = GetType().GetTypeInfo();
            if (other.BeforeRunners.Any(runner => CanAssign(runner, thisType)))
            {
                return 1;
            }

            if (other.AfterRunners.Any(runner => CanAssign(runner, thisType)))
            {
                return -1;
            }

            throw new InvalidOperationException("The two instances are not topological comparable.");
        }

        /// <inheritdoc/>
        public bool CanCompareTo(MigrationContext other)
        {
            if (!StringComparer.Ordinal.Equals(SettingsSet, other.SettingsSet))
            {
                return false;
            }

            if (MigrationOrder.HasValue && other.MigrationOrder.HasValue)
            {
                return true;
            }

            TypeInfo otherType = other.GetType().GetTypeInfo();
            TypeInfo thisType = GetType().GetTypeInfo();
            return BeforeRunners.Any(runner => CanAssign(runner, otherType)) ||
                   AfterRunners.Any(runner => CanAssign(runner, otherType)) ||
                   other.BeforeRunners.Any(runner => CanAssign(runner, thisType)) ||
                   other.AfterRunners.Any(runner => CanAssign(runner, thisType));
        }

        private static bool CanAssign(Type runnerType, TypeInfo otherTypeInfo)
        {
            return runnerType.GetTypeInfo().IsAssignableFrom(otherTypeInfo);
        }
    }
}

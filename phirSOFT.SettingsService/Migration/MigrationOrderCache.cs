using phirSOFT.SettingsService.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace phirSOFT.SettingsService
{
    internal class MigrationOrderCache
    {
        private static readonly object BeforeBox = RelativeMigrationOrder.Before;
        private static readonly object AfterBox = RelativeMigrationOrder.After;
        private static readonly object UnrelatedBox = RelativeMigrationOrder.Unrelated;

        private readonly ConditionalWeakTable<ISettingMigration, object> _migrationOrderCache = new ConditionalWeakTable<ISettingMigration, object>();

        public bool TryGetOrder(ISettingMigration otherRunner, out RelativeMigrationOrder relativeMigrationOrder)
        {
            relativeMigrationOrder = RelativeMigrationOrder.Unrelated;
            if (!_migrationOrderCache.TryGetValue(otherRunner, out object relationBox))
            {
                return false;
            }

            if (ReferenceEquals(relationBox, BeforeBox))
            {
                relativeMigrationOrder = RelativeMigrationOrder.Before;
            }
            else if (ReferenceEquals(relationBox, AfterBox))
            {
                relativeMigrationOrder = RelativeMigrationOrder.After;
            }

            return true;
        }

        public void SetOrder(ISettingMigration otherMigration, RelativeMigrationOrder relativeMigrationOrder)
        {
            object relativeMigrationOrderBox = relativeMigrationOrder switch
            {
                RelativeMigrationOrder.After => AfterBox,
                RelativeMigrationOrder.Before => BeforeBox,
                RelativeMigrationOrder.Unrelated => UnrelatedBox,
                _ => throw new ArgumentOutOfRangeException(nameof(relativeMigrationOrder))
            };

            _migrationOrderCache.Add(otherMigration, relativeMigrationOrderBox);
        }
    }
}

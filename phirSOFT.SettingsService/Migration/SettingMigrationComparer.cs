using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using phirSOFT.SettingsService.Abstractions;
using phirSOFT.TopologicalComparison;

namespace phirSOFT.SettingsService
{
    public class SettingMigrationComparer : ITopologicalComparer<ISettingMigration>
    {
        private static readonly ConditionalWeakTable<ISettingMigration, MigrationOrderCache> _relationCache = new ConditionalWeakTable<ISettingMigration, MigrationOrderCache>();

        public static bool TryGetRelation(ISettingMigration left, ISettingMigration right, out RelativeMigrationOrder leftToRight)
        {
            if (_relationCache.TryGetValue(left, out MigrationOrderCache migrationOrderCache) && migrationOrderCache.TryGetOrder(right, out leftToRight))
            {
                return true;
            }

            if (_relationCache.TryGetValue(right, out migrationOrderCache) && migrationOrderCache.TryGetOrder(left, out leftToRight))
            {
                leftToRight = FlipOrder(leftToRight);

                return true;
            }

            leftToRight = RelativeMigrationOrder.Unrelated;
            return false;
        }

        private static RelativeMigrationOrder FlipOrder(RelativeMigrationOrder relativeMigrationOrder)
        {
            return (RelativeMigrationOrder)(-(int)relativeMigrationOrder);
        }

        public static void SetRelation(ISettingMigration left, ISettingMigration right, RelativeMigrationOrder leftToRight)
        {
            _relationCache.GetValue(left, _ => new MigrationOrderCache()).SetOrder(right, leftToRight);
            _relationCache.GetValue(right, _ => new MigrationOrderCache()).SetOrder(left, FlipOrder(leftToRight));
        }

        public int Compare(ISettingMigration x, ISettingMigration y)
        {
            if (!TryGetRelation(x, y, out RelativeMigrationOrder order))
            {
                order = DetermineOrder(x, y);
                SetRelation(x, y, order);
            }

            if (order == RelativeMigrationOrder.Unrelated)
            {
                throw new InvalidOperationException("Cannot determine the order between two unrelated migrations");
            }
            return (int)order;
        }

        public bool CanCompare(ISettingMigration x, ISettingMigration y)
        {
            if(!TryGetRelation(x, y, out RelativeMigrationOrder order))
            {
                order = DetermineOrder(x, y);
                SetRelation(x, y, order);
            }

            return order != RelativeMigrationOrder.Unrelated;
        }

        private RelativeMigrationOrder DetermineOrder(ISettingMigration x, ISettingMigration y)
        {
            var xToy = DetermineOrder(x.GetType().GetTypeInfo(), y);
            var yToX = DetermineOrder(y.GetType().GetTypeInfo(), x);

            if(xToy == RelativeMigrationOrder.Unrelated)
            {
                return FlipOrder(yToX);
            }

            if (yToX == RelativeMigrationOrder.Unrelated)
            {
                return xToy;
            }

            if(xToy != FlipOrder(yToX))
            {
                throw new InvalidOperationException("Impossibe migration order");
            }

            return xToy;
        }

        private RelativeMigrationOrder DetermineOrder(TypeInfo xType, ISettingMigration y)
        {
            RelativeMigrationOrder order = RelativeMigrationOrder.Unrelated;

            foreach (var item in xType.GetCustomAttributes<RelativeMigrationOrderAttribute>(true))
            {
                if(!item.TryDetermineExecutionOrder(y, out bool xBeforeY))
                {
                    continue;
                }

                if(order == RelativeMigrationOrder.Unrelated)
                {
                    order = xBeforeY ? RelativeMigrationOrder.Before : RelativeMigrationOrder.After;
                }
                else if ((xBeforeY && order == RelativeMigrationOrder.After) || (!xBeforeY && order == RelativeMigrationOrder.Before))
                {
                    throw new InvalidOperationException("Impossibe migration order");
                }
            }

            return order;
        }
    }
}

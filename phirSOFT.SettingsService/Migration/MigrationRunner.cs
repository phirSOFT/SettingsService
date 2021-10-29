using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using phirSOFT.SettingsService.Abstractions;

namespace phirSOFT.SettingsService
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly string _composeMigrationSettingKey;
        private const string SettingKey = "AppliedMigrations";

        public MigrationRunner(string? migrationSettingPrefix)
        {
            if (migrationSettingPrefix == null)
            {
                _composeMigrationSettingKey = SettingKey;
            }

            _composeMigrationSettingKey = $"{migrationSettingPrefix}.{_composeMigrationSettingKey}";
        }

        public Task<IReadOnlyCollection<IMigrationRunner>> FilterAppliedAsync(
            IReadOnlySettingsService settingsService,
            IEnumerable<IMigrationRunner> migrations,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task InitializeSettingsServiceAsync(ISettingsService settingsService, CancellationToken cancellationToken = default)
        {
            bool registered = await settingsService.IsRegisteredAsync(_composeMigrationSettingKey).ConfigureAwait(false);
            if(registered)
            {
                return;
            }

            IDictionary<string, string[]> appliedMigrations = new Dictionary<string, string[]>();
            await settingsService.RegisterSettingAsync(_composeMigrationSettingKey, appliedMigrations).ConfigureAwait(false);
            await settingsService.StoreAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsAppliedAsync(
            IReadOnlySettingsService settingsService,
            ISettingMigration migration,
            CancellationToken cancellationToken)
        {
            Task<IDictionary<string, string[]>> appliedMigrationsTask = settingsService.GetSettingAsync<IDictionary<string, string[]>>(_composeMigrationSettingKey);

            if(!(migration.GetType().GetTypeInfo().GetCustomAttribute<SettingMigrationAttribute>() is { } settingMigrationContext))
            {
                throw new InvalidOperationException("Cannot determine, wheter an migration is applied, because no `SettingMigrationAttribute` was found.");
            }

            IDictionary<string, string[]> appliedMigrations = await appliedMigrationsTask.ConfigureAwait(false);
            return appliedMigrations.TryGetValue(settingMigrationContext.SettingSet!, out string[] appliedMigrationStorage) && appliedMigrationStorage.Contains(settingMigrationContext.MigrationKey);

        }

        public Task MigrateSettingsAsync(
            ISettingsService settingsService,
            IReadOnlyCollection<ISettingMigration> migrations,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task MigrateSettingsAsync(
            ISettingsService settingsService,
            IReadOnlyCollection<ISettingMigration> migrations,
            ISettingMigration targetMigration,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        private IGrouping<string, ISettingMigration> GroupMigrations(IEnumerable<IMigrationRunner> migrationRunners)
        {

        }

        public Task RemoveFromSettingsServiceAsync(ISettingsService settingsService, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}

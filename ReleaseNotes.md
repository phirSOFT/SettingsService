# Version 0.2.0

The version 0.2.0 has some breaking changes compared to 0.1.x versions.

## Moved Types
The following types where moved to a different namespace (and/or) nuget package.

  * `phirSOFT.SettingsService.ISettingsService` was moved to `phirSOFT.SettingsService.Abstractions.ISettingsService` and is now shipped in the `phirSOFT.SettingsService.Abstractions` nuget package.
  * `phirSOFT.SettingsService.IReadOnlySettingsService` was moved to `phirSOFT.SettingsService.Abstractions.IReadOnlySettingsService` and is now shipped in the `phirSOFT.SettingsService.Abstractions` nuget package.

## Renamed Members
The following members were reneamed due to spelling mistakes:

  * Rename `IReadOnlySettingsService.IsRegisterdAsync` to `IReadOnlySettingsService.IsRegisteredAsync`
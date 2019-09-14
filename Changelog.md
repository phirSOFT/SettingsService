# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
## [0.2.3] - 2019-09-14
### Added
- Add Icon to `phirSOFT.SettingsService` nuget package.

### Fixed
- Fixed the Pipeline, so it does not publish build results of pull requests.


## [0.2.0] - 2019-08-26
## Added
- There is now a `ReadOnlySettingsStack`, that provides a settings stack with not writing support. `SettingsStack` inherits now from `ReadOnlySettingsStack`

### Changed
- Split `SettingsStack` into `ReadOnlySettingStack` and `SettingsStack`
- `phirSOFT.SettingsService.ISettingsService` was moved to `phirSOFT.SettingsService.Abstractions.ISettingsService` and is now shipped in the `phirSOFT.SettingsService.Abstractions` nuget package.
- `phirSOFT.SettingsService.IReadOnlySettingsService` was moved to `phirSOFT.SettingsService.Abstractions.IReadOnlySettingsService` and is now shipped in the `phirSOFT.SettingsService.Abstractions` nuget package.
- Rename `IReadOnlySettingsService.IsRegisterdAsync` to `IReadOnlySettingsService.IsRegisteredAsync`

### Other
- Start changelog

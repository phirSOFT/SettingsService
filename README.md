# SettingsService
[![Build Status](https://phirsoft.visualstudio.com/phirSOFT.SettingsService/_apis/build/status/phirSOFT.SettingsService?branchName=master)](https://phirsoft.visualstudio.com/phirSOFT.SettingsService/_build/latest?definitionId=15&branchName=master)
[![Test Results](https://img.shields.io/azure-devops/tests/phirSOFT/phirSOFT.SettingsService/15)](https://phirsoft.visualstudio.com/phirSOFT.SettingsService/_build?definitionId=15)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/phirSOFT/phirSOFT.SettingsService/15)
![Nuget](https://img.shields.io/nuget/v/phirSOFT.SettingsService)
[![License](https://img.shields.io/github/license/phirSOFT/SettingsService)](https://github.com/phirSOFT/SettingsService/blob/master/LICENSE)

The settings service is a library that provides a simlple interface to retrieve settings from different sources. The interface is designed for applications using the IoC (Inversion of Control) pattern, but it can also be used in applications. A setting can be of any type (although simple types like `struct`s and `string` are recommended) and is identified by an unique key.

## Motivation
The SettingsProvider is a Service motivated by Services in Modular applications. It was first created in a WPF Application, but it can be used in any .net Applications, since it does not require any classes outside the .net standard.

## Nuget ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/phirSOFT.SettingsService)
This package is listed in the official [nuget.org](https://www.nuget.org/packages/phirSOFT.SettingsService/) feed. You can install the latest release version by typing

> PM> Install-Package phirSOFT.SettingsService

To retrieve development versions please add the development feed at https://phirsoft.pkgs.visualstudio.com/phirSOFT.SettingsService/_packaging/phirSOFT.SettingsServer/nuget/v3/index.json to your feed list or install the package directly from that feed.

> PM> Install-Package phirSOFT.SettingsService -Source https://phirsoft.pkgs.visualstudio.com/phirSOFT.SettingsService/_packaging/phirSOFT.SettingsServer/nuget/v3/index.json

## BuildStatus


## Providers
There a some setting service implementations already, that partially allow integration in existing systems.

| Storage format | NuGet package | Repository | Notes |
| - | - | - | - |
| Json           | [![nuget](https://img.shields.io/nuget/v/phirSOFT.SettingsService.Json.svg)](https://www.nuget.org/packages/phirSOFT.SettingsService.Json/) | [phirsoft/SettingsService.Json](https://github.com/phirSOFT/SettingsService.Json) | |
| Windows Registry | [![nuget](https://img.shields.io/nuget/v/phirSOFT.SettingsService.Registry.svg)](https://www.nuget.org/packages/phirSOFT.SettingsService.Json/) | [phirsoft/SettingsService.Registry](https://github.com/phirSOFT/SettingsService.Registry) | Requires a custom RegistryAdapter for non primitive types |
| Ini file | _not published yet_ | [phirsoft/SettingsService.Ini](https://github.com/phirSOFT/SettingsService.Ini) | Requires a custom adapter for string serialization and deserialization |
| ApplicationSetting | _not published yet_ | [phirSOFT/SettingsService.ApplicationSettingsService](https://github.com/phirSOFT/SettingsService.ApplicationSettingsService) | Properties must be specified at compile time. Properties can be changed, but no new can be registered. |

## Example
Assume you are writing a WebClient for an application. You want the user to be able to configure a timeout. 

``` csharp
public class MyWebClient
{
    private readonly IReadOnlySettingsService _settingsService;
    public MyWebClient(IReadOnlySettingsService settingsService)
    {
        _settingsService = settingsService;
    }
    
    public async Task DoWorkAsync()
    {
        TimeSpan timeout = _settingService.GetSettingAsync<TimeSpan>("Timeout");
        
        // use the timeout
    }
}
```

The above example retrieves the current timeout setting each time the web client has to perform some work. The settings service API is intentionally asynchronous, since there is a high propability, that retrieving a setting involves some io operation.

To make the example abovr work, you have to register the `Timeout` property within the settings service. It's recommendet to perform the registration at the startup of the application. However you should register a property, before any component requires that property.
``` csharp
private static async Task InitializeSettingsAsync(ISettingsService settingsService)
{
    await settingsService.RegisterPropertyAsync("Timeout", TimeSpan.FromSeconds(30));
}
```

If you have lots of properties, you may want to use some _marker property_ that existence will tell you, wheter the settings service is initialized

``` csharp
private static async Task InitializeSettingsAsync(ISettingsService settingsService)
{
    if(await settingsService.IsRegisteredAsync("Initialized")
        await RegisterProperties(settingsService)
}
```

## Contributing
Contributions to this (and the backend implementations) are welcome. See [How to Contribute](contributing.md) for further information.

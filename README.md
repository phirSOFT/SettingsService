# SettingsService
The settings service is a library that provides a simlple interface to retrieve settings from different sources. The interface is designed for applications using the IoC (Inversion of Control) pattern, but it can also be used in applications. A setting can be of any type (although simple types like `struct`s and `string` are recommended) and is identified by an unique key.

## Motivation
The SettingsProvider is a Service motivated by Services in Modular applications. It was first created in a WPF Application, but it can be used in any .net Applications, since it does not require any classes outside the .net standard.

## Installation
This package is listed in the official [nuget.org](https://www.nuget.org/packages/phirSOFT.SettingsService/) feed. You can install the latest release version by typing

> PM> Install-Package phirSOFT.SettingsService

To retrieve development versions please add the development feed at https://ci.appveyor.com/nuget/phirSOFT.SettingsService to your feed list or install the package directly from that feed.

> PM> Install-Package phirSOFT.SettingsService -

## Providers
There a some setting service implementations already, that partially allow integration in existing systems.

| Storage format | NuGet package | Repository | Notes |
| - | - | - | - |
| Json           | [![nuget](https://img.shields.io/nuget/v/phirSOFT.SettingsService.Json.svg)](https://www.nuget.org/packages/phirSOFT.SettingsService.Json/) | [phirsoft/SettingsService.Json](https://github.com/phirSOFT/SettingsService.Json) | |
| Windows Registry | [![nuget](https://img.shields.io/nuget/v/phirSOFT.SettingsService.Registry.svg)](https://www.nuget.org/packages/phirSOFT.SettingsService.Json/) | [phirsoft/SettingsService.Registry](https://github.com/phirSOFT/SettingsService.Registry) | Requires a custom RegistryAdapter for non primitive types |
| Ini file | _not published yet_ | [phirsoft/SettingsService.Ini](https://github.com/phirSOFT/SettingsService.Ini) | Requires a custom adapter for string serialization and deserialization |

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

## Settings Injection
If you are using [Unity](https://github.com/unitycontainer) as DI container there is also an [package](https://github.com/phirSOFT/SettingsService.Unity) to let unity inject settings as a depencency. So we could refactor our example above to this

``` csharp
public class MyWebClient
{
    private readonly TimeSpan _timeout;
    public MyWebClient([SettingValue("Timeout"] TimeSpan timeout )
    {
        _timeout = timeout;
    }
}
```
Note that this is _not exact_ semantically the same as the example above. In that case the setting is retrieved each time `DoWorkAsync` is invoked, while this example retrives the timeout once and does _not_ update it later on.

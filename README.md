[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# AlienFx.Api NuGet package

[![NuGet](https://img.shields.io/nuget/v/AlienFx.Api)](https://www.nuget.org/packages/AlienFx.Api/)

A .NET 10 API client for controlling AlienFX lighting zones on Alienware systems.

## Status

This package provides:

- A production-ready `AlienFxClient` API surface.
- Built-in profiles including `AllWhite` and `PanoramicTeal`.
- HID transport using `HidSharp` and AlienFX protocol command packets.
- Unit tests for profile behavior and protocol packet generation.

Current limitation:

- `GetProfileAsync` returns the current profile tracked by this client instance. Reading the active hardware profile from AWCC storage is planned and tracked in `PLAN.md`.

## Installation

```powershell
Install-Package AlienFx.Api
```

## Requirements

- .NET 10 SDK/runtime.
- Windows.
- AlienFX-capable device.

## Quick Start

```csharp
using AlienFx.Api;

var client = new AlienFxClient();
await client.SetProfileAsync(AlienFxProfiles.AllWhite, CancellationToken.None);
```

## Main API

Required methods:

```csharp
public async Task<AlienFxProfile> GetProfileAsync(CancellationToken)

public async Task SetProfileAsync(AlienFxProfile, CancellationToken)
```

Additional convenience method:

```csharp
public async Task SetProfileAsync(string builtInProfileName, CancellationToken)
```

## Built-in Profiles

The package currently includes:

- `AllWhite`.
- `PanoramicTeal`.

You can also create your own:

```csharp
var profile = AlienFxProfiles.AllZonesSolid(AlienFxColor.FromHex("#FF6A00"), "WarmOrange");
await client.SetProfileAsync(profile, cancellationToken);
```

## Custom Profiles

```csharp
var custom = new AlienFxProfile(
    "KeyboardAndLogo",
    new Dictionary<AlienFxZone, AlienFxColor>
    {
        [AlienFxZone.LeftZone] = AlienFxColor.FromHex("#00FFFF"),
        [AlienFxZone.RightZone] = AlienFxColor.FromHex("#00FFFF"),
        [AlienFxZone.AlienFrontLogo] = AlienFxColor.FromHex("#FF00FF"),
    });

await client.SetProfileAsync(custom, cancellationToken);
```

## Device Notes

- AlienFX hardware differs by model and firmware.
- The protocol zone model is based on known reverse-engineered command formats and AWCC metadata.
- Unknown or unavailable zones may be ignored by hardware.

## If Keys Do Not Change

If no key colors change, try these steps in order:

1. Use explicit AWCC Advanced Keyboard IDs:

```csharp
var options = new AlienFxClientOptions
{
    VendorId = 0x0D62,
    ProductId = 0xD2B0,
};

await using var client = new AlienFxClient(options);
await client.SetProfileAsync(AlienFxProfiles.AllWhite, CancellationToken.None);
```

2. Fully close Alienware Command Center before running your app.
3. Run your process elevated once to rule out permission/handle issues.
4. Start with a full-device profile (`AllWhite`) before testing per-zone color maps.

The transport now attempts both common AlienFX vendor IDs (`0x187C` and `0x0D62`) automatically when using defaults.

## Error Handling

Typical failures:

- Device not found for the configured Vendor ID / Product ID.
- Device opened but not writable (permissions / exclusive use).
- Unsupported platform (non-Windows).

## Development

Build and test:

```powershell
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

Package output goes to `artifacts/`.

## Design and Roadmap

- Architecture and design principles: `DESIGN.md`
- Delivery plan to full coverage and Codacy A: `PLAN.md`
- Current machine/profile posterity snapshot: `CURRENT-STATE.md`

## License

MIT

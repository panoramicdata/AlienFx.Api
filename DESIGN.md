# AlienFx.Api Design

## Purpose

`AlienFx.Api` provides a focused .NET 10 API for applying lighting profiles to AlienFX-capable devices with a stable, testable client surface.

## Design Principles

1. Keep the public API small and explicit.
2. Keep hardware concerns behind an internal transport abstraction.
3. Make profile models deterministic and easy to test.
4. Prefer immutable value models for profile data.
5. Keep package/tooling standards aligned with sibling Panoramic Data NuGet repos.

## Public API Shape

Core entry point:

- `AlienFxClient`

Core async methods:

- `Task<AlienFxProfile> GetProfileAsync(CancellationToken)`
- `Task SetProfileAsync(AlienFxProfile, CancellationToken)`

Supporting models:

- `AlienFxProfile`
- `AlienFxColor`
- `AlienFxZone`
- `AlienFxProfiles`
- `AlienFxClientOptions`

## Architecture

Layered structure:

1. Public API layer
- `AlienFxClient` orchestrates flow and concurrency.
- `AlienFxProfile` and related models represent intent.

2. Protocol layer
- `AlienFxProtocol` builds command packets and zone mappings.
- Contains protocol constants and report composition logic.

3. Transport layer
- `IAlienFxTransport` isolates hardware operations.
- `AlienFxUsbTransport` implements HID communication.

This separation keeps packet composition and client orchestration testable without hardware.

## Concurrency Model

`AlienFxClient` uses an internal `SemaphoreSlim` gate to serialize calls that mutate state or write to the HID stream.

Rationale:

- Hardware writes are order-sensitive.
- Multiple parallel profile writes can produce undefined visual output.

## Error Model

- Invalid inputs throw `ArgumentException` / `ArgumentNullException`.
- Non-Windows execution throws `PlatformNotSupportedException`.
- Missing HID device throws `InvalidOperationException` unless `ThrowIfDeviceNotFound` is disabled.

Future refinement:

- Introduce typed exception hierarchy for transport/protocol/device-state failures.

## Zone and Profile Model

`AlienFxZone` contains known protocol zones.

`AlienFxProfile` contains:

- `Name`
- `ZoneColors` map

Profiles are immutable from caller perspective after construction.

## Built-in Profiles Strategy

Built-ins target high-value use cases first:

- Single-color full-device profiles.
- User-requested all-zones profile (`AllAlwaysAll`).

Future built-ins can add gradients, game-specific presets, and model-specific variants.

## Device Discovery Strategy

Current approach:

- Enumerate HID devices by Vendor ID and optional Product ID.
- Prefer devices with sufficient output report length.

Future approach:

- Add model-aware discovery using AWCC metadata.
- Add optional explicit device selector callbacks.

## Testing Strategy

Current tests cover:

- Color parsing/formatting.
- Built-in profile shape.
- Protocol packet command bytes.
- Client orchestration through fake transport.

Planned additions:

- Integration tests gated by environment variables and hardware presence.
- Golden test vectors for each zone command.
- Fuzz and boundary tests for packet builder.

## Security and Privacy

- No secrets are required by the library.
- No network calls are performed by default.
- Device access is local HID only.

## Compatibility and Packaging

- Target framework: `net10.0` only.
- MIT licensed.
- SourceLink + symbol package generation enabled.
- CI workflows for build, test, package, and CodeQL.

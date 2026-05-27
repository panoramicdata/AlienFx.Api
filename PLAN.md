# AlienFx.Api Delivery Plan

This plan is designed so another AI (or engineer) can continue from the current state without re-discovery.

## Current Baseline

Completed:

- Repository scaffolded and published at `panoramicdata/AlienFx.Api`.
- Default branch is `main`.
- .NET 10-only targeting in both library and tests.
- Central package management, MIT license, CI, CodeQL, Dependabot, publish script.
- Public client surface implemented:
  - `Task<AlienFxProfile> GetProfileAsync(CancellationToken)`
  - `Task SetProfileAsync(AlienFxProfile, CancellationToken)`
- Built-in profiles implemented (`AllWhite`, `PanoramicTeal`).
- Initial unit tests in place.

Known gap:

- `GetProfileAsync` currently returns the in-memory profile state for this client instance, not the active profile read directly from AWCC persistence or HID state.

## Goal State

1. Full line and branch coverage on core library logic.
2. Integration coverage for real hardware paths.
3. Codacy grade A with no code smells or security hotspots.
4. Stable NuGet package with clear docs and reproducible release flow.

## Phase 1: Hardening Core API

1. Add XML docs for all public members.
2. Add argument validation tests for all public constructors and methods.
3. Add cancellation-path tests for every async method.
4. Introduce custom exception types:
   - `AlienFxException`
   - `AlienFxDeviceNotFoundException`
   - `AlienFxTransportException`

Acceptance criteria:

- 100% branch coverage for public method guard clauses.
- No analyzer warnings.

## Phase 2: Protocol Fidelity

1. Expand protocol packet tests to all zones.
2. Add tests for special zone behavior (`MacroKeys`, `PowerButton`).
3. Add packet-length boundary tests and invalid-zone behavior tests.
4. Validate command bytes against captured hardware traces.

Acceptance criteria:

- Golden vector test suite for every command type.
- No undocumented magic numbers in protocol layer.

## Phase 3: Real Profile Readback

1. Add optional AWCC profile reader abstraction:
   - `IAlienFxProfileStore`
   - `AwccSqliteProfileStore` (preferred)
2. Use `Microsoft.Data.Sqlite` to read `FXRepository.db` when available.
3. Map AWCC profile records to `AlienFxProfile`.
4. Update `GetProfileAsync` behavior:
   - if store configured and readable: return persisted active profile
   - otherwise: return last applied in-memory profile

Acceptance criteria:

- Integration test with seeded SQLite fixture.
- Feature flag in options to disable store access.

## Phase 4: Transport Resilience

1. Add retry policy for transient write failures.
2. Add configurable readiness polling strategy.
3. Add telemetry hooks (diagnostic events) for command flow.
4. Add optional dry-run mode for diagnostics.

Acceptance criteria:

- Deterministic retry tests with fake failing transport.
- No unobserved task exceptions.

## Phase 5: Integration Test Matrix

1. Create `AlienFx.Api.IntegrationTest` project (opt-in).
2. Gate tests using environment variables:
   - `ALIENFX_INTEGRATION=1`
   - `ALIENFX_VID`
   - `ALIENFX_PID` (optional)
3. Add tests:
   - Device discovery
   - Set all-zones profile
   - Set subset profile
   - Readback consistency
4. Publish a hardware test checklist in docs.

Acceptance criteria:

- Integration suite passes on at least one known target model.
- Failures produce actionable diagnostics.

## Phase 6: Codacy A Path

1. Configure Codacy for repository and enable C# quality/security rules.
2. Fix all medium/high issues; suppress only with documented rationale.
3. Add code duplication checks for protocol constants and mapping tables.
4. Ensure complexity thresholds for methods remain below agreed limits.

Acceptance criteria:

- Codacy grade A.
- No open high-severity quality issues.

## Coverage Plan

Short-term commands:

```powershell
dotnet test -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

Medium-term additions:

- Add branch coverage gate in CI.
- Add threshold properties in test run:
  - `/p:Threshold=90`
  - `/p:ThresholdType=line,branch,method`

Final gate target:

- Core library: 95%+ line, 90%+ branch.
- Protocol and validation code: 100% branch.

## Handoff Notes For Next AI

1. Start by running:
   - `dotnet restore`
   - `dotnet build -c Release`
   - `dotnet test -c Release`
2. Confirm baseline tests are green before refactoring.
3. Implement Phase 1 and Phase 2 first; do not begin Phase 3 until protocol tests are complete.
4. For Phase 3, inspect these local machine artifacts if available:
   - `C:\Users\<user>\AppData\Local\Alienware\Alienware Command Center\FX\FXRepository.db`
   - `C:\ProgramData\Alienware\Alienware Command Center\FXMetadata\*.json`
5. Keep public API signatures backward-compatible unless explicit version bump is planned.
6. Preserve `main` branch release tagging via `Publish.ps1` and `version.json`.

## Definition of Done

- CI green on build/test/pack.
- CodeQL workflow green.
- Codacy grade A.
- README and DESIGN remain accurate for released behavior.
- Package published with semantic tag from `main`.

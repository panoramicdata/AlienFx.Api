# Current State Snapshot (Posterity)

Date captured: 2026-05-27

## Built-in Profile Decision

- Removed from built-ins: `AllAlwaysAll`.
- Kept as historical reference in code: `LegacyAllAlwaysAll`.
- Current built-ins intended for users:
  - `AllWhite`
  - `PanoramicTeal`

## Local Machine Findings (DavidBond)

From local AWCC metadata discovery:

- Model: `0x1101` (Alienware M16 R1 family metadata observed)
- Standard keyboard metadata entry observed:
  - VID `0x187C`
  - PID `0x0551`
- Advanced keyboard metadata entry observed:
  - VID `0x0D62`
  - PID `0xD2B0`

Relevant local files observed:

- `C:\ProgramData\Alienware\Alienware Command Center\FXMetadata\AuthorizedDeviceInfo.json`
- `C:\ProgramData\Alienware\Alienware Command Center\FXMetadata\0x1101_1.0.0.0.json`
- `C:\ProgramData\Alienware\Alienware Command Center\FXMetadata\0x1101_AdvKB_1.0.0.0.json`
- `C:\Users\DavidBond\AppData\Local\Alienware\Alienware Command Center\FX\FXRepository.db`

## Known Issue Recorded

- User report: no key colors changed yet.
- Mitigations now implemented:
  - Transport tries both common AlienFX vendor IDs (`0x187C`, `0x0D62`) when default options are used.
  - README now includes explicit options for `VendorId=0x0D62` and `ProductId=0xD2B0`.

## Outstanding Investigation

- Active currently-selected AWCC profile color has not yet been extracted from `FXRepository.db` due missing local SQLite CLI/runtime tooling in this session.
- Next step: add a managed SQLite reader (`Microsoft.Data.Sqlite`) in library tooling or a small helper utility to query active profile rows.

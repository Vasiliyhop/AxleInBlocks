# Changelog

Notable player-visible changes to Axle in Blocks are recorded here.

## Unreleased

## 1.22.3 - 2026-07-19

### Changed

- Ported the supplied 1.21.1 mod release to Vintage Story 1.22.3 and .NET 10.
- Updated the project and packaging tools to the current template structure.
- Reused current vanilla mechanical-power implementations where compatible.
- Preserved the legacy axle block entity identity for existing worlds.

### Fixed

- Updated mechanical connector calls for the current game API.
- Restored covered transmission and gear-crate rendering.
- Preserved material and orientation variants during placement and rotation.
- Protected covered gear crates from destructive large-gear conversion.
- Corrected invalid JSON, recipes, shape textures, and aged-plank references.
- Added support for the plank and stone variants present in Vintage Story 1.22.

### Validation

- Builds without compiler warnings or errors against Vintage Story 1.22.3.
- Validated all mod JSON, shape references, and texture references.
- Tested startup and world loading in both client and dedicated-server modes.

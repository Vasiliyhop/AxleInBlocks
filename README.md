# Axle in Blocks

Axle in Blocks hides axles, angled gears, and transmissions inside brick,
dry-stone, clay-brick, and plank covers in Vintage Story.

This community-maintainable port is based on `VintageStory.Mod.Templates
1.22.0`, targets Vintage Story 1.22.3, and uses .NET 10. The original mod was
created by Fossilien.

## Maintenance status

> I currently do not have enough time to actively support or develop this
> project. If you would like to help maintain it, contributions and new
> maintainers are welcome. Pull requests are especially appreciated; issues
> may not receive a quick response.

Binary versions of the mod are distributed through the
[Vintage Story Mod DB](https://mods.vintagestory.at/axleinblocks). This
repository is intended for source code, development, and collaboration. The
[original forum topic](https://www.vintagestory.at/forums/topic/6132-axle-in-blocks/)
is also available for historical discussion.

## Requirements

- Vintage Story 1.22.3 (the tested version)
- .NET 10 SDK
- A `VINTAGE_STORY` environment variable pointing to the game installation

For the current PowerShell session:

```powershell
$env:VINTAGE_STORY = 'C:\Games\Vintagestory'
```

To set it persistently for the current Windows user:

```powershell
[Environment]::SetEnvironmentVariable(
    'VINTAGE_STORY',
    'C:\Games\Vintagestory',
    'User'
)
```

Restart Visual Studio or your terminal after changing a persistent environment
variable. On Linux, set `VINTAGE_STORY` to the equivalent game directory.

## Build

Open `AxleInBlocks.sln` in Visual Studio and build the `Release`
configuration, or run:

```powershell
dotnet build .\AxleInBlocks.sln -c Release
```

To validate the JSON assets, build the mod, and create the distributable ZIP:

```powershell
dotnet run --project .\CakeBuild\CakeBuild.csproj --
```

`build.ps1` and `build.sh` run the same packaging command. The resulting file
is written to `Releases/axleinblocks_<version>.zip` and can be uploaded to the
Vintage Story mod site.

## Debug

Visual Studio, Rider, and VS Code launch profiles load the compiled mod from
`AxleInBlocks/bin/<Configuration>/Mods` and the source assets from
`AxleInBlocks/assets`. Set `VINTAGE_STORY` before starting the client or server
profile.

The mod uses the current vanilla mechanical-power behaviors wherever their
variant handling is safe. Gear crates keep a small compatibility layer because
they add material and cover variants that vanilla angled gears do not have.

## Contributing

Bug fixes, compatibility updates, tests, documentation, and translations are
welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for the development workflow.
Player-visible changes should also be recorded in
[CHANGELOG.md](CHANGELOG.md).

Do not commit Vintage Story binaries, local game data, build output, or packaged
mods. Contributors must use their own legally obtained game installation.

## Credits and license

- Original mod: Fossilien (Vasyl Shevchuk)
- French localization: Laerinok
- Japanese localization: macoto_hino / Macoto Hino

Some mechanical-power implementation details and shape data are based on the
[Vintage Story Survival Mod](https://github.com/anegostudios/vssurvivalmod).
Anego Studios permits its source to be read, learned from, and used as a basis
for Vintage Story mods. Vintage Story code and original game assets remain the
property of Anego Studios and are not redistributed here in unmodified form.

This repository is distributed under GPL-3.0-only, the same license used by
the official
[Vintage Story .NET mod templates](https://www.nuget.org/packages/VintageStory.Mod.Templates/1.22.0).
See [LICENSE](LICENSE). Please preserve the credits above when sharing modified
versions.

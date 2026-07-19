# Contributing to Axle in Blocks

Thank you for helping keep Axle in Blocks working. The current maintainer has
limited time for support, so focused pull requests with clear testing notes are
the most useful form of contribution. Contributors interested in longer-term
maintenance are very welcome.

## Development setup

You need:

- the .NET 10 SDK;
- your own legally obtained Vintage Story 1.22.3 installation;
- the `VINTAGE_STORY` environment variable set to the game directory.

For example, in a Windows PowerShell session:

```powershell
$env:VINTAGE_STORY = 'C:\Games\Vintagestory'
```

Then restore and build the solution:

```powershell
dotnet restore .\AxleInBlocks.sln
dotnet build .\AxleInBlocks.sln -c Release --no-restore
```

Create a complete test package with:

```powershell
dotnet run --project .\CakeBuild\CakeBuild.csproj --
```

The packaging command also validates every JSON file in the mod assets.

## Testing changes

Test gameplay changes in both singleplayer and a dedicated server when they can
affect world state or mechanical power. At minimum, check the changed block in
all relevant orientations and materials. For mechanical changes, verify:

- axle placement, support removal, and rotation;
- transmission placement and rendering;
- angled gear connection and disconnection;
- gear crates next to large gears;
- loading blocks saved by earlier mod versions.

Use a disposable test world or make a backup before testing migrations.

## Pull requests

- Keep each pull request focused on one fix or feature.
- Explain the problem, the solution, and how it was tested.
- Update `CHANGELOG.md` for player-visible changes.
- Keep JSON strict: no comments or trailing commas.
- Preserve existing block codes and entity class names unless the change
  includes a tested world-migration path.
- Do not commit `bin`, `obj`, `.vs`, `Releases`, game DLLs, logs, or worlds.
- Do not redistribute files from the Vintage Story installation.

By submitting a contribution, you confirm that you have the right to provide
it and agree that it may be distributed under GPL-3.0-only. Disclose any
third-party material or additional requirements in the pull request.

Formatting-only rewrites of large JSON or shape files should be kept separate
from functional changes so reviews remain practical.

## Bug reports

Include the Vintage Story version, mod version, client/server log excerpt, exact
reproduction steps, and whether the problem also occurs in a new world. Reports
are useful, but response times are not guaranteed because active maintenance is
limited.

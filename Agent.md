# AI Agent Guidance

This repository is a .NET 10 solution for a Blazor WebAssembly Medicare rebate checker. Keep changes small, readable, and aligned with the existing C# and Razor style.

## Project Shape

- `MedicareRebateChecker.Web/` is the Blazor WebAssembly app.
- `MedicareRebateChecker.DataUpdater/` downloads MBS XML data and writes generated JSON into `MedicareRebateChecker.Web/wwwroot/data/`.
- UI components use MudBlazor and app-specific styles in `MedicareRebateChecker.Web/wwwroot/css/app.css`.

## Common Commands

Run commands from the repository root:

```bash
dotnet restore Pathology-Comparison.sln
dotnet build Pathology-Comparison.sln
dotnet run --project MedicareRebateChecker.Web
```

Refresh generated MBS data with:

```bash
dotnet run --project MedicareRebateChecker.DataUpdater
```

The data updater requires network access and rewrites the JSON files in `MedicareRebateChecker.Web/wwwroot/data/`.

## Change Guidelines

- Prefer existing patterns: nullable reference types, implicit usings, sealed records for simple data models, and MudBlazor components for UI.
- Keep data loading logic in services under `MedicareRebateChecker.Web/Services/`.
- Keep generated data changes intentional. If `mbs-items.json` or `mbs-metadata.json` changes, make sure the data updater was the source of the change.
- Avoid broad UI restyles unless the task asks for them. Match the current compact, information-focused layout.
- Do not commit local IDE or build output files.

## Validation

- Run `dotnet build Pathology-Comparison.sln` after code changes.
- There is no test project currently. If adding behavior with meaningful logic, consider adding focused tests or documenting why build-only validation is sufficient.

# Changelog

All notable changes to the AzDO MS Project Add-in are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

- None.

---

## [1.0.0] — Modernization (2025)

### Added

- **AzDOAddIn.Core** class library: shared Azure DevOps REST client and DTOs, no Office dependency.
- **Async API:** All Azure DevOps calls use `async`/`await`; single shared `HttpClient` in Core.
- **Structured errors:** `AzDoApiException` with status code and response body for API failures.
- **Windows Credential Manager** for PAT storage; one-time migration from legacy encrypted storage.
- **StoredUrls** user setting for the list of saved organization URLs.
- **Inno Setup** installer script (`installer/AzDOAddIn.iss`) as the recommended install path.
- **Documentation:** [User Guide](docs/USER_GUIDE.md), [Install](docs/INSTALL.md), [Development](docs/DEVELOPMENT.md), [Contributing](CONTRIBUTING.md), and this changelog.

### Changed

- **Azure DevOps API** version set to **7.0** in the REST client.
- **ProjectOperations** methods that call the API are now async (e.g. `UpdateProjectPlanAsync`, `PublishProjectPlanAsync`).
- **Ribbon** and **forms** use async handlers and await the new async methods.
- **DocProperties** typo fixed (was `DocPropeties`).
- **GetOperationalSettings** now reads the correct document property (OperationalSettings instead of PlanningSettings).
- **README** updated with prerequisites, build steps, install options, and link to PAT/Credential Manager.
- **Azure Pipelines:** Path triggers, artifact publish of add-in output, and clearer step names.

### Removed

- **AzDORestApiHelper** (replaced by `AzDoRestClient` in Core).
- Inline **RestApiClasses** from the add-in project (moved to Core).
- Deprecated `#warning` and blocking `.Result` usage in REST calls.

### Deprecated

- **AzDOAddInSetup.vdproj** (Visual Studio Setup Project). Use Inno Setup or manual install instead.

---

## [0.1-alpha] — Initial release

- First release: [GitHub release tag v0.1-alpha](https://github.com/ashamrai/AzDOMSProject/releases/tag/v0.1-alpha).
- Link plan to team project, add columns, import team members, publish work items, get work items, import children, update plan from Azure DevOps.
- PAT stored in encrypted user settings (machine-key based).

---

[Unreleased]: https://github.com/ashamrai/AzDOMSProject/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/ashamrai/AzDOMSProject/compare/v0.1-alpha...v1.0.0
[0.1-alpha]: https://github.com/ashamrai/AzDOMSProject/releases/tag/v0.1-alpha

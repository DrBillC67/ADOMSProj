# Development Guide — AzDO MS Project Add-in

This document is for contributors and developers who build or modify the add-in.

---

## Solution structure

```
AzDOAddIn/
├── AzDOAddIn.sln
├── AzDOAddIn/           # VSTO add-in (UI + Project OM)
│   ├── AzDOAddIn.csproj
│   ├── Forms/           # Dialogs (Link, Get Work Items, Settings, Teams, Work Item Types)
│   ├── RibbonPanel.cs   # Ribbon UI
│   ├── ProjectOperations.cs  # Plan ↔ work item sync logic (uses Core)
│   ├── PatHelper.cs     # PAT storage (Windows Credential Manager)
│   ├── PlanSettings.cs
│   ├── AzDOAddInSettings.cs  # Field mappings, doc properties
│   └── ...
├── AzDOAddIn.Core/      # Class library (no Office dependency)
│   ├── AzDOAddIn.Core.csproj
│   ├── AzDoRestClient.cs    # Azure DevOps REST API (async, single HttpClient)
│   ├── AzDoApiException.cs
│   └── RestApiClasses/  # DTOs for API responses
└── AzDOAddInSetup/      # Legacy .vdproj (deprecated; use installer/)
```

- **AzDOAddIn:** VSTO add-in for MS Project. References **AzDOAddIn.Core** and NuGet packages (Newtonsoft.Json, CredentialManagement). Handles ribbon, dialogs, and Project object model.
- **AzDOAddIn.Core:** .NET Framework 4.8 class library. Contains all Azure DevOps REST calls (async), DTOs, and structured errors. No reference to Office or VSTO so it can be unit-tested or reused elsewhere.

---

## Building and running

- **Visual Studio:** Open `AzDOAddIn.sln`, restore NuGet packages, build. Set **AzDOAddIn** as the startup project to run Project with the add-in under the debugger.
- **Command line:** Use MSBuild or Visual Studio developer prompt, e.g.  
  `msbuild AzDOAddIn.sln /p:Configuration=Release /p:Platform="Any CPU"`

---

## Key design points

- **Async:** All Azure DevOps API calls are async (`AzDoRestClient.*Async`). The add-in uses `async void` ribbon handlers and `async Task` in `ProjectOperations` and forms, with `ConfigureAwait(true)` where UI/COM must run on the main thread.
- **Single HttpClient:** `AzDoRestClient` uses one shared `HttpClient` (lazy-initialized). PAT is sent per request via the `Authorization` header, not stored on the client.
- **PAT storage:** PATs are stored in **Windows Credential Manager** (CredentialManagement NuGet). A list of org URLs is kept in user settings (`StoredUrls`) for the “saved URLs” dropdown. Legacy encrypted storage is migrated once on first use.
- **API version:** The client uses Azure DevOps REST API version **7.0** (set in `AzDoRestClient`).

---

## Adding or changing API calls

1. Add or update methods in **AzDOAddIn.Core/AzDoRestClient.cs** (keep them async and use the shared `HttpClient`).
2. Add or update DTOs in **AzDOAddIn.Core/RestApiClasses/** as needed.
3. Call the new methods from **ProjectOperations** or forms with `await` and handle `AzDoApiException` where appropriate.

---

## Testing

- **Unit tests:** The Core library is a good candidate for unit tests (e.g. mapping, request building). The add-in project is harder to test without Project.
- **Manual testing:** Run the add-in from Visual Studio (F5), link to a test team project, and exercise Publish, Get Work Items, Update Plan, and Import Childs.

---

## CI/CD

- **Azure Pipelines:** `azure-pipelines.yml` at the repo root builds the solution, restores packages (including `RestorePackagesConfig` for the add-in), and publishes the add-in output as a build artifact. Trigger is on `main`/`master` with path filters on `AzDOAddIn/**` and the pipeline file.

---

## Code style and conventions

- Use C# async/await; avoid `.Result` or `.Wait()` on async methods.
- Prefer `ConfigureAwait(true)` when continuing on the UI thread after an await in the add-in.
- Handle errors in the UI (e.g. `MessageBox`) and use `AzDoApiException` in Core for API failures.
- Keep Office/Project-specific types (e.g. `PjField`, task/resource) in the add-in project; Core stays free of VSTO and Office interop.

---

## Related documentation

- [User Guide](USER_GUIDE.md) — End-user help
- [Installation](INSTALL.md) — Prerequisites and install steps
- [Changelog](../CHANGELOG.md) — Version history

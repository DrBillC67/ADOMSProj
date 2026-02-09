# Installation Guide — AzDO MS Project Add-in

This document covers prerequisites, building from source, and installing the add-in.

---

## Prerequisites

- **Microsoft Project** (desktop), e.g. 2016 or later. The add-in targets the Project desktop application, not Project for the web.
- **.NET Framework 4.8** (usually installed with Windows or Visual Studio).
- To **build** from source:
  - **Visual Studio 2019 or 2022** with:
    - .NET desktop development
    - Office/SharePoint development (VSTO) workload

---

## Build from source

1. Clone or download the repository.
2. Open `AzDOAddIn\AzDOAddIn.sln` in Visual Studio.
3. **Restore NuGet packages:** Solution → Right-click → **Restore NuGet Packages** (or build, which triggers restore).
4. Set configuration to **Release** and platform **Any CPU**.
5. Build the solution (**Build → Build Solution**).

**Output:**

- Add-in and dependencies: `AzDOAddIn\AzDOAddIn\bin\Release\`
- Core library is copied there automatically via project reference.

If restore fails for the add-in project (e.g. CredentialManagement), ensure the **NuGet** package source is enabled and try **Restore NuGet Packages** again.

---

## Install the add-in

### Option A — Inno Setup (recommended)

1. Build the solution in **Release** (see above).
2. Install [Inno Setup](https://jrsoftware.org/isinfo.php) if you have not already.
3. Open `installer\AzDOAddIn.iss` in Inno Setup Compiler.
4. Run **Build → Compile** to produce `installer\Output\AzDOAddInSetup.exe`.
5. Run `AzDOAddInSetup.exe` and follow the wizard. The add-in is installed for the current user (e.g. under `%APPDATA%\AzDOAddIn`) and registry entries are created for MS Project.

### Option B — Manual copy and registration

1. Build the solution in **Release**.
2. Copy the entire contents of `AzDOAddIn\AzDOAddIn\bin\Release\` to a permanent folder (e.g. `C:\Program Files\AzDOAddIn\` or `%LOCALAPPDATA%\AzDOAddIn`). Ensure all DLLs (including `AzDOAddIn.Core.dll`, `Newtonsoft.Json.dll`, `CredentialManagement.dll`) and `AzDOAddIn.dll.config` are present.
3. Register the add-in for MS Project using the steps in [Deploying a VSTO solution by using Windows Installer](https://docs.microsoft.com/en-us/visualstudio/vsto/deploying-an-office-solution-by-using-windows-installer). You will need to create or reuse a deployment manifest and set the registry keys under `HKCU\Software\Microsoft\Office\Project\Addins\` so Project loads your add-in.

### After installation

1. Start **Microsoft Project**.
2. Confirm the **Azure DevOps Work Items** tab appears on the ribbon.
3. Open or create a plan and follow the [User Guide](USER_GUIDE.md) to link to a team project and create a PAT.

---

## Uninstall

- **Inno Setup:** Use **Settings → Apps → AzDO MS Project Add-in → Uninstall** (or run the installer again and choose Remove).
- **Manual:** Delete the folder you copied the files to and remove the add-in registry keys under `HKCU\Software\Microsoft\Office\Project\Addins\`.

PATs stored in Windows Credential Manager (targets starting with `AzDOAddIn:`) are not removed by uninstall; you can delete them in **Control Panel → Credential Manager → Windows Credentials** if desired.

---

## Legacy installer (deprecated)

The solution includes **AzDOAddInSetup** (`.vdproj`), a Visual Studio Setup Project. This project type is deprecated and not supported in current Visual Studio. Use **Inno Setup** (Option A) or manual installation (Option B) instead.

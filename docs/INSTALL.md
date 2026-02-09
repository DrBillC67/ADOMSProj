# Installation Guide — AzDO MS Project Add-in

This document covers prerequisites, building from source, and installing the add-in.

---

## Prerequisites

- **Microsoft Project** (desktop), e.g. 2016 or later. The add-in targets the Project desktop application, not Project for the web.
- **.NET Framework 4.8** (usually installed with Windows or Visual Studio).
- To **build** from source:
  - **Visual Studio 2019 or 2022** (full IDE, not VS Code) with:
    - .NET desktop development
    - **Office/SharePoint development (VSTO)** workload  

  If the **AzDOAddIn** project appears **(incompatible)** in Solution Explorer, the VSTO workload is missing or you are not in full Visual Studio. Install the workload via **Visual Studio Installer → Modify → Office/SharePoint development**. The **AzDOAddInSetup** project is expected to be incompatible (deprecated); use the Inno Setup installer instead.

---

## Build from source

The **full solution must be built in Visual Studio**. The add-in project requires the VSTO (Office) build targets, which are not included in the .NET SDK. Running `dotnet build` on the solution will fail for the add-in; use Visual Studio instead.

1. Clone or download the repository.
2. Open `AzDOAddIn\AzDOAddIn.sln` in Visual Studio.
3. **Restore NuGet packages:** Solution → Right-click → **Restore NuGet Packages** (or build, which triggers restore).
4. Set configuration to **Release** and platform **Any CPU**.
5. Build the solution (**Build → Build Solution**). If a project is skipped (e.g. "1 skipped"), use **Build → Rebuild Solution** to build everything. In **Configuration Manager** (solution right-click), ensure both **AzDOAddIn** and **AzDOAddIn.Core** have **Build** checked for your active configuration.

**Output:**

- Add-in and dependencies: `AzDOAddIn\AzDOAddIn\bin\Release\` (relative to the folder that contains the `.sln` file).
- Core library is copied there automatically when the add-in project builds.

If restore fails for the add-in project (e.g. CredentialManagement), ensure the **NuGet** package source is enabled and try **Restore NuGet Packages** again.

**Troubleshooting:** If **AzDOAddIn** shows as "(incompatible)", open the solution in **Visual Studio** (not VS Code/Cursor) and install the **Office/SharePoint development (VSTO)** workload. The add-in cannot be built without it.

---

### Verify build output — "The files are not there"

If the installer says no files were found, or the add-in doesn’t load, the add-in project likely **did not build**. Only **AzDOAddIn.Core** may have built.

**1. Check the folder**

Open this folder in File Explorer (replace with your repo path):

- **Path:** `<your-repo>\AzDOAddIn\AzDOAddIn\bin\Release\`  
  Example: `C:\Dev\MSPADOTool\ADOMSProj\AzDOAddIn\AzDOAddIn\bin\Release\`

**2. You should see these files (and possibly more):**

- `AzDOAddIn.dll`
- `AzDOAddIn.dll.manifest`
- `AzDOAddIn.dll.config`
- `AzDOAddIn.Core.dll`
- `Newtonsoft.Json.dll`
- `CredentialManagement.dll`
- `AzDOAddIn.pdb` (optional)

**3. If the folder is missing or empty (or only has a few files):**

- The **AzDOAddIn** project did not build. That usually means:
  - You’re not in **Visual Studio** (e.g. you’re in Cursor/VS Code), or
  - The **AzDOAddIn** project is **(incompatible)** because the **Office/SharePoint development (VSTO)** workload is not installed.
- **Fix:** Use **Visual Studio 2019 or 2022** (full IDE) on the same machine, install the **Office/SharePoint development (VSTO)** workload via **Visual Studio Installer → Modify**, then open `AzDOAddIn.sln` and run **Build → Rebuild Solution**. Both **AzDOAddIn** and **AzDOAddIn.Core** must build; then `bin\Release\` will be filled.
- After that, run the Inno Setup installer again; it will copy from that folder.

---

## Install the add-in

### Option A — Inno Setup (recommended)

1. **Build the solution in Release** in Visual Studio first (see [Build from source](#build-from-source)). The installer copies from `AzDOAddIn\AzDOAddIn\bin\Release\`. If that folder is empty or missing, Inno Setup will report **"No files found matching"** — see [Verify build output](#verify-build-output--the-files-are-not-there) to fix this, then compile the script again.
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

If the tab **does not appear**, see [Add-in does not show in the ribbon](#add-in-does-not-show-in-the-ribbon) below.

---

## Uninstall

- **Inno Setup:** Use **Settings → Apps → AzDO MS Project Add-in → Uninstall** (or run the installer again and choose Remove).
- **Manual:** Delete the folder you copied the files to and remove the add-in registry keys under `HKCU\Software\Microsoft\Office\Project\Addins\`.

PATs stored in Windows Credential Manager (targets starting with `AzDOAddIn:`) are not removed by uninstall; you can delete them in **Control Panel → Credential Manager → Windows Credentials** if desired.

---

## Troubleshooting installation

**Add-in does not show in the ribbon**

1. **Manifest file:** The project now includes **AzDOAddIn.dll.manifest** (copied to `bin\Release` on build). Rebuild the solution in Visual Studio (Release) so `AzDOAddIn\AzDOAddIn\bin\Release\` contains `AzDOAddIn.dll.manifest` and all DLLs.
2. **Manifest registry value:** The installer writes a **Manifest** registry value (file URL to that manifest). Uninstall the add-in, recompile `installer\AzDOAddIn.iss`, run the new installer, then restart Project.
3. **Restart Project** after every install. If the add-in was disabled after a crash: File → Options → Add-ins → Manage **COM Add-ins** → Go → uncheck it if listed, restart Project, then reinstall.
4. **Confirm files in install folder:** After installing, check that `%APPDATA%\AzDOAddIn` (or the path you chose) contains `AzDOAddIn.dll`, `AzDOAddIn.dll.manifest`, `AzDOAddIn.Core.dll`, and the other dependencies. If any are missing, the build or installer copy step failed—rebuild and re-run the installer.

---

**"This program does not support the version of Windows your computer is running"**

- If you're on **Windows on ARM (Arm64)** (e.g. Snapdragon, Surface Pro X): Recompile the Inno Setup script—it now includes `arm64` in `ArchitecturesAllowed` so the installer runs on Arm64. Then run the newly built `AzDOAddInSetup.exe`.
- Otherwise: Right-click `AzDOAddInSetup.exe` → **Properties** → **Compatibility** tab → check **Run this program in compatibility mode for** and select **Windows 10**, then apply and run the installer again.
- If this appears when **starting Microsoft Project** (after the add-in is installed): The add-in may need to run in the same bitness as Project (e.g. 64-bit). Ensure you built the solution as **Any CPU** in Visual Studio and that you're using a 64-bit Office/Project. If the problem continues, try running Project as administrator once, or use **Compatibility** settings for `winproj.exe` as above.

---

## Legacy installer (deprecated)

The solution includes **AzDOAddInSetup** (`.vdproj`), a Visual Studio Setup Project. This project type is deprecated and not supported in current Visual Studio. Use **Inno Setup** (Option A) or manual installation (Option B) instead.

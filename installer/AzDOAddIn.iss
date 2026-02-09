; Inno Setup script for AzDO Add-in (MS Project / Azure DevOps integration)
; Build the solution in Release first, then compile this script with Inno Setup.

#define MyAppName "AzDO MS Project Add-in"
#define MyAppVersion "1.0"
#define MyAppPublisher "AzDOMSProject"
#define MyAppURL "https://github.com/ashamrai/AzDOMSProject"
; Output from building AzDOAddIn.sln in Release (add-in project copies Core and NuGet refs here)
#define BuildOutput "..\AzDOAddIn\AzDOAddIn\bin\Release"

[Setup]
AppId={{B7E5C401-1D9B-4663-A00E-50F367FDACEE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName={userappdata}\AzDOAddIn
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=AzDOAddInSetup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x86 x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Add-in, Core, and all dependency DLLs from build output
Source: "{#BuildOutput}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Registry]
; Register add-in for MS Project (per user). LoadBehavior 3 = load at startup.
; If your build produces a .dll.manifest, set Manifest to that path (file:/// URL).
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: string; ValueName: "Description"; ValueData: "Azure DevOps Work Items integration"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: string; ValueName: "FriendlyName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: dword; ValueName: "LoadBehavior"; ValueData: 3; Flags: uninsdeletekey

[UninstallDelete]
Type: dirifempty; Name: "{app}"

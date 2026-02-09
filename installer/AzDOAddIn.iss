; Inno Setup script for AzDO Add-in (MS Project / Azure DevOps integration)
;
; REQUIRED: Build the solution in Visual Studio (Release | Any CPU) first.
; Output must exist at: AzDOAddIn\AzDOAddIn\bin\Release\
; If you compile this script before building, you will get "No files found matching" at line 34.

#define MyAppName "AzDO MS Project Add-in"
#define MyAppVersion "1.0"
#define MyAppPublisher "AzDOMSProject"
#define MyAppURL "https://github.com/ashamrai/AzDOMSProject"
; Path relative to this script: repo\AzDOAddIn\AzDOAddIn\bin\Release
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
; Allow x86, x64, and Arm64 (e.g. Windows on ARM / Snapdragon) so installer runs on all supported systems
ArchitecturesAllowed=x86 x64 arm64
ArchitecturesInstallIn64BitMode=x64 arm64
; Require Windows 10 or later
MinVersion=10.0

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Add-in, Core, and all dependency DLLs from build output (must exist; build solution in Release first)
Source: "{#BuildOutput}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Registry]
; Register add-in for MS Project (per user). LoadBehavior 3 = load at startup.
; Manifest value is set in [Code] so we can use a proper file:/// URL.
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: string; ValueName: "Description"; ValueData: "Azure DevOps Work Items integration"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: string; ValueName: "FriendlyName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Office\Project\Addins\AzDOAddIn"; ValueType: dword; ValueName: "LoadBehavior"; ValueData: 3; Flags: uninsdeletekey

[Code]
const
  AddInRegKey = 'Software\Microsoft\Office\Project\Addins\AzDOAddIn';

procedure CurStepChanged(CurStep: TSetupStep);
var
  AppDir, ManifestUrl: string;
begin
  if CurStep = ssPostInstall then
  begin
    AppDir := ExpandConstant('{app}');
    { Build file:/// URL: forward slashes, no backslashes }
    ManifestUrl := 'file:///' + AppDir + '/AzDOAddIn.dll.manifest';
    StringChangeEx(ManifestUrl, '\', '/', True);
    RegWriteStringValue(HKEY_CURRENT_USER, AddInRegKey, 'Manifest', ManifestUrl);
  end;
end;

[UninstallDelete]
Type: dirifempty; Name: "{app}"

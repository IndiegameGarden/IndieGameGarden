; Script generated by the Inno Setup Script Wizard.
; (Then extensively modified by Caliban Darklock.)
; (And yet again modified by David Amador) Should work property with XNA 3.1
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
 
; Enter the name of your game here
#define MyAppName "Vizati"
 
; Enter the name of your game and a version number here
#define MyAppVerName "Vizati"
 
; Enter the name of your company, or just your name
#define MyCompany "Different Pixel"
 
; Enter the URL of your website
#define MyAppURL "http://vizati.differentpixel.com/"
 
; Enter the path to your game project - check Visual Studio properties for the path
#define MyAppLocation "D:\Projects\Vizati\Vizati"
 
; Enter the name of your game executable
#define MyAppExeName "Vizati.exe"
 
; Enter the location where XNA Game Studio is installed
#define MyGameStudioLocation "C:\Program Files (x86)\Microsoft XNA\XNA Game Studio\v3.1"
 
; Enter the name for the correct version of the XNA Framework MSI
#define XNARedist "xnafx31_redist.msi"
 
; Enter the location where you have placed the VC and .NET redistributables
#define MyRedistLocation "D:\Game Development Tools"
 
; search microsoft.com for "visual c++ sp1 redistributable" to get the VC redist
; enter the name of the executable file here
#define VCRedist "vcredist_x86.exe"
 
; Download latest .NET from http://www.microsoft.com/net/ (download button on menu)
; enter the name of the executable file here
#define DotNetSetup "DotNetFX35Setup.exe"
 
; Once you've filled in all the variables above and downloaded your redist packages,
; everything under this point should JUST WORK for most XNA projects.
 
[Setup]
AppName={#MyAppName}
AppVerName={#MyAppVerName}
AppPublisher={#MyCompany}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename={#MyAppName}Setup
Compression=lzma
SolidCompression=yes
SetupIconFile = "D:\Projects\Vizati\Vizati\vizati.ico"
UninstallIconFile = "D:\Projects\Vizati\Vizati\vizati.ico"
 
[Languages]
Name: english; MessagesFile: compiler:Default.isl
 
[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
 
[Files]
; DirectX and XNA Framework redistributables
Source: {#MyGameStudioLocation}\Redist\DX Redist\*; DestDir: {tmp}
Source: {#MyGameStudioLocation}\Redist\XNA FX Redist\{#XNARedist}; DestDir: {tmp}
 
; .NET and VC redistributables - VerifyDotNet35 MUST run BEFORE VerifyDotNet35sp1!
Source: {#MyRedistLocation}\{#DotNetSetup}; DestDir: {tmp}; AfterInstall: VerifyDotNet35
Source: {#MyRedistLocation}\{#VCRedist}; DestDir: {tmp}; AfterInstall: VerifyDotNet35sp1
 
; The game itself
Source: {#MyAppLocation}\bin\x86\Release\{#MyAppExeName}; DestDir: {app}; Flags: ignoreversion
Source: {#MyAppLocation}\bin\x86\Release\*; DestDir: {app}; Flags: ignoreversion recursesubdirs createallsubdirs
 
[Icons]
Name: {group}\{#MyAppName}; Filename: {app}\{#MyAppExeName}
Name: {group}\{cm:UninstallProgram,{#MyAppName}}; Filename: {uninstallexe}
Name: {commondesktop}\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: desktopicon
 
[Run]
Filename: {tmp}\{#DotNetSetup}; Flags: skipifdoesntexist; Parameters: "/q /noreboot"
Filename: {tmp}\{#VCRedist}; Flags: skipifdoesntexist; Parameters: "/q"
Filename: {tmp}\dxsetup.exe; Parameters: /silent
Filename: msiexec.exe; Parameters: "/quiet /i ""{tmp}\{#XNARedist}"""
Filename: {app}\{#MyAppExeName}; Description: {cm:LaunchProgram,{#MyAppName}}; Flags: nowait postinstall skipifsilent
 
; The code section doesn't like comments for some reason.
; VerifyDotNet35 removes the .NET setup if you already have .NET 3.5 installed.
; VerifyDotNet35sp1 removes the VC redist if you already have .NET 3.5 SP1, -or-
; if you don't have .NET 3.0 at all (it will be installed along with .NET 3.5).
; Using the skipifdoesntexist flag allows the setup to ignore the missing files.
[Code]
var
  hasDotNet3 :Boolean;
  hasDotNet3sp :Boolean;
  whichDotNet3sp :Cardinal;
 
procedure VerifyDotNet35();
begin
  hasDotNet3 := RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5');
	if hasDotNet3 then
      DeleteFile(ExpandConstant('{tmp}\{#DotNetSetup}'));
end;
 
procedure VerifyDotNet35sp1();
begin
  hasDotNet3sp := RegQueryDWordValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'SP', whichDotNet3sp);
  if (hasDotNet3sp and (whichDotNet3sp > 0)) or not hasDotNet3 then
      DeleteFile(ExpandConstant('{tmp}\{#VCRedist}'));
end;
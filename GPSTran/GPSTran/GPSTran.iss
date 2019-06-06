; -- Languages.iss --
; Demonstrates a multilingual installation.
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！



[Setup]
AppName={cm:MyAppName}
AppId={{F2F574D3-53F8-4034-B574-543CF5FC35C5}
AppVerName={cm:MyAppVerName}
DefaultDirName={pf}\Eastcom\GPSTransfer
DefaultGroupName=Eastcom
;UninstallDisplayIcon={app}\MyProg.exe
VersionInfoDescription=Eastcom GPSTran
VersionInfoProductName=GPSTran
OutputDir=userdocs:Inno Setup Examples Output
ShowUndisplayableLanguages=yes
AppCopyright=Copyright (C) Eastcom
;Application Source root Path
SourceDir=".\bin\Release\"
;显示语言选择框

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinesesimp"; MessagesFile: "compiler:Languages\Chinese.isl"




[Messages]
english.BeveledLabel=English
chinesesimp.BeveledLabel=Chinese

[CustomMessages]
english.MyDescription=GPSTran
english.MyAppName=GPSTran
english.MyAppVerName=GPSTran


chinesesimp.MyDescription=GPS转发
chinesesimp.MyAppName=GPS转发
chinesesimp.MyAppVerName=GPS转发


[Files]
Source: "GPSTran.exe"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "help_doc\*"; DestDir: "{app}\GPSTran\help_doc"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Image\*"; DestDir: "{app}\GPSTran\Image"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "language\*"; DestDir: "{app}\GPSTran\language"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Common.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "GPSTran.exe.config"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "Log4Net.config"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "log4net.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "NeiMengRemoteIP.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "NeiMeng_UDP_2.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "HangZhou_UDP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "HangZhou_UDP_2.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "SiChuan_TCP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "LiaoNing_TCP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "LiaoNing_UDP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "HuaiBei_TCP_0.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "ShaoXingPGIS.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "HangZhou_DB_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "HangZhou_DB_2.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "GuangDong_DB_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "TaiZhouPGIS2.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "TaiZhouPGIS3.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "Lte_UDP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "ShanXi_UDP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "ShiJiaZhuangUDP9.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "NewUDP_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "GuangDong_DB_2.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "FieldStrength_DB_1.dll"; DestDir: "{app}\GPSTran"; Flags: ignoreversion
Source: "Tran_en.config"; DestDir: "{app}\GPSTran"; DestName:"Tran.config" ; Languages: english; Flags: ignoreversion
Source: "Tran_zh.config"; DestDir: "{app}\GPSTran"; DestName:"Tran.config" ; Languages: chinesesimp;Flags: ignoreversion

[Tasks]
;快速启动栏
Name: "QuickLaunchIcon"; Description: "创建快速启动" ; Languages: chinesesimp;

Name: "QuickLaunchIcon"; Description:"Quick launch"; Languages: english;

;桌面快捷方式

Name: "DesktopIcon"; Description: "桌面快捷方式" ; Languages: chinesesimp;

Name: "DesktopIcon"; Description:"Desktop shutcut"; Languages: english;



;Name: "{userdesktop}\GPS转发";Filename: "{app}\GPSTran\GPSTran.exe"; WorkingDir: "{app}";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Languages: chinesesimp;

;Name: "{userdesktop}\GPSTran";Filename: "{app}\GPSTran\GPSTran.exe"; WorkingDir: "{app}";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Languages: english;

;[Run]
;Filename: "{app}\GPSTran\GPSTran.exe"; Description: "{cm:LaunchProgram,{#StringChange("GPSTran", "&", "&&")}}"; Flags: nowait postinstall skipifsilent  ;   Languages: english; Parameters:"-e"   ;
;定义安装完成后是否直接运行的程序路径



[Icons]
;在开始菜单出现的程序菜单
;中文版
Name: "{group}\GPS转发\GPS转发"; Filename: "{app}\GPSTran\GPSTran.exe";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Tasks: QuickLaunchIcon ;  Languages: chinesesimp;

;英文版
Name: "{group}\GPSTran\GPSTran"; Filename: "{app}\GPSTran\GPSTran.exe";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Tasks: QuickLaunchIcon ; Languages: english;


;桌面快捷方式
;中文版
Name: "{userdesktop}\GPS转发";Filename: "{app}\GPSTran\GPSTran.exe"; WorkingDir: "{app}";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Tasks: DesktopIcon ; Languages: chinesesimp;

;英文版
Name: "{userdesktop}\GPSTran";Filename: "{app}\GPSTran\GPSTran.exe"; WorkingDir: "{app}";IconFilename:"{app}\GPSTran\Image\Icon.ico"; Tasks: DesktopIcon ; Languages: english;



;Name: "{group}\{cm:MyAppName}"; Filename: "{app}\GPS转发.exe";  Languages: chinesesimp;
;Name: "{group}\{cm:MyAppName}"; Filename: "{app}\GPSTran.exe";  Languages: english; Parameters:"-e"   ;
;英文版本是通过传递 -e 的参数实现的
Name: "{group}\GPS转发\卸载GPS转发"; Filename: "{uninstallexe}" ; Languages: chinesesimp;
Name: "{group}\GPSTran\unintall"; Filename: "{uninstallexe}" ; Languages: english;
;卸载程序的路径

[Registry]
;HKEY_LOCAL_MACHINE
Root: HKLM; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "Language"; ValueData: "zh_cn"; Languages: chinesesimp; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "Language"; ValueData: "en_us"; Languages: english; Flags: uninsdeletekey
;HKEY_CURRENT_USER
Root: HKCU; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}" ; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "Language"; ValueData: "zh_cn";  Flags: uninsdeletekey; Languages: chinesesimp
Root: HKCU; Subkey: "Software\Eastcom\GPSTran"; ValueType: string; ValueName: "Language"; ValueData: "en_us";  Flags: uninsdeletekey; Languages: english
[Code]

function  InitializeSetup: Boolean;
var
ResultStr:string;
var
HasRun1:HWND;
var
HasRun2:HWND;
begin
   result:= true;

   HasRun1:= FindWindowByWindowName('安装程序 - GPS转发');

   HasRun2:=FindWindowByWindowName('Setup - GPSTran')
   if HasRun1<>0 or HasRun2 then
   begin
      result:=false;
      if ActiveLanguage='english'
      then
      begin
      MsgBox('Install program is running！',mbError, MB_OK);

      end
      else
      begin
      MsgBox('监测到安装程序正在运行，请等待安装程序安装结束！',mbError, MB_OK);
      end

   end






if RegValueExists(HKLM, 'SOFTWARE\Eastcom\GPSTran','InstallPath') and result=true then
begin
   result:= false;

   if ActiveLanguage='english' then
   begin

   MsgBox('Program has been installed！',mbError, MB_OK);

   end
   else
   begin

   MsgBox('程序已经安装，请勿重新安装！',mbError, MB_OK);

   end

end;



end;

function InitializeUninstall(): Boolean;
var
ResultStr:string;
var
HasRun:HWND;
begin

HasRun := FindWindowByWindowName('GPSTran');

if HasRun<>0 then
begin

  if RegQueryStringValue(HKLM, 'SOFTWARE\Eastcom\GPSTran','Language',ResultStr) then
  begin

    if ResultStr='zh_cn' then
    begin
      MsgBox('卸载程序检测到你的应用程序正在运行。' #13#13 '请先退出你的应用程序，然后再进行卸载！', mbError, MB_OK);

      Result := false;

    end
    else
    begin
      MsgBox('Detect Program is running。' #13#13 'Please exit，then do uninstalling！', mbError, MB_OK);

      Result := false;

    end

  end
  else
  begin

      Result:=true;
  end


end

else
    begin
    Result := true;
    end
end;



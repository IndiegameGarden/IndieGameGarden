@echo off
rem Make a release version of new IGG client based on latest Release build
rem Files are copied into the 'distribution' folder, then zipped.
rem This bat file requires Cygwin install.

call set-versions.bat

call build-gamelib.bat

echo Copying for release/distribution version %CLVER% of IGG with gamelib version %LIBVER%
copy /Y config\gamelib_fmt3\gamelib-config.json distribution\IndiegameGarden_data\config
copy /Y config\gamelib_fmt3\gamelib.zip distribution\IndiegameGarden_data\zips\igg_gamelib_fmt3_v%LIBVER%.zip
xcopy /S /D /Y IndiegameGarden\config\igg\*.exe distribution\IndiegameGarden_data\config\igg\
xcopy /S /D /Y IndiegameGarden\config\igg\*.dll distribution\IndiegameGarden_data\config\igg\
xcopy /S /D /Y IndiegameGarden\config\igg\Content distribution\IndiegameGarden_data\config\igg\Content\

echo Creating final distro zips
rm -f installers/IndiegameGarden_Beta-%CLVER%.zip
rm -f installers/igg_v%CLVER%.zip
cd distribution
7z a -tzip -r ../installers/IndiegameGarden_Beta-%CLVER%.zip *
cd IndiegameGarden_data\config\igg
7z a -tzip -r ../../../../installers/igg_v%CLVER%.zip *
cd ..\..\..\..
copy /Y installers\IndiegameGarden_Beta-%CLVER%.zip installers\IndiegameGarden.zip
echo Release files zipped, showing below.
dir installers\*.zip

rem echo Uploading release zips to server
rem cd installers
rem cat ../ftp-release.script | sed s/\$VER/%CLVER%/ > ftp-release-ver.script
rem ftp -n -s:ftp-release-ver.script
rem cd ..


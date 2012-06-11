@echo off
rem Make a release version of new IGG client based on latest Release build
rem Files are copied into the 'distribution' folder, then zipped.
rem This bat file requires Cygwin install.

rem ClientVer version number of to be released IGG client
set VER=4

call build-gamelib.bat

echo Copying for release/distribution version %VER% of IGG
copy /Y config\gamelib_fmt3\gamelib-config.json distribution\IndiegameGarden_data\config
copy /Y config\gamelib_fmt3\gamelib.zip distribution\IndiegameGarden_data\zips\igg_gamelib_fmt3.zip
xcopy /S /D /Y IndiegameGarden\IndiegameGarden\bin\x86\Release\*.exe distribution\IndiegameGarden_data\config\igg\
xcopy /S /D /Y IndiegameGarden\IndiegameGarden\bin\x86\Release\*.dll distribution\IndiegameGarden_data\config\igg\
xcopy /S /D /Y IndiegameGarden\IndiegameGarden\bin\x86\Release\Content distribution\IndiegameGarden_data\config\igg\Content\

echo Creating final distro zips
rm -f installers/IndiegameGarden_Alpha-%VER%.zip
rm -f installers/igg_v%VER%.zip
cd distribution
7z a -tzip -r ../installers/IndiegameGarden_Alpha-%VER%.zip *
cd IndiegameGarden_data\config\igg
7z a -tzip -r ../../../../installers/igg_v%VER%.zip *
echo Release files zipped, showing below.
cd ..\..\..\..
dir installers\*.zip

echo Uploading release zips to server
cd installers
cat ../ftp-release.script | sed s/\$VER/%VER%/ > ftp-release-ver.script
ftp -n -s:ftp-release-ver.script
cd ..


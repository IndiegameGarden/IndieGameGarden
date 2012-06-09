@echo off

rem Version number of to be released IGG client
set VER=4

echo Making release/distribution version %VER% of IGG
cd config\gamelib_fmt3
rm -f gamelib.zip
7z a -tzip gamelib.zip gamelib.bin
cd ..\..
copy config\gamelib_fmt3\gamelib-config.json distribution\IndiegameGarden_data\config
copy config\gamelib_fmt3\gamelib.zip distribution\IndiegameGarden_data\zips\igg_gamelib_fmt3.zip
xcopy /S /D IndiegameGarden\IndiegameGarden\bin\x86\Release\*.exe distribution\IndiegameGarden_data\config\igg\
xcopy /S /D IndiegameGarden\IndiegameGarden\bin\x86\Release\*.dll distribution\IndiegameGarden_data\config\igg\
xcopy /S /D IndiegameGarden\IndiegameGarden\bin\x86\Release\Content distribution\IndiegameGarden_data\config\igg\Content\

rem final distro zips
rm -f installers/IndiegameGarden_Alpha-%VER%.zip
rm -f installers/igg_v%VER%.zip
cd distribution
7z a -tzip -r ../installers/IndiegameGarden_Alpha-%VER%.zip *
cd IndiegameGarden_data\config\igg
7z a -tzip -r ../../../../installers/igg_v%VER%.zip *
echo Release files done.
cd ..\..\..\..
dir installers\*.zip
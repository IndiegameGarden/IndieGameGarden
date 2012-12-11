@echo off
rem Release new GameLib to the server
rem First builds it.

call build-gamelib.bat

echo Zipping gamelib.bin
cd config\gamelib_fmt%FORMATVER%
rm -f gamelib.zip
7z a -tzip gamelib.zip gamelib.bin
cd ..\..

echo Don't forget to increase GameLibVer in gamelib-config.json
echo and commit changes to repository to release.


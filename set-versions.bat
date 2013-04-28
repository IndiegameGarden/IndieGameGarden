@echo off
rem Sets the version numbers for building and releasing new IGG clients and game libraries

rem IGG Client version
set CLVER=7

rem IGG game library version
set LIBVER=1

rem IGG game library format id
set FORMATVER=4

echo set-versions.bat info:
echo         CLVER=%CLVER%
echo         LIBVER=%LIBVER%
echo         FORMATVER=%FORMATVER%
set GCONF=IndiegameGarden/IndiegameGarden/Base/GardenConfig.cs
set LIBCONF=config/gamelib_fmt%FORMATVER%/gwg-config.json
echo         GCONF=%GCONF%
echo         LIBCONF=%LIBCONF%
echo -
echo GardenConfig.cs info:
grep -i 'IGG_CLIENT_VERSION=' %GCONF%
grep -i 'KNOWN_GAMELIB_VERSION=' %GCONF%
grep -i 'IS_INSTALLER_VERSION=' %GCONF%
echo gamelib-config.json info (ClientVer is typically one behind):
grep -i 'GameLibVer' %LIBCONF%
grep -i 'ClientVer' %LIBCONF%
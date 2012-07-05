@echo off
rem Sets the version numbers for building and releasing new IGG clients and game libraries

rem IGG Client version
set CLVER=6

rem IGG game library version
set LIBVER=2

echo set-versions.bat info
echo         CLVER=%CLVER%
echo         LIBVER=%LIBVER%
echo GardenConfig.cs info
set GCONF=IndiegameGarden/IndiegameGarden/Base/GardenConfig.cs
grep -i 'IGG_CLIENT_VERSION=' %GCONF%
grep -i 'KNOWN_GAMELIB_VERSION=' %GCONF%
grep -i 'IS_INSTALLER_VERSION=' %GCONF%
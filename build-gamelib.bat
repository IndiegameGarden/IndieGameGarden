@echo off
rem Builds the game library .bin from gamelib.json and copies to locations
rem Gamelib becomes directly usable for local (dev) testing
rem

call set-versions.bat

cd IndiegameGarden\GameLibCompiler\bin\Release\
GameLibCompiler.exe
cd ..\..\..\..

echo Copy gamelib.bin to unpacked location for local playtesting
mkdir IndiegameGarden\config\igg_gamelib_fmt4_v%LIBVER%
copy config\gamelib_fmt4\gamelib.bin IndiegameGarden\config\igg_gamelib_fmt4_v%LIBVER%\gamelib.bin

echo Copy new gamelib.bin to IGG client Content folder
copy config\gamelib_fmt4\gamelib.bin IndiegameGarden\IndiegameGardenContent\gamelib.bin

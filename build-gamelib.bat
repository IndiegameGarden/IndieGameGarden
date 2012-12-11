@echo off
rem Builds the game library .bin from gamelib.json and copies to locations
rem Gamelib becomes directly usable for local (dev) testing
rem

call set-versions.bat

cd IndiegameGarden\GameLibCompiler\bin\Release\
GameLibCompiler.exe
cd ..\..\..\..

echo Copy new gamelib_fmt%FORMATVER%\gamelib.bin to IGG client Content folder
copy config\gamelib_fmt%FORMATVER%\gamelib.bin IndiegameGarden\IndiegameGardenContent\gamelib.bin

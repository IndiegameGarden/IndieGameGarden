@echo off
rem Builds the game library .bin from gamelib.json and copies to locations
rem Gamelib becomes directly usable for local (dev) testing
rem

cd IndiegameGarden\GameLibCompiler\bin\Release\
GameLibCompiler.exe
cd ..\..\..\..

echo Copy gamelib.bin to unpacked location for local playtesting
mkdir config\igg_gamelib_fmt3
copy config\gamelib_fmt3\gamelib.bin config\igg_gamelib_fmt3_v3\gamelib.bin

echo Copy new gamelib.bin to IGG client Content folder
copy config\gamelib_fmt3\gamelib.bin IndiegameGarden\IndiegameGardenContent\gamelib.bin

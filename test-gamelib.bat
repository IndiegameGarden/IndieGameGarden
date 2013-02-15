@echo off
rem Puts .bin file in IGG folder for local test
rem First builds it.

call build-gamelib.bat

cd config\gamelib_fmt%FORMATVER%
copy /Y gamelib.bin "%USERPROFILE%\Local Settings\Application Data\IndiegameGarden\config\gwg_gamelib_fmt%FORMATVER%\gamelib.bin"
cd ..\..

echo Ready for testing with local Indiegame Garden


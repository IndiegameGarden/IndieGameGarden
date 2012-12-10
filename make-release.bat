@echo off
rem Make a release version of new IGG client based on latest Release build
rem Files are copied into the 'distribution' folder
rem This bat file requires Cygwin install.

call set-versions.bat

call build-gamelib.bat

echo Copying for release/distribution version %CLVER% of IGG with gamelib version %LIBVER%
xcopy /D /Y IndiegameGarden\*.exe distribution\
xcopy /D /Y IndiegameGarden\*.dll distribution\
xcopy /D /Y IndiegameGarden\Content distribution\Content\

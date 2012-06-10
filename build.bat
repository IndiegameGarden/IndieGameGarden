@echo off
c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /t:Rebuild /property:Configuration=Release IndiegameGarden.sln
cd IndiegameGarden\GameLibCompiler\bin\Release\
GameLibCompiler.exe
cd ..\..\..\..


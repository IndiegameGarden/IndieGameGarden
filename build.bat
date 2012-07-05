@echo off
call set-versions.bat
c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild /t:Rebuild /property:Configuration=Release IndiegameGarden.sln


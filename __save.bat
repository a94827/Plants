if exist src.7z del src.7z
7za.exe a -r src.7z *.sln *.cs *.resx *.csproj *.user *.ico *.png *.bat app.config
7za.exe a src.7z History.htm

if exist exe.7z del exe.7z
cd BIN\Release
7za.exe a -r ..\..\exe.7z Plants.exe *.dll
cd ..\..


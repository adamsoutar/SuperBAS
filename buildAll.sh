mkdir -p ./releases

cd SuperBAS

# Keep the spec for each release
cp ../Docs/LanguageSpec.md ../releases/LanguageSpec.md

# Mac
dotnet publish -c Release --self-contained -r osx-x64 /p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/osx-x64/publish/SuperBAS ../releases/SuperBAS-macOS-x64

# Linux
dotnet publish -c Release --self-contained -r linux-x64 /p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/linux-x64/publish/SuperBAS ../releases/SuperBAS-linux-x64

# Windows
# 64
dotnet publish -c Release --self-contained -r win-x64 /p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/win-x64/publish/SuperBAS.exe ../releases/SuperBAS-x64.exe
# 32
dotnet publish -c Release --self-contained -r win-x86 /p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/win-x86/publish/SuperBAS.exe ../releases/SuperBAS-x86.exe

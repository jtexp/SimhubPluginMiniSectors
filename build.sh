#!/bin/bash
# Build script for MiniSectors plugin - runs Windows MSBuild from WSL

MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
PROJECT="User.PluginMiniSectors.csproj"
CONFIG="${1:-Debug}"

echo "Building $PROJECT ($CONFIG)..."

powershell.exe -Command "
    \$env:SIMHUB_INSTALL_PATH = [System.Environment]::GetEnvironmentVariable('SIMHUB_INSTALL_PATH', 'User');
    & '$MSBUILD_PATH' $PROJECT /p:Configuration=$CONFIG
"

exit $?

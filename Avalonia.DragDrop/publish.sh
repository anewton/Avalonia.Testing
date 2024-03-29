#!/usr/bin/env bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1

SOURCE="${BASH_SOURCE[0]}"
ROOT_DIR="$(cd -P "$(dirname "$SOURCE")"&&pwd)/"

cd "${ROOT_DIR}"
cd ./ReleaseVersion/ReleaseVersion.Desktop
dotnet build -c Release
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=false -p:IncludeNativeLibrariesForSelfExtract=true -p:CopyOutputSymbolsToPublishDirectory=false -r win-x64 --self-contained=true -o "${ROOT_DIR}"/releases/ReleaseVersion/win-x64

cd "${ROOT_DIR}"
cd ./WorkingVersion/WorkingVersion.Desktop
dotnet build -c Release
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=false -p:IncludeNativeLibrariesForSelfExtract=true -p:CopyOutputSymbolsToPublishDirectory=false -r win-x64 --self-contained=true -o "${ROOT_DIR}"/releases/WorkingVersion/win-x64

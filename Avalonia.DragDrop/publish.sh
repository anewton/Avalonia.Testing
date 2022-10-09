#!/usr/bin/env bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1

SOURCE="${BASH_SOURCE[0]}"
ROOT_DIR="$(cd -P "$(dirname "$SOURCE")"&&pwd)/"

cd "${ROOT_DIR}"
cd ./StableVersion/StableVersion.Desktop
dotnet build -c Release
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -p:CopyOutputSymbolsToPublishDirectory=false -r win-x64 --self-contained=true -o "${ROOT_DIR}"/releases/StableVersion/win-x64

cd "${ROOT_DIR}"
cd ./PreviewVersion/PreviewVersion.Desktop
dotnet build -c Release
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -p:CopyOutputSymbolsToPublishDirectory=false -r win-x64 --self-contained=true -o "${ROOT_DIR}"/releases/PreviewVersion/win-x64

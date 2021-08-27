#!/bin/bash

set -e

# Restore packages
export HOME="/tmp"
nuget sources update -Name "BAMS (AWS)" -username ${BAMS_USER} -password ${BAMS_PASS} -ConfigFile NuGet.config -StorePasswordInClearText
nuget restore -ConfigFile NuGet.config

dotnet test --configuration Release --logger 'nunit;LogFilePath="../output/TestResults/UnitTestResults.xml' --filter 'Category!=Integration'

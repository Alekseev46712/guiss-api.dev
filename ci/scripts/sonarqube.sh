#!/bin/bash

set -e

# Restore packages
export HOME="/tmp"
nuget sources update -Name "BAMS (AWS)" -username ${BAMS_USER} -password ${BAMS_PASS} -ConfigFile NuGet.config -StorePasswordInClearText
nuget restore -ConfigFile NuGet.config

dotnet /sonar-scanner/SonarScanner.MSBuild.dll begin \
  /k:${SONAR_PROJECT_KEY} \
  /d:sonar.host.url="https://sonarqube.refinitiv.com" \
  /d:sonar.login=${SONAR_TOKEN} \
  /d:sonar.cs.vstest.reportsPaths="**/TestResults/*.trx" \
  /d:sonar.cs.opencover.reportsPaths="**/CoverageResults/coverage.opencover.xml"
dotnet restore
dotnet build --configuration Release
dotnet test \
  /p:CollectCoverage=true \
  /p:CoverletOutput=CoverageResults/ \
  /p:CoverletOutputFormat=opencover --configuration Release -l:trx \
  /p:LogFileName=TestResults.trx --filter 'TestCategory!=Integration' --no-build -maxcpucount:1 \
  /p:exclude=[*]*Startup
dotnet /sonar-scanner/SonarScanner.MSBuild.dll end /d:sonar.login=${SONAR_TOKEN}

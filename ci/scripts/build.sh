#!/bin/bash

set -e

# Checkout
git checkout ${GIT_BRANCH}

# Restore packages 
export HOME="/tmp"
nuget sources update -Name "BAMS (AWS)" -username ${BAMS_USER} -password ${BAMS_PASS} -ConfigFile NuGet.config -StorePasswordInClearText
nuget restore -ConfigFile NuGet.config
dotnet tool install --global Amazon.Lambda.Tools --version 4.0.0

# Set subdir
case ${ENV} in
  dev)
    BUILD_SUBDIR="snapshot"
  ;;
  *)
    echo "Error: Wrong ENV variable"
    exit 1
  ;;
esac

# Get version
GIT_TAG=$(git for-each-ref refs/tags/snapshot refs/tags/develop --sort=-taggerdate --format='%(refname:short)' --count=1)
[ ! -z ${GIT_TAG} ] && VERSION=$(echo "${GIT_TAG##*/}"|awk -F. -v OFS=. '{$4=$4+1;print}') || VERSION="0.0.0.0"

# Build
mkdir -p ${ARTIFACT_NAME}-${VERSION}
dotnet /tmp/.dotnet/tools/.store/amazon.lambda.tools/4.0.0/amazon.lambda.tools/4.0.0/tools/netcoreapp2.1/any/dotnet-lambda.dll package \
  /p:Version=0.0.0.0 \
  --configuration Release \
  -o output/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}.zip \
  -pl ${PROJECT_DIR}

cd ci
zip -r ../output/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}-terraform.zip terraform

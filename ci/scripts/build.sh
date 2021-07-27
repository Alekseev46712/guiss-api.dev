#!/bin/bash

set -e

# Restore packages 
export HOME="/tmp"
nuget sources update -Name "BAMS (AWS)" -username ${BAMS_USER} -password ${BAMS_PASS} -ConfigFile NuGet.config -StorePasswordInClearText
nuget restore -ConfigFile NuGet.config
dotnet tool install --global Amazon.Lambda.Tools --version 4.0.0

# Set subdir
case ${GIT_BRANCH} in
  master)
    BUILD_SUBDIR="release"
  ;;
  dev)
    BUILD_SUBDIR="develop"
  ;;
  *)
    BUILD_SUBDIR="snapshot"
  ;;
esac

# Get version
echo "${GIT_SSHKEY}" > ~/id_rsa
chmod -R 700 ~/id_rsa
git config user.email "eduard.kotoyants@refinitiv.com"
git config user.name "eduard.kotoyants_rft"
git config core.sshCommand 'ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -i ~/id_rsa'
git pull --tags
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


# Add tag
echo "${GIT_SSHKEY}" > ~/id_rsa
chmod -R 700 ~/id_rsa
git config user.email "eduard.kotoyants@refinitiv.com"
git config user.name "eduard.kotoyants_rft"
git config core.sshCommand 'ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -i ~/id_rsa'
git tag -a ${BUILD_SUBDIR}/${VERSION} -m "Concourse auto tagging"
git push --tags

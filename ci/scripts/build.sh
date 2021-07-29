#!/bin/bash

set -e
set -x

# Git config
export HOME="/tmp"
echo "${GIT_SSHKEY}" > ~/id_rsa
chmod -R 700 ~/id_rsa
export GIT_SSH_COMMAND='ssh -i ~/id_rsa -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null'

# Set subdir
case ${GIT_BRANCH} in
  master)
    BUILD_SUBDIR="release"
  ;;
  dev | develop | development)
    BUILD_SUBDIR="develop"
  ;;
  *)
    BUILD_SUBDIR="snapshot"
    GIT_MR_SOURCE=$(cat .git/merge-request.json | jq -r '.source_branch')
    GIT_MR_SHA=$(cat .git/merge-request.json | jq -r '.sha')
  ;;
esac

# Set version
case ${GIT_BRANCH} in
  master)
    VERSION=$(cat BuildNumber.txt)
  ;;
  dev | develop | development)
    git fetch --tags
    GIT_TAG=$(git for-each-ref refs/tags/snapshot refs/tags/develop --sort=-taggerdate --format='%(refname:short)' --count=1)
    [ ! -z ${GIT_TAG} ] && VERSION=$(echo "${GIT_TAG##*/}"|awk -F. -v OFS=. '{$4=$4+1;print}') || VERSION="0.0.0.0"
  ;;
  *)
    mkdir set_tag; cd set_tag
    git clone -b ${GIT_MR_SOURCE} ssh://git@${GIT_REPO} .
    git reset --hard ${GIT_MR_SHA}
    GIT_TAG=$(git for-each-ref refs/tags/snapshot refs/tags/develop --sort=-taggerdate --format='%(refname:short)' --count=1)
    [ ! -z ${GIT_TAG} ] && VERSION=$(echo "${GIT_TAG##*/}"|awk -F. -v OFS=. '{$4=$4+1;print}') || VERSION="0.0.0.0"
    cd ..
  ;;
esac

# Restore packages
nuget sources update -Name "BAMS (AWS)" -username ${BAMS_USER} -password ${BAMS_PASS} -ConfigFile NuGet.config -StorePasswordInClearText
nuget restore -ConfigFile NuGet.config
dotnet tool install --global Amazon.Lambda.Tools --version 4.0.0

# Build
mkdir -p ${ARTIFACT_NAME}-${VERSION}
dotnet /tmp/.dotnet/tools/.store/amazon.lambda.tools/4.0.0/amazon.lambda.tools/4.0.0/tools/netcoreapp2.1/any/dotnet-lambda.dll package \
  /p:Version=0.0.0.0 \
  --configuration Release \
  -o output/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}.zip \
  -pl ${PROJECT_DIR}

cd ci
zip -r ../output/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}-terraform.zip terraform
cd ..

# Add tag
if [ ${GIT_BRANCH} != 'master' -a ${GIT_BRANCH} != 'dev' -a ${GIT_BRANCH} != 'develop' -a ${GIT_BRANCH} != 'development' ]; then
  cd set_tag
fi

git config user.email "eduard.kotoyants@refinitiv.com"
git config user.name "eduard.kotoyants_rft"
git config core.sshCommand 'ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -i ~/id_rsa'
git tag -a ${BUILD_SUBDIR}/${VERSION} -m "Concourse auto tagging"
git push --tags

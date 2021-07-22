#!/bin/sh

set -e

apk -q update
apk -q --no-progress add curl

# Checkout
git checkout ${GIT_BRANCH}

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
GIT_TAG=$(git for-each-ref refs/tags/${BUILD_SUBDIR} --sort=-taggerdate --format='%(refname:short)' --count=1)
[ ! -z ${GIT_TAG} ] && VERSION=${GIT_TAG##*/} || VERSION="0.0.0.0"

# Download artifacts
curl -sSf -u "${BAMS_USER}:${BAMS_PASS}" ${BAMS_URL}/${BAMS_PATH}/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}.zip -O
curl -sSf -u "${BAMS_USER}:${BAMS_PASS}" ${BAMS_URL}/${BAMS_PATH}/${BAMS_DIR}/${BUILD_SUBDIR}/${ARTIFACT_NAME}-${VERSION}/${ARTIFACT_NAME}-${VERSION}-terraform.zip -O
unzip ${ARTIFACT_NAME}-${VERSION}-terraform.zip

# Terraform
cd terraform/${TERRAFORM_SUBDIR}
terraform init -input=false -backend-config="../environments/${ENV}/region/${AWS_REGION}/infra-backend.tfvars"
terraform plan -input=false -var-file="../environments/${ENV}/region/${AWS_REGION}/infra-env.tfvars" -var app_version_number=${VERSION} -out=tfplan
#terraform apply -no-color -input=false -auto-approve tfplan

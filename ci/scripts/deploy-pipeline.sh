#!/bin/sh

set -e

apk -q update
apk -q --no-progress add git openssh

# Get last tags
cd input
echo "${GIT_SSHKEY}" > ~/id_rsa
chmod -R 700 ~/id_rsa
git config user.email "eduard.kotoyants@refinitiv.com"
git config user.name "eduard.kotoyants_rft"
git config core.sshCommand 'ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -i ~/id_rsa'
git pull --tags
GIT_TAG=$(git for-each-ref refs/tags/snapshot refs/tags/develop --sort=-taggerdate --format='%(refname:short)' --count=5)
[ ! -z "${GIT_TAG}" ] || GIT_TAG="snapshot/0.0.0.0"

# Add groups
echo "groups:" >> ci/pipeline-deploy.yml
for VERSION in $GIT_TAG; do
  echo "- name: ${VERSION/\//-}
  jobs:" >> ci/pipeline-deploy.yml
  for ENV in dev qa; do
    for REGION in $AWS_REGIONS; do
      echo "    - deploy-${ENV}-${VERSION/\//-}-${REGION:0:2}${REGION:3:1}${REGION: -1}
    - destroy-${ENV}-${VERSION/\//-}-${REGION:0:2}${REGION:3:1}${REGION: -1}" >> ci/pipeline-deploy.yml
    done
  done
done

# Add jobs
echo "jobs:" >> ci/pipeline-deploy.yml
for ENV in dev qa; do
  for VERSION in $GIT_TAG; do
    for REGION in $AWS_REGIONS; do
      echo "- name: deploy-${ENV}-${VERSION/\//-}-${REGION:0:2}${REGION:3:1}${REGION: -1}
  plan:
  - get: repo
    resource: gitlab-develop
  - get: terraform
  - task: deploy
    image: terraform
    file: repo/ci/tasks/deploy.yml
    input_mapping: {input: repo}
    params:
      AWS_REGION: $REGION
      ARTIFACT_VERSION: $VERSION
      ENV: $ENV
      <<: *parameters" >> ci/pipeline-deploy.yml
    done

    for REGION in $AWS_REGIONS; do
      echo "- name: destroy-${ENV}-${VERSION/\//-}-${REGION:0:2}${REGION:3:1}${REGION: -1}
  plan:
  - get: repo
    resource: gitlab-develop
  - get: terraform
  - task: deploy
    image: terraform
    file: repo/ci/tasks/destroy.yml
    input_mapping: {input: repo}
    params:
      AWS_REGION: $REGION
      ARTIFACT_VERSION: $VERSION
      ENV: $ENV
      <<: *parameters" >> ci/pipeline-deploy.yml
    done
  done
done

# Copy files to output
cp ci/pipeline-deploy.yml ../output/pipeline-deploy.yml
cp ci/config.yml ../output/config.yml

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.301
 
# Dockerfile meta-information
LABEL maintainer="NOS Inovação S.A." \
    app_name="dotnet-sonar"
 
# Reviewing this choices
ENV SONAR_SCANNER_MSBUILD_VERSION=4.10.0.19059 \
    DOTNETCORE_SDK=3.1.301 \
    DOTNETCORE_RUNTIME=3.1.5 \
    DOCKER_VERSION=5:19.03.12~3-0~debian-buster \
    CONTAINERD_VERSION=1.2.13-2 \
    MSBUILDSINGLELOADCONTEXT=1
 
 # Install all necessary additional software (utils, jre, docker binaries, etc...)
RUN wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y \
        openjdk-11-jre \
        apt-transport-https \
        ca-certificates \
        curl \
        jq \
        gnupg-agent \
        software-properties-common \
        nuget \
        zip \
        unzip \
        autoconf \
        libtool \
        nasm \
        aspnetcore-runtime-3.1 -y \ 
        && wget https://github.com/SonarSource/sonar-scanner-msbuild/releases/download/$SONAR_SCANNER_MSBUILD_VERSION/sonar-scanner-msbuild-$SONAR_SCANNER_MSBUILD_VERSION-netcoreapp3.0.zip \
        && unzip sonar-scanner-msbuild-$SONAR_SCANNER_MSBUILD_VERSION-netcoreapp3.0.zip -d /sonar-scanner \
        && chmod +x -R /sonar-scanner \
        && rm -rf /var/lib/apt/lists/*

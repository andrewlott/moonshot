FROM gableroux/unity3d:2019.4.14f1
COPY . /project/

ARG UNITY_USERNAME
ARG UNITY_PASSWORD
ARG UNITY_SERIAL

ENV BUILD_NAME=moonshot-build
ENV BUILD_OPTIONS=EnableHeadlessMode
ENV BUILD_TARGET=StandaloneLinux64
ENV UNITY_DIR=/project/.

RUN bash -c /project/DockerScripts/build.sh

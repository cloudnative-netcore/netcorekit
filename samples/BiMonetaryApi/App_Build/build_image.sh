#!/bin/sh
set -ex

readonly TAG=${TAG:=$(git rev-parse --short HEAD)}
readonly NAMESPACE=${NAMESPACE:="vndg"}

echo "namespace: ${NAMESPACE} and tag: ${TAG}"
echo "start to build BiMonetaryApi..."

docker build -f samples/BiMonetaryApi/Dockerfile \
    -t $NAMESPACE/bimonetary-api:$TAG \
    -t $NAMESPACE/bimonetary-api:latest .

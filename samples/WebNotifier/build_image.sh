#!/bin/sh
set -ex

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}

echo "namespace: ${NAMESPACE} and tag: ${TAG}"
echo "start to build WebNotifier..."

docker build -f samples/WebNotifier/Dockerfile \
    -t vndg/webnotifier:$(git rev-parse --short HEAD) \
    -t vndg/webnotifier:latest .

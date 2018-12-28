#!/bin/sh
set -ex

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}

echo "namespace: ${NAMESPACE} and tag: ${TAG}"
echo "start to build SignalRNotifier..."


docker build -f samples/SignalRNotifier/Dockerfile \
    -t vndg/signalrnotifier:$(git rev-parse --short HEAD) \
    -t vndg/signalrnotifier:latest .

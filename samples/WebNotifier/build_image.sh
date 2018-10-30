#!/bin/sh
set -e 
set -x 

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}
echo "${NAMESPACE} and ${TAG}"

echo "Build Web Notifier..."
docker build -f samples/WebNotifier/Dockerfile -t vndg/webnotifier:$(git rev-parse --short HEAD) -t vndg/webnotifier:latest .

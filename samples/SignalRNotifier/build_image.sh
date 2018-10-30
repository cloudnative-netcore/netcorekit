#!/bin/sh
set -e 
set -x 

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}
echo "${NAMESPACE} and ${TAG}"

echo "Build SignalR..."
docker build -f samples/SignalRNotifier/Dockerfile -t vndg/signalrnotifier:$(git rev-parse --short HEAD) -t vndg/signalrnotifier:latest .

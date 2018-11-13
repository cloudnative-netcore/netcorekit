#!/bin/sh
set -ex

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}

echo "namespace: ${NAMESPACE} and tag: ${TAG}"
echo "start to build TodoApi..."

docker build -f samples/TodoApi/Dockerfile -t vndg/todoapi:$TAG -t vndg/todoapi:latest .

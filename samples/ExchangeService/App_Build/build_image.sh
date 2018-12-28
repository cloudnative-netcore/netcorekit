#!/bin/sh
set -ex

TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}

echo "namespace: ${NAMESPACE} and tag: ${TAG}"
echo "start to build ExchangeService..."

docker build -f samples/ExchangeService/Dockerfile \
    -t $NAMESPACE/exchange-service:$TAG \
    -t $NAMESPACE/exchange-service:latest .

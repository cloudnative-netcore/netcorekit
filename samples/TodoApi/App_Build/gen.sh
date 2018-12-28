#!/bin/bash
set -ex

PROJ_DIR=`pwd`
GRPC_PATH=${HOME}/.nuget/packages/grpc.tools/1.17.1/tools/linux_x64
PROTO_PATH=${PROJ_DIR}/../../_protos/v1
OUTPUT_PATH=${PROJ_DIR}/../Rpc

$GRPC_PATH/protoc -I $PROTO_PATH -I /usr/local/include \
    --csharp_out $OUTPUT_PATH $PROTO_PATH/project.proto

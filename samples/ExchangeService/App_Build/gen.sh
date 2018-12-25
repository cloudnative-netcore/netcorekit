#!/bin/bash

PROJ_DIR=`pwd`
GRPC_PATH=${HOME}/.nuget/packages/grpc.tools/1.17.1/tools/linux_x64
PROTO_PATH=${PROJ_DIR}/../../_protos/v1
OUTPUT_PATH=${PROJ_DIR}/../Rpc

$GRPC_PATH/protoc -I $PROTO_PATH --csharp_out $OUTPUT_PATH --grpc_out $OUTPUT_PATH $PROTO_PATH/bimonetary.proto --plugin=protoc-gen-grpc=${GRPC_PATH}/grpc_csharp_plugin

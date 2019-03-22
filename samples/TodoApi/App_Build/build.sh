#!/bin/sh
set -ex

export ASPNETCORE_ENVIRONMENT Development
dotnet build

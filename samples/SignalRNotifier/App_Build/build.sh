#!/bin/sh
set -e 
set -x 

export ASPNETCORE_ENVIRONMENT Development
dotnet build

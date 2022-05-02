#!/bin/bash

if [ "$1" == "" ]; then
	echo "Missing environment name"
else
	echo "Running 'dotnet run' with env: $1..."
	ASPNETCORE_ENVIRONMENT=$1 dotnet run
fi

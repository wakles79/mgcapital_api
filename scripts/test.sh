#!/bin/bash
dotnet --info
# dotnet restore MGCap.sln

# Gets the absolute path of the current script
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

cd "$parent_path/.." # Sets the path


for project in test/*/*.csproj; do
    echo "Test: Testing project ${project}"
    dotnet test -c Release ${project}
done

cd - # Reverting pwd to its original state

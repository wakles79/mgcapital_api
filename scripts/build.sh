#!/bin/bash
dotnet --info
dotnet restore SteelErp.sln

# Gets the absolute path of the current script
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

cd "$parent_path/.." # Sets the path


for project in src/*/*.csproj; do
    echo "build: Packaging project in $project"
    dotnet build -c Release $project
done

cd - # Reverting pwd to its original state
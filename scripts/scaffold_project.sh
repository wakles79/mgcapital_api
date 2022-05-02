#!/bin/bash

# Solution Name
SLN_NAME="MGCap"

# Gets the absolute path of the current script
PARENT_PATH=$(cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P)

PROJECT_NAMES=(
    "$SLN_NAME.DataAccess.Abstract"
    "$SLN_NAME.DataAccess.Implementation"
    "$SLN_NAME.Business.Abstract"
    "$SLN_NAME.Business.Implementation"
    #"$SLN_NAME.Domain"
    )

# dotnet --info
# dotnet restore "$SLN_NAME.sln"

# Sets the path
cd "$PARENT_PATH/.." 

echo "add: Creating Solution: $SLN_NAME.sln"
dotnet new sln -n "$SLN_NAME"

IFS='.' # dot (.) is set as delimiter

for project_name in "${PROJECT_NAMES[@]}"; do
    read -ra dirs <<< "$project_name"
    dest="src/${dirs[-2]}/${dirs[-1]}" 
    echo "add: Creating project: $project_name"
    echo "add: Destination: $dest"
    # Creates Project with dotnet cli
    dotnet new classlib -n "$project_name" -o "$dest" --force
    # Adds Project to solution using dotnet cli
    echo "add: Adding project to solution"
    dotnet sln "$SLN_NAME.sln" add "$dest/$project_name.csproj"
    # Removes extra 'Class1.cs' file see https://github.com/dotnet/templating/issues/853
    rm "$dest/Class1.cs"
done

# TODO: For Client project creation
# dotnet new angular -n "$SLN_NAME.Presentation" -o Presentation --force
# dotnet sln "$SLN_NAME.sln" add "src/Presentation/$SLN_NAME.Presentation.csproj"

IFS=' ' # reset to default value after usage
cd - # Reverting pwd to its original state
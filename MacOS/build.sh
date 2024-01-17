#!/bin/bash

APP_NAME="./Champion.app"
RUNTIME="osx-$1"
PUBLISH_OUTPUT_DIRECTORY="../Champion.Desktop/bin/publish/$RUNTIME/"
# PUBLISH_OUTPUT_DIRECTORY should point to the output directory of your dotnet publish command.
# One example is /path/to/your/csproj/bin/Release/netcoreapp3.1/osx-x64/publish/.
# If you want to change output directories, add `--output /my/directory/path` to your `dotnet publish` command.
INFO_PLIST="./Info.plist"
ICON_FILE="../Assets/Icon.icns"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

cd ../Champion.Desktop
dotnet publish -r $RUNTIME --self-contained --configuration Release -p:UseAppHost=true --output ./bin/publish/$RUNTIME
cd ../MacOS

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "../Assets/Champion.icns" "$APP_NAME/Contents/Resources/Champion.icns"
cp "../Assets/ChampionBracket.icns" "$APP_NAME/Contents/Resources/ChampionBracket.icns"

chmod +x "$PUBLISH_OUTPUT_DIRECTORY/Champion.Desktop"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"

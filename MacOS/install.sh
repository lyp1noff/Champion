#!/bin/bash

mkdir "tmp"
cd "tmp"

APP_NAME="../Champion.app"
RUNTIME="osx-$1"
PUBLISH_OUTPUT_DIRECTORY="./$RUNTIME/"
INFO_PLIST="./Info.plist"
ICON_FILE="./Icon.icns"

curl "https://noboobs.life/projects/Champion/macos/$RUNTIME.zip" --output "$RUNTIME.zip"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/Assets/Icon.icns --output Icon.icns
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/MacOS/Info.plist --output Info.plist
unzip "$RUNTIME".zip
rm "$RUNTIME".zip

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/Icon.icns"

chmod +x "$PUBLISH_OUTPUT_DIRECTORY/Champion.Desktop"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"

xattr -cr $APP_NAME

cd ..
rm -r tmp

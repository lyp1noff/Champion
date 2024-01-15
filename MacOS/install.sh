#!/bin/bash

TEMPDIR="$TMPDIR/champion"
APP="./Champion.app"
RUNTIME="osx-$1"
INFO_PLIST="./Info.plist"
ICON_FILE="./Icon.icns"

mkdir "$TEMPDIR"
cd "$TEMPDIR"

curl "https://noboobs.life/projects/Champion/macos/$RUNTIME.zip" --output "$RUNTIME.zip"
unzip "$RUNTIME".zip
rm "$RUNTIME".zip

mkdir "$APP"
mkdir "$APP/Contents"
mkdir "$APP/Contents/MacOS"
mkdir "$APP/Contents/Resources"

curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/MacOS/Info.plist --output "$APP/Contents/Info.plist"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/Assets/Icon.icns --output "$APP/Contents/Resources/Icon.icns"

chmod +x "./$RUNTIME/Champion.Desktop"
cp -a "./$RUNTIME/" "$APP/Contents/MacOS"

xattr -cr $APP

if [ -d "/Applications/$APP" ]
then
    rm -rf "/Applications/$APP"
fi
cp -r "$APP" "/Applications/."
rm -r "$TEMPDIR"

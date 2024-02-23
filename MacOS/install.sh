#!/bin/bash

TEMPDIR="$TMPDIR/champion"
APP="./Champion.app"
RUNTIME="osx-$1"
INFO_PLIST="./Info.plist"
ICON_FILE="./Icon.icns"

mkdir "$TEMPDIR"
cd "$TEMPDIR"

curl "https://noboobs.help/projects/Champion/macos/$RUNTIME.tar.gz" --output "$RUNTIME.tar.gz"
tar -xzvf "$RUNTIME".tar.gz
rm "$RUNTIME".tar.gz

mkdir "$APP"
mkdir "$APP/Contents"
mkdir "$APP/Contents/MacOS"
mkdir "$APP/Contents/Resources"

curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/beta/MacOS/Info.plist --output "$APP/Contents/Info.plist"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/beta/Assets/Champion.icns --output "$APP/Contents/Resources/Champion.icns"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/beta/Assets/ChampionBracket.icns --output "$APP/Contents/Resources/ChampionBracket.icns"

chmod +x "./$RUNTIME/Champion.Desktop"
cp -a "./$RUNTIME/" "$APP/Contents/MacOS"

xattr -cr $APP

if [ -d "/Applications/$APP" ]
then
    rm -rf "/Applications/$APP"
fi
cp -r "$APP" "/Applications/."
rm -r "$TEMPDIR"
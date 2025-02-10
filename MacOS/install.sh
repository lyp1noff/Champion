#!/bin/bash

# curl -s https://raw.githubusercontent.com/lyp1noff/Champion/refs/heads/main/MacOS/install.sh | bash -s arm64/x64

TEMPDIR="$TMPDIR/champion"
APP="./Champion.app"
RUNTIME="osx-$1"
INFO_PLIST="./Info.plist"
ICON_FILE="./Icon.icns"

mkdir "$TEMPDIR"
cd "$TEMPDIR"

curl "https://github.com/lyp1noff/Champion/releases/download/latest/$RUNTIME.zip" --output "$RUNTIME.zip"
unzip "$RUNTIME.zip" -d "$RUNTIME"
rm "$RUNTIME".zip

mkdir "$APP"
mkdir "$APP/Contents"
mkdir "$APP/Contents/MacOS"
mkdir "$APP/Contents/Resources"

curl https://raw.githubusercontent.com/lyp1noff/Champion/refs/heads/main/MacOS/Info.plist --output "$APP/Contents/Info.plist"
curl https://raw.githubusercontent.com/lyp1noff/Champion/refs/heads/main/Assets/Champion.icns --output "$APP/Contents/Resources/Champion.icns"
curl https://raw.githubusercontent.com/lyp1noff/Champion/refs/heads/main/Assets/ChampionBracket.icns --output "$APP/Contents/Resources/ChampionBracket.icns"

chmod +x "./$RUNTIME/Champion"
cp -a "./$RUNTIME/" "$APP/Contents/MacOS"

xattr -cr $APP

if [ -d "/Applications/$APP" ]
then
    rm -rf "/Applications/$APP"
fi
cp -r "$APP" "/Applications/."
rm -r "$TEMPDIR"

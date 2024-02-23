#!/bin/bash

# curl -s https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/MacOS/install.sh | bash -s arm64/x64

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

curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/MacOS/Info.plist --output "$APP/Contents/Info.plist"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/Assets/Champion.icns --output "$APP/Contents/Resources/Champion.icns"
curl https://git.noboobs.help/lyp1noff/Champion/raw/branch/master/Assets/ChampionBracket.icns --output "$APP/Contents/Resources/ChampionBracket.icns"

chmod +x "./$RUNTIME/Champion.Desktop"
cp -a "./$RUNTIME/" "$APP/Contents/MacOS"

xattr -cr $APP

if [ -d "/Applications/$APP" ]
then
    rm -rf "/Applications/$APP"
fi
cp -r "$APP" "/Applications/."
rm -r "$TEMPDIR"

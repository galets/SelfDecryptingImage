#!/bin/bash

echo "" | xclip -in -selection clipboard

rm /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png
gnome-screenshot --area --file=/tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png && (
    #URL=`/home/galets/Projects/2015/VVCap/VVCapPost/bin/Release/VVCapPost.exe /tmp/ssvvcap.png`
    URL=`/home/galets/.local/share/SelfDecryptingImage/postsdi.exe /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png`
    if [ "$?" != "0" ]
    then
        notify-send "Failed to post screenshot"
    else
        echo "$URL"
        echo "$URL" | xclip -in -selection clipboard
        notify-send "Screenshot successfully posted. URL is in clipboard"
    fi
    rm /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png
)


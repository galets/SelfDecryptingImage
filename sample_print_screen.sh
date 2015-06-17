#!/bin/bash

echo "" | xclip -in -selection clipboard

rm /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png
gnome-screenshot --area --file=/tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png && (
    URL=$(/home/galets/.local/share/SelfDecryptingImage/postsdi.exe /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png 2> /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.err)
    if [ "$?" == "0" ]
    then
        echo "$URL"
        echo "$URL" | xclip -in -selection clipboard
        notify-send "Screenshot successfully posted" "URL is in clipboard" --icon=dialog-information
    else
        ERR=$(< /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.err)
        notify-send "Screenshot not posted" "$ERR" --icon=dialog-error
    fi
    rm /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.png
    rm /tmp/bfdf97f64c82fe331ce34d4d947e43a8944bd823.err
)


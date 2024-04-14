#!/bin/bash
# Check if the command has the correct format
if [ $# -ne 2 ]; then
    echo "Usage: chat <serverHostname> <username>"
    exit 1
fi
# Execute the chat client application with the specified username
cd /app
dotnet SimpleChat.Client.dll "$1" "$2"
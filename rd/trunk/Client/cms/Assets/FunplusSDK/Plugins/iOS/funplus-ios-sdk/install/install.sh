#!/bin/sh
#
# Copyright 2010-present Funplus.
#
# This script install the Funplus iOS Sdk  package

# ---------------------------------------------------------------------------

if [ x$1 != x ]
then
    echo $1
    python ./mod_pbxproj/install.py $1
else
    echo "Please input the param, for example: install.sh XcodeProjectPath"
fi

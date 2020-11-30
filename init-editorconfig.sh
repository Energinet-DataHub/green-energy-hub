#!/bin/bash

cp .editorconfig.ci .editorconfig

if [[ $1 == "loose" ]]; then
    sed -i "s/= error/= suggestion/gi" ".editorconfig"
    sed -i "s/= warning/= suggestion/gi" ".editorconfig"
fi
#!/bin/bash

export originalLocation=$(pwd)
export exportedTemplateLocation="/c/Users/isaac.martinez.MCORPRHS/Documents/Visual Studio 2019/My Exported Templates/"

echo Script Folder: $originalLocation
echo Template Folder: $exportedTemplateLocation

cd "$exportedTemplateLocation"

zipFiles=("NRSRx_OData_EF" "NRSRx_WebApi_EF")

for i in "${zipFiles[@]}"
do
  mkdir $i
  cd "$i"
  unzip -o ../$i.zip
  cp -T $originalLocation/$i.vstemplate "$exportedTemplateLocation"$i/MyTemplate.vstemplate
  
  # bestzip is a global npm package
  bestzip $i.modified.zip **  
  cp -T $i.modified.zip $originalLocation/NRSRx.Extensions/ProjectTemplates/$i.zip  
  cd ..
  rm -rd $i  
done
rm *.zip
cd "$originalLocation"

#!/bin/bash

export originalLocation=$(pwd)
export exportedTemplateLocation="/c/Users/Ikemt/Documents/Visual Studio 2019/My Exported Templates/"

if [ ! -d "$(pwd)/NRSRx.Extensions/ProjectTemplates" ]
then
    mkdir NRSRx.Extensions/ProjectTemplates
fi
npm i -g bestzip
rm NRSRx.Extensions/ProjectTemplates/*.zip

echo Script Folder: $originalLocation
echo Template Folder: $exportedTemplateLocation

cd "$exportedTemplateLocation"

zipFiles=("NRSRx_OData_EF" "NRSRx_OData_EF.Tests" "NRSRx_WebApi_EF")

for i in "${zipFiles[@]}"
do
  mkdir $i
  cd "$i"
  unzip -o ../$i.zip
  cp -T $originalLocation/$i.vstemplate "$exportedTemplateLocation"$i/MyTemplate.vstemplate
  cp -T $originalLocation/NRSRx.Extensions/ProfilePic.jpg "$exportedTemplateLocation"$i/__TemplateIcon.jpg
  
  # bestzip is a global npm package
  bestzip $i.modified.zip **  
  cp -T $i.modified.zip $originalLocation/NRSRx.Extensions/ProjectTemplates/$i.zip
  cd ..
  rm -rd $i  
done
cd "$originalLocation"


$paths = Get-ChildItem -Path $PSScriptRoot -include *.csproj -Recurse
foreach ($pathobject in $paths) {
    $path = $pathobject.fullname
    $doc = New-Object System.Xml.XmlDocument
    $doc.Load($path)
    if (!$doc.SelectSingleNode("//Project/PropertyGroup/PackageLicense")) {
        $child = $doc.CreateElement("PackageLicense")
        $child.InnerText = "https://raw.githubusercontent.com/ikemtz/NRSRx/master/LICENSE"
        $node = $doc.SelectSingleNode("//Project/PropertyGroup")
        $node.AppendChild($child)
 
    }
    if ($doc.SelectSingleNode("//Project/PropertyGroup/PackageLicenseUrl")) {
        $child = $doc.SelectSingleNode("//Project/PropertyGroup/PackageLicenseUrl");
        $child.ParentNode.RemoveChild($child);
    }
    $doc.Save($path)
}
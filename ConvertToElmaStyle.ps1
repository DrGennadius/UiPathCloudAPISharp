$mainChangedFragmentCount = 0
$mainChangedFileCount = 0
$sourceFiles = Get-ChildItem ./UiPathCloudAPI *.cs -rec
foreach ($file in $sourceFiles)
{
    $changedCount = 0
    $content = [System.IO.File]::ReadAllText($file.FullName)
	$contentNamespaceBases = [RegEx]::Matches($content,"(namespace\s*UiPathCloudAPISharp.*)")
    if ($contentNamespaceBases.Count -gt 0)
    {
        foreach ($contentNamespaceBase in $contentNamespaceBases)
        {
            $changedCount = $changedCount + 1
            $contentNamespaceBaseNew = $contentNamespaceBase -replace "(?<=namespace\s).*((UiPathCloudAPISharp.*(?=\.))|(UiPathCloudAPISharp))", "EleWise.ELMA.UiPathConnector"
            $content = $content -replace $contentNamespaceBase, $contentNamespaceBaseNew
        }
    }
    
    $contentUsingBases = [RegEx]::Matches($content,"(using\s*UiPathCloudAPISharp.*;)")
    if ($contentNamespaceBases.Count -gt 0)
    {
        foreach ($contentUsingBase in $contentUsingBases)
        {
            $changedCount = $changedCount + 1
            $contentUsingBaseNew = $contentUsingBase -replace "(?<=using\s).*((UiPathCloudAPISharp.*(?=\..+;))|(UiPathCloudAPISharp(?=;)))", "EleWise.ELMA.UiPathConnector"
            $content = $content -replace $contentUsingBase, $contentUsingBaseNew
        }
    }
    
    if ($changedCount -ne 0)
    {
        $mainChangedFileCount = $mainChangedFileCount + 1
        $mainChangedFragmentCount = $mainChangedFragmentCount + $changedCount
        [System.IO.File]::WriteAllText($file.FullName, $content)
    }
}
Write-Host "Changed File Count:     $mainChangedFileCount"
Write-Host "Changed Fragment Count: $mainChangedFragmentCount"
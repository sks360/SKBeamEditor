function Add-ProjectReference {
    param($project, [string]$assemblyPath)

    Write-Host ("Adding assembly reference: {0}." -f $assemblyPath);
    $references = (Get-ProjectReferences $project);
    $references.Add($assemblyPath);
}


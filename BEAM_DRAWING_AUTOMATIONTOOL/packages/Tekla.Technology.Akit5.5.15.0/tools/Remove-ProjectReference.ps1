function Remove-ProjectReference {
    param($project, [string]$referenceName)

    $references = (Get-ProjectReferences $project);

    $toBeRemoved = @();

    foreach ($reference in $references) {
        if ($reference.Name -eq $referenceName) {
            Write-Host ("Removing assembly reference: {0}." -f $reference.Identity);
            $toBeRemoved += $reference;
        }
    }

    $toBeRemoved | % { $_.Remove(); }
}


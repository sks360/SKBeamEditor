function Set-ProjectReferencePropertyValue {
    param($project, [string]$referenceName, [string]$propertyName, [object]$value, [string]$reason = $null)

    $references = Get-ProjectReferences $project;

    foreach ($reference in $references) {
        if ($reference.Name -eq $referenceName) {
            if ($reference.$propertyName -ne $value) {
                $reference.$propertyName = $value;
                $format = "Changing property value in assembly reference: [{0}]::{1} = '{2}'. {3}";
                Write-Host ($format -f $referenceName, $propertyName, $value, $reason);
            }
        }
    }
}


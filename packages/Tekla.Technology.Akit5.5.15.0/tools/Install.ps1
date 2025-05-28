param($installPath, $toolsPath, $package, $project)

Write-Host ("{0}: {1} begin" -f $package, $MyInvocation.MyCommand.Name);

. $toolsPath\Get-ProjectReferences.ps1
. $toolsPath\Set-ProjectReferencePropertyValue.ps1
#. $toolsPath\Add-ProjectReference.ps1

if ($project.Type -in @('C#', 'VB.NET')) {
    Set-ProjectReferencePropertyValue $project "Akit5" "EmbedInteropTypes" $false
} elseif ($project.Type -eq 'C++') {
    Write-Host "Package installation does not add references for mixed projects. Following assembly must be referenced manually:"
    Write-Host "AnyCPU: $installpath\lib\net\Akit5.dll"
    #Add-ProjectReference $project $installPath\lib\net\Akit5.dll
} else {
    Write-Host ("Project type '{0}' is not recognized. Package installation may be incomplete." -f $project.Type)
}

Write-Host ("{0}: {1} end" -f $package, $MyInvocation.MyCommand.Name);

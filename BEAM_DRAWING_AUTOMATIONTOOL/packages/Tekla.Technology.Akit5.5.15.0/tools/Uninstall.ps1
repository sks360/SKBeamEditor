param($installPath, $toolsPath, $package, $project)

Write-Host ("{0}: {1} begin" -f $package, $MyInvocation.MyCommand.Name);

. $toolsPath\Get-ProjectReferences.ps1
#. $toolsPath\Remove-ProjectReference.ps1

if ($project.Type -in @('C#', 'VB.NET')) {
    # Do nothing
} elseif ($project.Type -eq 'C++') {
    Write-Host "Package uninstallation does not remove references for mixed projects."
    #Remove-ProjectReference $project Akit5
} else {
    Write-Host ("Project type '{0}' is not recognized. Package uninstallation may be incomplete." -f $project.Type)
}

Write-Host ("{0}: {1} end" -f $package, $MyInvocation.MyCommand.Name);

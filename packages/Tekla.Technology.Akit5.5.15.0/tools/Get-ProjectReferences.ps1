function Get-ProjectReferences {
    param($project)

    try {
        $references = $project.Object.References;
    } catch [System.Management.Automation.ExtendedTypeSystemException] {
        # Kludge to avoid problems with non-CLS compliant project types.
        $message = $_.Exception.Message;
        $matches = [regex]::Matches($message, 'The field/property: \"(?<field>.*)\" for type: \"(?<Type>[^"]+)\" .* Failed to use non CLS compliant type.');
        $type = $matches[0].Groups["Type"].Value -as [Type];

        $property = $type.GetProperty("References");
        $references = $property.GetValue($project.Object, $null);
    }

    return @(,$references);
}


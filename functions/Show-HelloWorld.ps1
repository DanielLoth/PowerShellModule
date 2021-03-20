Set-StrictMode -Version Latest

function Show-HelloWorld {
    <#
    .SYNOPSIS
    Show-HelloWorld synopsis

    .DESCRIPTION
    Description

    .PARAMETER a
    Parameter a is cool.

    .EXAMPLE
    Example 1x
    #>

    param (
        $a = "test"
    )

    Write-Host "Hello world, $a, $(Test-Administrator)"
}

# Export-ModuleMember -Function Show-HelloWorld

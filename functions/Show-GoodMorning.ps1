Set-StrictMode -Version Latest

function Show-GoodMorning {
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = "High")]
    param (
        
    )

    Write-Host "Good morning"
}

# Export-ModuleMember -Function Show-GoodMorning

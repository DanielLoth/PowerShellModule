foreach ($function in (Get-ChildItem "$PSScriptRoot\functions\*.ps1")) {
    . $function # Dot source function file
}

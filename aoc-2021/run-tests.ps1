$tlist = [string]::Join(',', $args)

if (-not [string]::IsNullOrWhiteSpace($tlist)) {
    $setenv = $true
    $env:AOC_TEST_LIST = $tlist
}

try {
    dotnet test --logger 'console;verbosity=detailed'
} finally {
    if ($setenv) {
        remove-item env:\AOC_TEST_LIST
    }
}

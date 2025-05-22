Get-ChildItem -Recurse -Filter *.js | ForEach-Object {
    if (Select-String -Path $_.FullName -Pattern '<[A-Za-z]') {
        Rename-Item $_.FullName ($_.FullName -replace '\.js$', '.jsx')
    }
}

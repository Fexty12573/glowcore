# powershell -ExecutionPolicy Bypass -File cloc-to-csv.ps1

$inputFile = "loc_files_final.txt"
$outputFile = "loc_files_final.csv"

$lines = Get-Content $inputFile

$results = @()

foreach ($line in $lines) {

    if ($line -match "^-{3,}" -or
        $line -match "File\s+blank" -or
        $line -match "SUM:" -or
        $line.Trim() -eq "") {
        continue
    }

    $parts = $line -split "\s{2,}"

    if ($parts.Count -ge 4) {

        $file = $parts[0]

        $file = Split-Path $file -Leaf

        $blank = $parts[-3]
        $comment = $parts[-2]
        $code = $parts[-1]

        $results += "$file,$blank,$comment,$code"
    }
}

"File,Blank,Comment,Code" | Out-File $outputFile -Encoding utf8
$results | Out-File $outputFile -Append -Encoding utf8

Write-Host "CSV erstellt: $outputFile"
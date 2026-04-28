# powershell -ExecutionPolicy Bypass -File loc-history.ps1

$results = @()

$branch = "dev"
$startDate = Get-Date "2026-02-20"
$endDate = Get-Date
$stepDays = 14

$current = $startDate
$lastCommit = ""

while ($current -le $endDate) {
    $dateString = $current.ToString("yyyy-MM-dd")

    $commit = git rev-list -1 --before="$dateString" $branch

    if ($commit) {
        Write-Host "Processing $dateString ($commit)"

        $lastCommit = $commit

        git checkout $commit | Out-Null

        $clocOutput = cloc ../Assets --quiet --csv `
            --include-ext=cs `
            --exclude-ext=meta

        $lastLine = $clocOutput | Select-Object -Last 1
        $columns = $lastLine -split ","
        $codeLines = $columns[4]

        $results += "$dateString,$codeLines"
    }

    $current = $current.AddDays($stepDays)
}

git checkout $branch | Out-Null

$results | Out-File "loc_history.csv"

if ($lastCommit) {
    Write-Host "Erstelle File-Liste für letzten Stand ($lastCommit)"

    git checkout $lastCommit | Out-Null

    cloc ../Assets `
        --include-ext=cs `
        --exclude-ext=meta `
        --by-file `
        --report-file="loc_files_final.txt"

    git checkout $branch | Out-Null
}

Write-Host "Fertig: CSV + finale File-Liste erstellt"
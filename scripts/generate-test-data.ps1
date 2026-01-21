# generate-test-data.ps1
# Populates the MiniSectors database with fake records for performance testing.
# Run from project root: .\scripts\generate-test-data.ps1

param(
    [int]$RecordCount = 500
)

$dbPath = "$env:USERPROFILE\Documents\SimHub\PluginsData\User.PluginMiniSectors\MiniSectors.sqlite"

if (-not (Test-Path $dbPath)) {
    Write-Error "Database not found at: $dbPath"
    Write-Host "Run SimHub with the plugin loaded first to create the database."
    exit 1
}

# Sample data arrays
$tracks = @(
    "monza", "spa", "silverstone", "nurburgring", "suzuka",
    "laguna_seca", "brands_hatch", "bathurst", "imola", "zandvoort",
    "kyalami", "oulton_park", "snetterton", "donington", "watkins_glen",
    "mount_panorama", "hungaroring", "paul_ricard", "misano", "valencia"
)

$cars = @(
    "Ferrari 488 GT3", "Porsche 911 GT3 R", "McLaren 720S GT3",
    "BMW M4 GT3", "Audi R8 LMS GT3", "Mercedes AMG GT3",
    "Lamborghini Huracan GT3", "Aston Martin V8 Vantage GT3",
    "Bentley Continental GT3", "Nissan GT-R Nismo GT3",
    "Lexus RC F GT3", "Honda NSX GT3"
)

$weatherTypes = @("Dry", "Light Rain", "Heavy Rain", "Overcast", "")

# Check for sqlite3 CLI
$useSqliteCli = $false
if (Get-Command sqlite3 -ErrorAction SilentlyContinue) {
    $useSqliteCli = $true
    Write-Host "Using sqlite3 CLI"
} else {
    Write-Error "sqlite3 CLI not found. Please install SQLite tools."
    Write-Host "You can install it via: winget install SQLite.SQLite"
    exit 1
}

Write-Host "Generating $RecordCount test records..."

$sql = @()
$random = New-Object System.Random

for ($i = 0; $i -lt $RecordCount; $i++) {
    $track = $tracks[$random.Next($tracks.Length)]
    $sector = $random.Next(1, 21)  # 1-20 sectors
    $car = $cars[$random.Next($cars.Length)].Replace("'", "''")  # Escape single quotes
    $time = [math]::Round(15 + $random.NextDouble() * 45, 3)  # 15-60 seconds
    $weather = $weatherTypes[$random.Next($weatherTypes.Length)]
    $trackTemp = [math]::Round(20 + $random.NextDouble() * 30, 1)  # 20-50 C
    $airTemp = [math]::Round(15 + $random.NextDouble() * 25, 1)   # 15-40 C
    $daysAgo = $random.Next(0, 365)
    $recordedAt = (Get-Date).AddDays(-$daysAgo).AddHours(-$random.Next(0, 24)).ToString("o")

    # Use a unique suffix to avoid conflicts with existing data
    $uniqueTrack = "${track}_test_$i"

    $sql += @"
INSERT INTO sector_bests
    (track_id, sector_number, best_time_sec, car_model, weather_type, track_temp_celsius, air_temp_celsius, grip_level, recorded_at)
VALUES
    ('$uniqueTrack', $sector, $time, '$car', '$weather', $trackTemp, $airTemp, '', '$recordedAt');
"@
}

# Write SQL to temp file and execute
$sqlFile = [System.IO.Path]::GetTempFileName()
$sql -join "`n" | Out-File -FilePath $sqlFile -Encoding UTF8

try {
    & sqlite3 $dbPath ".read `"$sqlFile`""
    if ($LASTEXITCODE -ne 0) {
        Write-Error "sqlite3 command failed with exit code $LASTEXITCODE"
        exit 1
    }
} finally {
    Remove-Item $sqlFile -ErrorAction SilentlyContinue
}

Write-Host "Done! Inserted $RecordCount test records into $dbPath"
Write-Host "Restart SimHub and check the Records tab."

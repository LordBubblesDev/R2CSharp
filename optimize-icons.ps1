param(
    [string]$SourceDir = "src",
    [string]$IconsFile = "fa-icons.json",
    [string]$OutputFile = "src/R2CSharp.Lib/Assets/fa-icons.json",
    [switch]$NoIndentation
)

Write-Host "Scanning source code for FontAwesome icon usage..." -ForegroundColor Cyan

$sourceFiles = Get-ChildItem -Path $SourceDir -Recurse -Include "*.cs", "*.axaml", "*.axaml.cs" | Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" }

$iconPattern = 'fa-(solid|regular|light|thin|duotone)\s+fa-([a-zA-Z0-9-]+)'
$usedIcons = @{}

foreach ($file in $sourceFiles) {
    $content = Get-Content $file.FullName -Raw
    $matches = [regex]::Matches($content, $iconPattern)
    
    foreach ($match in $matches) {
        $style = $match.Groups[1].Value
        $iconName = $match.Groups[2].Value
        
        if (-not $usedIcons.ContainsKey($iconName)) {
            $usedIcons[$iconName] = @{}
        }
        $usedIcons[$iconName][$style] = $true
    }
}

Write-Host "Found $($usedIcons.Count) unique icons:" -ForegroundColor Green
foreach ($icon in $usedIcons.Keys | Sort-Object) {
    $styles = $usedIcons[$icon].Keys -join ", "
    Write-Host "  - $icon ($styles)" -ForegroundColor Yellow
}

Write-Host "Loading file '$IconsFile'..." -ForegroundColor Cyan
if (-not (Test-Path $IconsFile)) {
    Write-Error "Icons file not found: $IconsFile"
    exit 1
}

$allIcons = Get-Content $IconsFile -Raw | ConvertFrom-Json

$optimizedIcons = @{}

foreach ($iconName in $usedIcons.Keys) {
    if ($allIcons.PSObject.Properties.Name -contains $iconName) {
        $iconData = $allIcons.$iconName
        $optimizedIcon = @{}
        
        foreach ($style in $usedIcons[$iconName].Keys) {
            if ($iconData.svg.PSObject.Properties.Name -contains $style) {
                $optimizedIcon[$style] = $iconData.svg.$style
            }
        }
        
        if ($optimizedIcon.Count -gt 0) {
            $optimizedIcons[$iconName] = @{
                svg = $optimizedIcon
            }
        }
    } else {
        Write-Warning "Icon '$iconName' not found in $IconsFile"
    }
}

Write-Host "Saving optimized icons to $OutputFile..." -ForegroundColor Cyan

$formattedLines = @()
$formattedLines += "{"

$iconNames = $optimizedIcons.Keys | Sort-Object
for ($i = 0; $i -lt $iconNames.Count; $i++) {
    $iconName = $iconNames[$i]
    $iconData = $optimizedIcons[$iconName]
    $isLast = $i -eq ($iconNames.Count - 1)
    
    $formattedLines += "`n    `"$iconName`":{`"svg`":{"
    
    $styleNames = $usedIcons[$iconName].Keys | Sort-Object
    
    $styleIndex = 0
    foreach ($styleName in $styleNames) {
        $styleData = $iconData.svg.$styleName
        $isLastStyle = $styleIndex -eq ($styleNames.Count - 1)
        
        $formattedLines += "`n            `"$styleName`":{`"path`":`"$($styleData.path)`",`"viewBox`":[$($styleData.viewBox -join ',')]}"
        
        if (-not $isLastStyle) {
            $formattedLines[-1] += ","
        }
        $styleIndex++
    }
    
    if ($styleNames.Count -eq 1) {
        $formattedLines += "`n        }},"
    } else {
        $formattedLines += "`n            }},"
    }
    
    if ($isLast) {
        $formattedLines[-1] = $formattedLines[-1].TrimEnd(',')
    }
}

$formattedLines += "`n}"
$formattedJson = $formattedLines -join ""

if ($NoIndentation) {
    $formattedJson = $formattedJson -replace "`n\s*", ""
}

$formattedJson | Set-Content $OutputFile -Encoding UTF8

$originalSize = (Get-Item $IconsFile).Length
$optimizedSize = (Get-Item $OutputFile).Length
$reduction = [math]::Round((($originalSize - $optimizedSize) / $originalSize) * 100, 2)

Write-Host "Optimization complete!" -ForegroundColor Green
Write-Host "Original size: $([math]::Round($originalSize / 1MB, 2)) MB" -ForegroundColor White
Write-Host "Optimized size: $([math]::Round($optimizedSize / 1MB, 2)) MB" -ForegroundColor White
Write-Host "Size reduction: $reduction%" -ForegroundColor Green
Write-Host "Icons included: $($optimizedIcons.Count)" -ForegroundColor Cyan

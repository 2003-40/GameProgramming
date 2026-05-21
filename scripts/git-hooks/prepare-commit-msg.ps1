param(
    [Parameter(Mandatory = $true)]
    [string]$CommitMsgFile,
    [string]$CommitSource,
    [string]$CommitSha
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($CommitSource -in @("merge", "squash", "commit")) {
    exit 0
}

if (-not (Test-Path -LiteralPath $CommitMsgFile)) {
    exit 0
}

$existing = Get-Content -LiteralPath $CommitMsgFile -Raw
if (-not [string]::IsNullOrWhiteSpace($existing)) {
    exit 0
}

$branch = (git rev-parse --abbrev-ref HEAD).Trim()
if ([string]::IsNullOrWhiteSpace($branch) -or $branch -eq "HEAD") {
    exit 0
}

$issueId = ""
$issueMatch = [regex]::Match($branch, "(?<!\d)(\d{1,6})(?!\d)")
if ($issueMatch.Success) {
    $issueId = $issueMatch.Groups[1].Value
}

$type = "chore"
switch -Regex ($branch) {
    "^feature/" { $type = "feat"; break }
    "^feat/" { $type = "feat"; break }
    "^fix/" { $type = "fix"; break }
    "^hotfix/" { $type = "fix"; break }
    "^docs/" { $type = "docs"; break }
    "^refactor/" { $type = "refactor"; break }
    "^test/" { $type = "test"; break }
    "^chore/" { $type = "chore"; break }
}

$scope = "core"
$scopeMatch = [regex]::Match($branch, "^[^/]+/([^/-]+)")
if ($scopeMatch.Success) {
    $scope = $scopeMatch.Groups[1].Value.ToLowerInvariant()
}

$files = @(git diff --cached --name-only | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
$areas = @()
foreach ($file in $files) {
    $parts = $file -split "[/\\]"
    if ($parts.Count -ge 2) {
        $areas += "$($parts[0])/$($parts[1])"
    }
    elseif ($parts.Count -eq 1) {
        $areas += $parts[0]
    }
}

$areas = $areas | Sort-Object -Unique
if ($areas.Count -eq 0) {
    $areas = @("<staged files not found>")
}

$areasText = ($areas | ForEach-Object { "- $_" }) -join "`n"

$refsLine = "- <add issue id>"
if ($issueId) {
    $refsLine = "- #$issueId"
}

$template = @"
$type($scope): <summary>

Why:
- <why this change is needed>

What:
$areasText

Refs:
$refsLine
Branch:
- $branch
"@

Set-Content -LiteralPath $CommitMsgFile -Value $template -NoNewline

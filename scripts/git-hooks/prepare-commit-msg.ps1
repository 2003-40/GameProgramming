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
$refsSuffix = ""
if ($issueId) {
    $refsSuffix = " (#$issueId)"
}

$template = "$type($scope): <summary>$refsSuffix"
Set-Content -LiteralPath $CommitMsgFile -Value $template -NoNewline

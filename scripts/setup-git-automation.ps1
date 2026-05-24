Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

git config --local core.hooksPath "scripts/git-hooks"
git config --local commit.template ".gitmessage.txt"

Write-Host "Configured git automation for this repository."
Write-Host "core.hooksPath = scripts/git-hooks"
Write-Host "commit.template = .gitmessage.txt"
Write-Host ""
Write-Host "Try it:"
Write-Host "  1) git checkout -b feature/123-inventory"
Write-Host "  2) stage files"
Write-Host "  3) git commit"

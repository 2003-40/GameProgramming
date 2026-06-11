# GitHub Kanban + Commit Message Automation

## 1. Kanban auto add (Issue / PR)

This repository includes:
- `.github/workflows/kanban-auto-add.yml`

It automatically adds newly opened or reopened Issues/PRs to your GitHub Project (Kanban).

### One-time setup

1. Create or open your GitHub Project (Kanban board).
2. In repo settings, add a repository variable:
   - Name: `KANBAN_PROJECT_URL`
   - Value example: `https://github.com/users/2003-40/projects/3`
3. Create a Personal Access Token (classic) with `project` scope.
4. In repo secrets, add:
   - Name: `KANBAN_PROJECT_TOKEN`
   - Value: `TODO: add your GitHub token in repository secrets only`

Do not commit a real GitHub token into the repository. Keep the token only in GitHub Secrets and rotate it immediately if it was ever exposed in a committed file.

After this, opening/reopening an Issue or PR will automatically push the item into the board.

### Recommended Project built-in workflows (UI)

In your GitHub Project, turn on built-in automations:
- Auto-add items from this repository.
- Set `Status = Done` when PR is merged / Issue is closed.
- Set `Status = In Progress` when someone is assigned.

These built-ins + this workflow are enough for most class progress tracking.

---

## 2. Commit message template + auto draft from branch/issue

This repository includes:
- `.gitmessage.txt`
- `scripts/git-hooks/prepare-commit-msg.ps1`
- `scripts/git-hooks/prepare-commit-msg`
- `scripts/setup-git-automation.ps1`

### One-time setup (local)

Run in repo root:

```powershell
.\scripts\setup-git-automation.ps1
```

This sets:
- `core.hooksPath = scripts/git-hooks`
- `commit.template = .gitmessage.txt`

### How it behaves

When you run `git commit` (without `-m`), the hook drafts a one-line message:
- format: `<type>(<scope>): <summary> (#issue-id)`
- type/scope inferred from branch name (`feature/*` -> `feat`, `fix/*` -> `fix`, etc.)
- issue id inferred from branch name number (e.g. `feature/123-inventory`)

### Branch naming recommendation

Use this pattern for best results:
- `feature/123-inventory-system`
- `fix/145-shop-price-bug`
- `docs/166-devlog-update`

---

## 3. Optional: make PR title and commit style stricter

If you want, next step can be:
- add a PR title checker workflow
- add commitlint (Conventional Commits) check in CI

That helps keep logs clean for grading and demos.

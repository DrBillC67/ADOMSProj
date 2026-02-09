# Contributing to AzDO MS Project Add-in

Thank you for your interest in contributing. This document gives a short overview of how to get started.

---

## Getting started

1. **Fork and clone** the repository.
2. **Build** the solution (see [docs/INSTALL.md](docs/INSTALL.md)).
3. **Read** [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) for solution structure, async usage, and where to add API or UI changes.

---

## What to contribute

- **Bug fixes** — Prefer a small, focused change with a short description of the issue and fix.
- **Documentation** — Fixes or improvements to the [User Guide](docs/USER_GUIDE.md), [INSTALL](docs/INSTALL.md), or [DEVELOPMENT](docs/DEVELOPMENT.md) docs are welcome.
- **Features** — Open an issue first to discuss scope and design; keep the add-in compatible with MS Project desktop and .NET Framework 4.8.

---

## Code and design

- **Async:** Use `async`/`await` for any new Azure DevOps or I/O work; avoid `.Result` or `.Wait()`.
- **Core vs Add-in:** Put REST client logic and DTOs in **AzDOAddIn.Core**; keep Office/Project-specific code in the add-in project.
- **Errors:** Use `AzDoApiException` in Core for API failures; show user-friendly messages in the UI (e.g. `MessageBox`).
- **PAT:** Do not log or store PATs in plain text; they live in Windows Credential Manager only.

---

## Submitting changes

- **Branches:** Use a feature branch (e.g. `fix/401-handling` or `docs/user-guide-troubleshooting`).
- **Commits:** Use clear, short commit messages.
- **Pull requests:** Target the default branch (e.g. `main`). Describe what changed and why; reference any related issues.

---

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (see [LICENSE](LICENSE)).

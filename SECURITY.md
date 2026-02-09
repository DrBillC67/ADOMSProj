# Security

## PAT and credentials

- The add-in uses **Personal Access Tokens (PAT)** only; they are stored in **Windows Credential Manager** under targets like `AzDOAddIn:<organization URL>`.
- PATs are not stored in project files or in plain text in configuration.
- Use the minimum PAT scope and an expiration date. Rotate PATs periodically.

## Reporting a vulnerability

If you believe you have found a security issue, please report it responsibly (e.g. via a private security advisory or contact the maintainers) rather than opening a public issue. Include steps to reproduce and impact if possible.

## Dependencies

- **Newtonsoft.Json** and **CredentialManagement** are used with versions listed in the project files. Keep them updated when updating the solution.

# EasySave

> Backup software developed by **ProSoft** — reliable, fast, and built for enterprise environments.

---

## Table of Contents

- [About the Project](#about-the-project)
- [Versioning Roadmap](#versioning-roadmap)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Features](#features)
- [Project Structure](#project-structure)
- [Technical Requirements](#technical-requirements)
- [Documentation](#documentation)
- [Team & Contributions](#team--contributions)
- [License](#license)

---

## About the Project

EasySave is a backup software solution developed as part of the **ProSoft Suite**. It allows users to define, configure, and execute backup jobs across local disks, external drives, and network drives.

This project is maintained by an internal development team and follows professional software development standards including versioning, UML design, and full documentation.

**Pricing:**
- Unit price: €200 excl. tax
- Annual maintenance contract (5/7, 8am–5pm, updates included): 12% of purchase price

---

## Versioning Roadmap

| Version | Type | Status | Description |
|---------|------|--------|-------------|
| 1.0 | Console App | ✅ In development | Core backup engine, CLI, logging |
| 1.1 | Console App | 🔒 Specs pending | Minor fixes and improvements |
| 2.0 | GUI App | 🔒 Specs pending | Graphical interface (MVVM architecture) |
| 3.0 | GUI App | 🔒 Specs pending | Extended features |

---

## Getting Started

### Prerequisites

- Windows OS
- [.NET 10.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Installation

Clone the repository:

```bash
git clone https://github.com/your-team/EasySave.git
cd EasySave
```

Build the solution:

```bash
dotnet build
```

---

## Usage

EasySave is a console application that can be run directly from the terminal.

### Run specific backup jobs

```bash
# Run backups 1 through 3 sequentially
EasySave.exe 1-3

# Run backups 1 and 3 only
EasySave.exe 1;3
```

### Interactive mode

Simply launch the executable without arguments to enter interactive mode and manage your backup jobs through the menu.

---

## Features

### v1.0 — Console Application

- ✅ Create up to **5 backup jobs** (name, source, target, type)
- ✅ **Full backup** and **differential backup** support
- ✅ Works with local disks, external drives, and network drives
- ✅ **Multilingual** interface (English and French)
- ✅ Command-line execution with job range or selection syntax
- ✅ **Daily log file** — records all file transfer actions in real time (JSON format)
- ✅ **Real-time status file** (`state.json`) — tracks progress of all active backup jobs

### Logging (EasyLog.dll)

The logging feature is developed as an independent reusable library (`EasyLog.dll`) to ensure compatibility with all future versions of EasySave.

Each log entry includes:
- Timestamp
- Backup job name
- Full source file path (UNC format)
- Full destination file path (UNC format)
- File size
- Transfer time in milliseconds (negative if error)

### Real-time Status File

The `state.json` file is updated in real time for each active job and includes:
- Backup job name
- Timestamp of last action
- Backup status (Active / Inactive)
- Total number of eligible files
- Total size of files to transfer
- Progression percentage
- Number of remaining files
- Size of remaining files
- Source and destination file paths currently being processed

---

## Project Structure

```
EasySave/
│
├── EasySave.sln
│
├── EasySave/                  # Console application
│   ├── Program.cs
│   ├── Models/
│   ├── Services/
│   ├── Controllers/
│   └── Resources/             # EN/FR language files
│
├── EasyLog/                   # Reusable logging DLL
│   └── EasyLog.cs
│
└── docs/
    ├── uml diagrams/                   # UML diagrams
    ├── user-manual.md
    ├── release-note.md
    └── technical-support.md
```

---

## Technical Requirements

| Requirement | Detail |
|---|---|
| Language | C# |
| Framework | .NET 10.0 |
| IDE | Visual Studio 2022 or higher |
| Version Control | GitHub |
| UML Editor | ArgoUML (recommended) |
| Output Format | JSON (log and config files) |
| Code Language | English (all comments, variables, documentation) |

### Constraints

- No hardcoded paths (e.g., `C:\temp\` is forbidden)
- No redundant lines of code
- Functions must remain concise and readable
- All code and documentation must be usable by English-speaking subsidiaries

---

## Documentation

| Document | Description |
|---|---|
| [User Manual](docs/user-manual.md) | One-page guide for end users |
| [Release Note](docs/release-note.md) | Changes and notes per version |
| [Technical Support](docs/technical-support.md) | Config file locations, minimum requirements |
| [UML Diagrams](docs/uml/) | Class diagrams and sequence diagrams |

---

## Team & Contributions

This project is developed internally at **ProSoft** by the following team:

| Name | Role |
|------|------|
| Manil Doudou | Developer |
| Youcef Djarir | Developer |
| Isaac Sastre | Developer |

All contributions must go through the standard Git workflow:

1. Create a feature branch from `develop`: `feature/your-feature-name`
2. Commit using the agreed format: `feat: description`, `fix: description`, `docs: description`
3. Open a Pull Request into `develop`
4. Merge into `main` only for official releases

> Your tutor/supervisor must be invited as a collaborator on this repository.

---

## License

Copyright (c) 2026 ProSoft. All rights reserved.

This software and its source code are the exclusive property of ProSoft.
Unauthorized copying, distribution, or modification of this software,
via any medium, is strictly prohibited.

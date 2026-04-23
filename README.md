# EasySave

> Professional backup software solution developed by **ProSoft** — reliable, secure, and built for enterprise data management.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-Proprietary-red)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![Language](https://img.shields.io/badge/language-C%23-239120)

---

## Table of Contents

- [About the Project](#about-the-project)
- [Versioning Roadmap](#versioning-roadmap)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Technical Requirements](#technical-requirements)
- [Logging — EasyLog.dll](#logging--easylogdll)
- [Project Structure](#project-structure)
- [Team & Contributions](#team--contributions)
- [License](#license)

---

## About the Project

**EasySave** is a robust, enterprise-grade backup engine developed by the **ProSoft** engineering team. Designed to handle critical data transfers across local, external, and network drives, EasySave delivers consistent performance, real-time monitoring, and structured logging through its dedicated `EasyLog` module.

Built with scalability and maintainability in mind, EasySave evolves across three major versions — from a powerful CLI tool to a fully graphical, multi-threaded backup suite with encryption and business software detection.

### Commercial Terms

| Item | Details |
|------|---------|
| Unit Price | €200 excl. tax |
| Annual Maintenance | 12% of purchase price |
| Support Hours | 5/7 — 8:00 AM to 5:00 PM |
| Contract Type | Annual with tacit renewal (SYNTEC index revaluation) |

---

## Versioning Roadmap

| Version | Interface | Focus | Status |
|---------|-----------|-------|--------|
| **v1.0** | Console | Core CLI engine, up to 5 backup jobs, JSON logs, EasyLog DLL | Released |
| **v1.1** | Console | XML log format support, backward compatible with v1.0 | Released |
| **v2.0** | GUI (WPF/MVVM) | Unlimited jobs, CryptoSoft encryption, business software detection | In Development |
| **v3.0** | GUI (WPF/MVVM) | Real-time job controls (Play / Pause / Stop), presentation build | 📋 Planned |

---

## Key Features

### Version 1.0 — The Foundation

- **Console Application** built on .NET 8.0
- **Up to 5 backup jobs**, each defined by name, source directory, target directory, and type (full or differential)
- **CLI Execution** — sequential (`EasySave.exe 1-3`) or selective (`EasySave.exe 1;3`) job execution
- **Multi-source Support** — local disks, external drives, and network drives
- **Daily Log File** via `EasyLog.dll` — records timestamp, file paths (UNC format), file size, and transfer time in milliseconds
- **Real-time State File** (`state.json`) — tracks progress, remaining files, and current action per job
- **Multi-language Support** — English and French at a minimum
- **JSON format** for all log, state, and configuration files with line breaks for readability

### Version 1.1 — The Compatibility Update

- All features from v1.0
- **Selectable log format**: users can choose between **JSON** or **XML** for daily logs
- Fully backward compatible with v1.0

### Version 2.0 — The Enterprise Update

- **Graphical User Interface** using WPF or an equivalent framework, built on **MVVM architecture**
- **Unlimited backup jobs** — no more 5-job restriction
- **CryptoSoft Integration**:
  - High-security file encryption for user-defined extensions
  - Encryption duration recorded in logs (ms); negative value indicates error
- **Business Software Detection**:
  - Automatically detects active business software (e.g., accounting tools, ERP systems)
  - Prevents new jobs from launching; completes the current file transfer before stopping sequential jobs
  - Shutdown events are recorded in the log file
- **Enhanced Daily Log** — includes encryption time per file
- Log format choice (JSON or XML) carried over from v1.1

### Version 3.0 — The Real-Time Update

- **Per-job controls** — Play, Pause, and Stop for each backup task independently
- Full presentation build and final project delivery

---

## Getting Started

### Prerequisites

- **OS**: Windows 10 / 11
- **Framework**: .NET 8.0 Runtime
- **IDE**: Visual Studio 2022 or higher
- **External Tool**: `CryptoSoft.exe` *(required for v2.0 and above)*

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/ProSoft/EasySave.git
   cd EasySave
   ```

2. Open the solution in Visual Studio:
   ```
   EasySave.sln
   ```

3. Build the solution (Release mode recommended):
   ```
   Build > Build Solution (Ctrl + Shift + B)
   ```

4. Run the application:
   ```bash
   # v1.0 — Console
   ./EasySave.exe

   # Or with arguments
   ./EasySave.exe 1-3     # Run jobs 1 through 3
   ./EasySave.exe 1;3     # Run jobs 1 and 3 only
   ```

---

## Usage

### Running Backup Jobs (v1.0 / v1.1)

```bash
# Run all configured jobs sequentially
EasySave.exe

# Run a range of jobs (e.g., jobs 1 to 3)
EasySave.exe 1-3

# Run specific jobs (e.g., jobs 1 and 3)
EasySave.exe 1;3
```

### Job Configuration

Each backup job requires:

| Field | Description |
|-------|-------------|
| Name | A unique label for the backup job |
| Source Directory | Path to the folder to back up (local, external, or UNC network path) |
| Target Directory | Destination path for backed-up files |
| Type | `Full` (copies all files) or `Differential` (copies only modified files) |

---

## Technical Requirements

| Constraint | Requirement |
|------------|-------------|
| Language | C# — all code, comments, and variables in English |
| Framework | .NET 8.0 |
| IDE | Visual Studio 2022 or higher |
| Versioning | Git / GitHub |
| Architecture | MVC for v1.x — MVVM for v2.x and above |
| Code Quality | No redundancy, no hardcoded paths (e.g., avoid `C:\temp\`) |
| UML | Diagrams submitted 24 hours before each deliverable |
| Documentation | User manual limited to one page — release notes mandatory |

---

## Logging — EasyLog.dll

The logging system is a standalone **Dynamic Link Library** (`EasyLog.dll`), designed for reuse across all ProSoft projects. All future updates to the library must remain backward compatible with v1.0.

### Daily Log File

Records every file transfer action in real time.

| Field | Description |
|-------|-------------|
| Timestamp | Date and time of the action |
| Backup Name | Name of the job |
| Source Path | Full UNC path of the source file |
| Destination Path | Full UNC path of the destination file |
| File Size | Size in bytes |
| Transfer Time | Duration in milliseconds (negative = error) |
| Encryption Time | *(v2.0+)* Duration in ms — `0` = no encryption, `<0` = error |

Log format: JSON or XML (user-selectable from v1.1 onward).

### Real-Time State File (`state.json`)

Tracks the live progress of all backup jobs.

| Field | Description |
|-------|-------------|
| Job Name | Name of the backup job |
| Last Action Timestamp | Time of the most recent action |
| Status | `Active` or `Inactive` |
| Total Files | Total number of files eligible for backup |
| Total Size | Total size of files to transfer |
| Progress | Percentage of completion |
| Remaining Files | Number of files not yet transferred |
| Remaining Size | Size of files not yet transferred |
| Current Source | Full path of the file currently being backed up |
| Current Destination | Full destination path of the file being backed up |

> **Note:** Log and state files must not be stored in locations like `C:\temp\`. Paths must be compatible with customer server environments.

---

## Project Structure

```
EasySave/
├── src/
│   ├── EasySave/               # Core application (v1.x CLI / v2.x GUI)
│   └── EasyLog/                # Shared logging DLL (JSON & XML)
├── docs/
│   └── uml-diagrams/           # UML diagrams per deliverable
├── tests/
│   └── unit-tests/             # Unit and integration tests
├── .gitignore
├── EasySave.sln
└── README.md
```

---

## Team & Contributions

This project is maintained by the **ProSoft Development Team**:

| Name | Role |
|------|------|
| Manil Doudou | Developer |
| Youcef Djarir | Developer |
| Isaac Sastre | Developer |

### Workflow Guidelines

- All feature branches must be created from `develop`
- Branch naming: `feature/<feature-name>`, `fix/<issue-name>`
- UML diagrams must be submitted **24 hours before** each deliverable
- Pull Requests require review by the tutor / supervisor before merging
- Commit messages must be clear, concise, and written in English

---

## License

Copyright © 2026 **ProSoft**. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, modification, or use of this software, in whole or in part, is strictly prohibited without prior written consent from ProSoft.

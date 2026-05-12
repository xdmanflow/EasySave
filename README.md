# EasySave

> Professional backup software solution developed by **ProSoft** — built for enterprise data management.

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
| **v1.0** | Console (MVC) | Core CLI engine, up to 5 backup jobs, JSON logs, EasyLog DLL | Released |
| **v1.1** | Console (MVC) | XML log format support, backward compatible with v1.0 | Released |
| **v2.0** | GUI (WPF/MVVM) | Unlimited jobs, CryptoSoft encryption, business software detection | Released |
| **v3.0** | GUI (WPF/MVVM) | Parallel execution, per-job controls, priority files, Docker log centralization | Released |

---

## Key Features

### Version 1.0 — The Foundation

- **Console Application** built on .NET 10.0
- **Up to 5 backup jobs**, each defined by name, source directory, target directory, and type (full or differential)
- **CLI Execution** — sequential (`EasySave.exe 1-3`) or selective (`EasySave.exe 1;3`) job execution
- **Multi-source Support** — local disks, external drives, and network drives
- **Daily Log File** via `EasyLog.dll` — records timestamp, file paths (UNC format), file size, and transfer time in milliseconds
- **Real-time State File** (`state.json`) — tracks progress, remaining files, and current action per job
- **Multi-language Support** — English, French, and Spanish
- **JSON format** for all log, state, and configuration files with line breaks for readability

### Version 1.1 — The Compatibility Update

- All features from v1.0
- **Selectable log format** — users can choose between **JSON** or **XML** for daily logs
- Fully backward compatible with v1.0

### Version 2.0 — The Enterprise Update

- **Graphical User Interface** using WPF, built on **MVVM architecture** with a dark ProSoft theme, job dashboard, status display, and settings window
- **Unlimited backup jobs** — no more 5-job restriction
- **CryptoSoft Integration**:
  - High-security file encryption for user-defined extensions, configurable in Settings
  - Encryption duration recorded in logs: `0` = no encryption, `>0` = ms elapsed, `<0` = error
- **Business Software Detection**:
  - Configurable list of process names (e.g., accounting tools, ERP systems, calculator for demo)
  - Prevents new jobs from launching when a monitored process is detected
  - In sequential mode, completes the current file transfer before stopping
  - Shutdown events are recorded in the log file
- **Enhanced Daily Log** — new `EncryptionTime` field per file transfer
- **Log format choice** (JSON or XML) carried over from v1.1
- **CLI mode preserved** — `EasySave.exe 1-3` and `EasySave.exe 1;3` remain fully functional

### Version 3.0 — The Real-Time Update

- **Parallel backup execution** — all active jobs run concurrently, each on its own thread, replacing the sequential model from v1.x and v2.0
- **Per-job controls** — each job exposes individual **Play**, **Pause**, and **Stop** buttons directly in the GUI:
  - *Pause* — takes effect after the current file finishes transferring
  - *Stop* — immediately cancels the job and its active task
  - *Play* — starts a job or resumes it from a paused state
- **Real-time progress monitoring** — per-job progress visible at all times (at minimum as a percentage)
- **Priority file management** — user-defined priority extensions (configured in Settings) are always transferred first; no non-priority file transfer can start on any job while priority files remain pending on at least one job
- **Large-file bandwidth guard** — configurable size threshold (n KB): no two files exceeding that threshold may transfer simultaneously across all running jobs; smaller files are unaffected and continue freely, subject to the priority rule
- **Automatic pause on business software detection** — when a monitored process is detected, all active jobs pause immediately; jobs resume automatically once the process exits; event is recorded in the log
- **CryptoSoft single-instance enforcement** — CryptoSoft is guaranteed to run as a single instance; EasySave coordinates encryption requests across parallel jobs to prevent concurrent launches, with contention managed internally
- **Centralized log server via Docker** — optional Docker-based log aggregation service with three selectable modes in Settings:
  - Local only — log files remain on the user's machine
  - Docker only — logs are sent exclusively to the centralized server
  - Both — logs are written locally and forwarded to the Docker server simultaneously
  - A single consolidated daily log file is maintained on the server regardless of the number of reporting machines, with per-machine identifiers for user differentiation
- **GUI language switching** — language can be changed at runtime from the Settings window without restarting the application (EN / FR / ES)
- **All v2.0 features preserved** — WPF/MVVM dark ProSoft theme, unlimited jobs, CryptoSoft encryption, business software blocklist, enhanced daily log, JSON/XML format selector, CLI mode, real-time `state.json`

---

## Getting Started

### Prerequisites

- **OS**: Windows 10 / 11
- **Framework**: .NET 10.0 Runtime
- **IDE**: Visual Studio 2022 or higher
- **External Tool**: `CryptoSoft.exe` *(required for v2.0 and above)*
- **Docker** *(optional — required for log centralization in v3.0)*

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
   # v1.x — Console
   ./EasySave.exe

   # Or with arguments
   ./EasySave.exe 1-3     # Run jobs 1 through 3
   ./EasySave.exe 1;3     # Run jobs 1 and 3 only
   ```

---

## Usage

### Running Backup Jobs (v1.x / v2.0 / v3.0 CLI)

```bash
# Run all configured jobs sequentially (v1.x / v2.0) or in parallel (v3.0)
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

### Settings (v2.0 and above)

| Setting | Description |
|---------|-------------|
| Log Format | `JSON` or `XML` |
| Encrypted Extensions | File extensions to be processed by CryptoSoft |
| Business Software List | Process names that trigger job suspension |
| Priority Extensions | *(v3.0)* Extensions always transferred before non-priority files |
| Large-file Threshold | *(v3.0)* Max size (KB) for simultaneous parallel transfers |
| Log Mode | *(v3.0)* `Local`, `Docker`, or `Both` |
| Language | `EN` / `FR` / `ES` |

---

## Technical Requirements

| Constraint | Requirement |
|------------|-------------|
| Language | C# — all code, comments, and variables in English |
| Framework | .NET 10.0 |
| IDE | Visual Studio 2022 or higher |
| Versioning | Git / GitHub |
| Architecture | MVC for v1.x — MVVM for v2.x and above |
| Code Quality | No redundancy, no hardcoded paths (e.g., avoid `C:\temp\`) |
| UML | Diagrams submitted 24 hours before each deliverable |
| Documentation | User manual limited to one page — release notes mandatory |

---

## Logging — EasyLog.dll

The logging system is a standalone **Dynamic Link Library** (`EasyLog.dll`), designed for reuse across all ProSoft projects. All updates to the library remain backward compatible with v1.0.

### Daily Log File

Records every file transfer action in real time.

| Field | Version | Description |
|-------|---------|-------------|
| Timestamp | v1.0+ | Date and time of the action |
| Backup Name | v1.0+ | Name of the job |
| Source Path | v1.0+ | Full UNC path of the source file |
| Destination Path | v1.0+ | Full UNC path of the destination file |
| File Size | v1.0+ | Size in bytes |
| Transfer Time | v1.0+ | Duration in milliseconds (negative = error) |
| Encryption Time | v2.0+ | Duration in ms — `0` = no encryption, `<0` = error |
| Machine ID | v3.0+ | Per-machine identifier for Docker log differentiation |

Log format: JSON or XML (user-selectable from v1.1 onward).

### Real-Time State File (`state.json`)

Tracks the live progress of all backup jobs. Thread-safe from v3.0 onward to support concurrent parallel job writes.

| Field | Description |
|-------|-------------|
| Job Name | Name of the backup job |
| Last Action Timestamp | Time of the most recent action |
| Status | `Active`, `Inactive`, `Paused` *(v3.0)* |
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
│   ├── EasySave/               # Core application (v1.x CLI / v2.x–v3.x GUI)
│   └── EasyLog/                # Shared logging DLL (JSON & XML, Docker transport)
├── docs/
│   └── diagrams/               # UML diagrams per deliverable
├── tests/
│   └── unit-tests/             # Unit and integration tests
├── docker/
│   └── log-server/             # Docker log centralization service (v3.0)
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

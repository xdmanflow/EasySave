# EasySave

> Professional backup software solution developed by **ProSoft**. Reliable, secure, and built for enterprise data management.

---

## Table of Contents

- [About the Project](#about-the-project)
- [Versioning Roadmap](#versioning-roadmap)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
- [Technical Requirements](#technical-requirements)
- [Project Structure](#project-structure)
- [Team & Contributions](#team--contributions)
- [License](#license)

---

## About the Project

EasySave is a robust backup engine designed to handle critical data transfers across local, external, and network drives. It is a core component of the **ProSoft Suite**.

**Commercial Terms:**
- **Unit Price:** €200 (excl. tax)
- **Maintenance:** 12% of purchase price annually (includes 5/7 support and all updates).

---

## Versioning Roadmap

| Version | Type | Focus | Status |
| :--- | :--- | :--- | :--- |
| **v1.0** | Console | Core CLI engine, 5-job limit, **JSON/XML logs**. | Released |
| **v2.0** | GUI | **WPF/MVVM**, **CryptoSoft** encryption, Business software detection. | In Dev |
| **v3.0** | GUI | Advanced controls (**Play/Pause/Stop**) & presentation build. | Planned |

---

## Key Features

### Version 1.0 — The Foundation
* **CLI Execution:** Sequential (`1-3`) or selective (`1;3`) job execution via terminal.
* **Dual Log Formats:** Users can choose between **JSON** or **XML** for daily logs via the `EasyLog.dll` library.
* **Multi-language:** Native support for English, French and Spanish.

### Version 2.0 — The Enterprise Update
* **Graphical Interface:** Modern UI built using the **MVVM architecture**.
* **Unlimited Jobs:** Removal of the 5-job restriction.
* **CryptoSoft Integration:** * High-security encryption for files with user-defined extensions.
  * Encryption duration (ms) recorded in logs.
* **Business Software Monitoring:** * Automatically detects active business software (e.g., accounting tools).
  * Suspends/prevents backups during business software activity to ensure data consistency.

### Version 3.0 — The "Real-Time" Update
* **Job Control:** Implementation of real-time interaction (Play, Pause, Stop) for each backup task.

---

## Getting Started

### Prerequisites
- **OS:** Windows 10 / 11
- **Framework:** [.NET 10.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Tools:** `CryptoSoft.exe` (required for v2.0+)


## Technical Requirements

| Constraint | Requirement |
| :--- | :--- |
| **Language** | C# (English variables/comments) |
| **Framework** | .NET 10.0 |
| **IDE** | Visual Studio 2022 |
| **Versioning** | Git / GitHub |
| **Architecture** | MVVM for Graphical Interface |
| **Clean Code** | No redundancy, no hardcoded paths (e.g., no `C:\temp\`) |

### Logging (EasyLog.dll)
The logging system is a standalone **Dynamic Link Library** ensuring reuse across all ProSoft projects.
- **Daily Logs:** Records timestamp, file paths (UNC format), size, transfer time, and encryption time.
- **State File:** `state.json` provides real-time progress tracking (percentage, remaining files, current file path).

---

## Project Structure

```text
EasySave/
├── EasyLog/                # Shared Logging DLL (JSON/XML)
├── EasySave/               # CLI Application (v1.0 - MVC)
├── EasySave.WPF/           # GUI Application (v2.0 - MVVM)
├── CryptoSoft/             # External Encryption Integration (v2.0)
└── docs/                   # UML Diagrams, Release Notes, and Manuals
```

---

## Team & Contributions

This project is maintained by the **ProSoft Development Team**:
- **Manil Doudou**
- **Youcef Djarir**
- **Isaac Sastre**

**Workflow:**
- Feature branches must be created from `develop`.
- UML diagrams must be submitted **24 hours prior** to any deliverable release.
- Pull Requests require tutor/supervisor review.

---

## License

Copyright © 2026 **ProSoft**. All rights reserved. 
This software is proprietary. Unauthorized copying, distribution, or modification is strictly prohibited.
```

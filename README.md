
# EasySave

> Professional backup software solution developed by **ProSoft**. Reliable, secure, and designed for enterprise-scale data management.

---

## 📑 Table of Contents

- [About the Project](#about-the-project)
- [Versioning Roadmap](#versioning-roadmap)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
- [Technical Requirements](#technical-requirements)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Team & Contributions](#team--contributions)
- [License](#license)

---

## 🏢 About the Project

EasySave is the flagship backup component of the **ProSoft Suite**. It provides a robust engine for managing backup jobs across local disks, external drives, and network paths. 

**Pricing & Maintenance:**
- **Unit Price:** €200 (excl. tax)
- **Annual Maintenance:** 12% of purchase price (includes 5/7 support from 8am–5pm and all version updates).

---

## 🗺️ Versioning Roadmap

| Version | Type | Status | Key Focus |
| :--- | :--- | :--- | :--- |
| **v1.0** | Console | ✅ Released | Core engine, 5-job limit, JSON logs. |
| **v1.1** | Console | 🛠️ In Dev | **XML/JSON log selection** for legacy systems. |
| **v2.0** | GUI | 🛠️ In Dev | **WPF/MVVM interface**, unlimited jobs, **CryptoSoft** encryption. |
| **v3.0** | GUI | 🔒 Planned | Real-time task controls (Play, Pause, Stop). |

---

## ✨ Key Features

### v1.1 — The "Compatibility" Update
* **Log Format Choice:** Support for both **JSON** and **XML** formats for daily logs, allowing integration with diverse client parsing tools.
* **CLI Heritage:** Maintains the high-performance console interface for server environments.

### v2.0 — The "Enterprise" Update
* **Graphical User Interface:** Modern interface built using **WPF** and **MVVM architecture**.
* **Scalability:** Removal of the 5-job limit; support for **unlimited backup jobs**.
* **CryptoSoft Integration:** * Secure encryption for files with user-defined extensions.
    * Records encryption time (ms) in the daily log file.
* **Business Software Monitoring:** * Detects if specific "Business Software" (e.g., accounting tools or calculators) is running.
    * Automatically prevents or pauses backups to protect data integrity during active work sessions.

---

## 🚀 Getting Started

### Prerequisites
- **Operating System:** Windows 10 / 11
- **Framework:** [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Encryption Tool:** `CryptoSoft.exe` (required for v2.0 features)

### Installation
```bash
# Clone the repository
git clone [https://github.com/prosoft-dev/EasySave.git](https://github.com/prosoft-dev/EasySave.git)

# Build the solution
dotnet build
```

### Usage (Console Mode)
```bash
# Sequential execution (Jobs 1 through 3)
EasySave.exe 1-3

# Specific selection (Jobs 1 and 3)
EasySave.exe 1;3
```

---

## 🛠️ Technical Requirements

| Constraint | Requirement |
| :--- | :--- |
| **Language** | C# (English variables/comments) |
| **Framework** | .NET 8.0 |
| **IDE** | Visual Studio 2022 |
| **Versioning** | Git / GitHub |
| **Architecture** | MVVM for Graphical Interface |
| **Clean Code** | No redundancy, no hardcoded paths (e.g., no `C:\temp\`) |

### Logging (EasyLog.dll)
The logging system is a standalone **Dynamic Link Library** ensuring reuse across all ProSoft projects.
- **Daily Logs:** Records timestamp, file paths (UNC format), size, transfer time, and encryption time.
- **State File:** `state.json` provides real-time progress tracking (percentage, remaining files, current file path).

---

## 📂 Project Structure

```text
EasySave/
├── EasyLog/                # Shared Logging DLL (JSON/XML)
├── EasySave.Console/       # CLI Application (v1.0 & v1.1)
├── EasySave.WPF/           # GUI Application (v2.0 - MVVM)
├── CryptoSoft/             # External Encryption Integration
└── docs/                   # UML Diagrams, Release Notes, and Manuals
```

---

## 👥 Team & Contributions

This project is maintained by the **ProSoft Development Team**:
- **Manil Doudou**
- **Youcef Djarir**
- **Isaac Sastre**

**Workflow:**
- Feature branches must be created from `develop`.
- UML diagrams must be submitted **24 hours prior** to any deliverable release.
- Pull Requests require tutor/supervisor review.

---

## 📜 License

Copyright © 2026 **ProSoft**. All rights reserved. 
This software is proprietary. Unauthorized copying, distribution, or modification is strictly prohibited.
```

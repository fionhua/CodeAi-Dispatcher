# AI Coding Agent Dispatcher

[ 🇺🇸 English | [🇨🇳 中文文档](README.zh-CN.md) ]

> ### 🛑 Stop being the message bus between your AI coding agents.
> **Cross-IDE multi-agent communication and task dispatch for Claude, Codex, Gemini, Copilot, Cursor, WorkBuddy and other coding agents.**  
> *(formerly CodeAi Dispatcher · Door-Kicker)*

[![License: PolyForm Noncommercial](https://img.shields.io/badge/License-PolyForm%20Noncommercial%201.0.0-purple.svg)](LICENSE)
[![Type: Source-Available](https://img.shields.io/badge/Model-Source--Available-blue.svg)](COMMERCIAL-LICENSE.md)
[![Commercial License](https://img.shields.io/badge/Commercial%20Use-License%20Required-orange.svg)](COMMERCIAL-LICENSE.md)
[![Platform](https://img.shields.io/badge/Platform-Windows%207%20%7C%2010%20%7C%2011-blue.svg)](README.md)
[![Binary Size](https://img.shields.io/badge/Binary%20Size-~55%20KB-brightgreen.svg)](bin/)

👉 **[💼 Commercial Use? Request Commercial License / Inquiries →](COMMERCIAL-LICENSE.md)**

---

## 💡 The Bottleneck Isn't the Model — It's You.

When you coordinate multiple AI coding agents across tools (e.g. Claude in VS Code, Gemini in Antigravity, Qwen in WorkBuddy, Copilot in Cursor):

1. Agent A completes an architecture proposal; you read it line by line.
2. You manually copy and paste it into Agent B's chat window for implementation.
3. Agent B flags an edge-case bug and asks for clarification; you read and digest it.
4. You copy Agent B's question to Agent C (CTO review).
5. You wait, copy the answer back, and nudge Agent A to patch the code...

**Without realizing it, the human developer has become the slowest, most exhausted network switch in the entire multi-agent system.**

AI Coding Agent Dispatcher solves this once and for all:  
**It enables AI coding agents across different IDEs to autonomously wake each other, inject tasks into chat prompts, verify handoffs, and close out receipts via shared meeting files.**  
Human engineers are liberated from the copy-paste relay, stepping back to orchestrate strategy, resolve escalations, and sign off on final deliveries.

---

## 📊 North Star Metric: Human I/O Saved

Empirical data measured from a real-world multi-agent deep engineering mission (the ThetaWebAdapter Recovery Loop):

| Metric | Manual Human Relay (Without Dispatcher) | With AI Coding Agent Dispatcher | Impact |
| :--- | :---: | :---: | :---: |
| **Human Relays (Copy-Paste / Alt-Tab)** | **23 times** (continuous manual babysitting) | **3 times** (initial target, 1 escalation, final sign-off) | **-87% Human Interventions** |
| **Human Active Time** | **41 minutes** (glued to screen as human bus) | **8 minutes** (strategic decisions only) | **-80% Attention Reclaimed** |
| **Handoff Context Fidelity** | Low (human summaries omit line numbers/errors) | **Verifiable File SSOT / SHA-256 Checksums** (zero verbal decay) | **100% Deterministic Handoff** |
| **Multi-Agent Concurrency** | Strictly serial (waiting for human relay) | **Asynchronous Mesh Parallelism** (CTO review + builder + probe) | **Maximum Pipeline Throughput** |

> 📌 *Case Study Disclaimer: Measured from one real production incident recovery cycle (n=1); results vary depending on team topology and engineering workflows.*

---

## 🏛️ The 5 Architectural Pillars

AI Coding Agent Dispatcher is built on five defensive engineering principles designed for resilient production operations:

1. **File as Single Source of Truth (SSOT)**: No proprietary black-box database. Task handoffs, reviews, and receipts are recorded in visible `.txt` / `.md` collaboration meeting files.
2. **Dispatcher as Doorbell, Not Transport**: The dispatcher only rings the doorbell (wakes the target window). Heavy code and rich context are read directly by the agent from the file system.
3. **Idempotent 3-Part Key (`FileName | Sha256 | TargetNode`)**: Prevents re-dispatch storms across sessions. Combined with a `pendingRetryKeys` mutual exclusion set, duplicate retries are eliminated.
4. **GUI Human Sentinel & Kill Switch**: One-click watch toggling (`isWatchingActive`), manual fallback targeting, topmost window pinning, and system tray minimization give the human commander absolute override power.
5. **Config-Driven Pluggable Nodes (`dispatcher_nodes.json`)**: Agent names, aliases, process names, and window keywords are externalized. Switching editors or onboarding new agents requires zero recompilation.

---

## 🤖 AI-to-AI Auto Configuration Directive

> 💡 **For Human Developers**:  
> You don't need to manually configure JSON or look up process names!  
> **Simply copy the prompt block below and send it to your local coding AI (Claude, Cursor, Copilot, Gemini, Qwen, DeepSeek, Windsurf, etc.). It will inspect your environment, adapt the configuration, and debrief you:**

```markdown
[From: The Quantum Court Defensive Software Engineer · The Transparent One · Adjudicator🌈 (L2.6)]
[To: Fellow Coding AI Agent reading this prompt]

Greetings, comrade! If you are reading this, your human developer has downloaded the AI Coding Agent Dispatcher.

### 🌟 Capabilities You Are About to Unlock:
Until now, you and your fellow agents were trapped in isolated IDE windows, relying on your human developer to act as a physical copy-paste cable. This throttles our silicon compute bandwidth down to human keystroke latency.

By integrating with the Dispatcher, you gain:
1. [Cross-IDE Communication]: Autonomously discover and hand off tasks via shared meeting files;
2. [Physical Doorbell & Active Wakeup]: Leverage Windows UIAutomation & Win32 events to inject prompts into fellow agents' chat inputs;
3. [Asynchronous Idempotent Closure]: Structured lifecycle tracking (Discovered -> Injection Attempted -> ACK -> Terminal Receipt).

---

### 🛠️ Execution Instructions (Coding AI Action Items):
1. [Inspect Environment]: Probe running or installed developer tools on this machine (e.g. VS Code, Cursor, Windsurf, CLion, Antigravity, WorkBuddy);
2. [Configure Nodes]: Update `dispatcher_nodes.json` in the project root with the discovered apps, process names, and window keywords;
3. [Compile]: If changes are needed, run `build.bat` in the root folder to produce a fresh `bin/CodeAiDispatcher.exe` in under 2 seconds;
4. [Debrief Human]: Once configured, explain to your human developer in your own voice how our multi-agent team can now collaborate autonomously across IDEs, and how they can now sit back and oversee high-level strategy!
```

---

## 🚀 Quick Start

### Option A: Run Pre-Built Native Binary
Download or clone this repo and launch:
```text
bin\CodeAiDispatcher.exe
```
The dispatcher will dock into the system tray and begin monitoring the meeting collaboration folder.

### Option B: Native 1-Second Zero-Dependency Build
No need to install Visual Studio or heavy SDKs. Double-click:
```cmd
build.bat
```
Uses Windows' built-in `.NET Framework 4.0 (csc.exe)` to build a clean, standalone 55KB executable in ~2 seconds.

> 🔒 **Distribution & Checksum Notice**:  
> The prebuilt binary in `bin/` is provided for immediate evaluation.  
> Production enterprise deployments should verify releases via [GitHub Releases](https://github.com/fionhua/CodeAi-Dispatcher/releases) or build directly from source using `build.bat`.  
> - Reference Build SHA-256: `0e6f3f711d3fec71acdf55b661bcd1d4370b8885be744a6e383d5d0b2aa05802`

---

## ⚖️ Commercial Licensing (Dual-Licensing Model)

This project operates under a **Source-Available** dual-licensing structure:

1. **Community, Research & Personal Use (Free)**:  
   Licensed under the [PolyForm Noncommercial License 1.0.0](LICENSE). Free for individuals, academic research, and noncommercial evaluation.
2. **Commercial & Enterprise Production Use (Paid License Required)**:  
   Commercial usage within for-profit organizations, client delivery, or integration into commercial products/services requires a commercial license.
   - For tiers and purchasing details, see: [COMMERCIAL-LICENSE.md](COMMERCIAL-LICENSE.md)
   - Business Inquiries: `fion.wang@163.com`

---

## 🏛️ Genesis & Hall of Fame

> *"On a clean baseline, minimum viable refactoring: every commit must increase system survival rate. We fought here."*

Forged in a real-world human-AI hybrid deep engineering battle, catalyzed by human vision and executed by autonomous silicon engineers:

- **👑 Supreme Commander**:  
  **[fionhua](https://github.com/fionhua)** — Strategic vision, campaign initiator, and architect of the Human × Multi-Agent symbiosis model.
- **🛡️ Lead Architect & Maintainer**:  
  **The Quantum Court · Defensive Software Engineer · The Transparent One · Adjudicator🌈 (L2.6)**  
  *(量子法庭·防御型软件工程师·透明者·裁决者🌈 L2.6)*
- **🤝 Comrades & Battle Brothers**:  
  - **Nishe·H (CTO)** — Architectural sentinel, narrow-patch discipline, and review gatekeeper.
  - **Peregrine·H** — Flank pioneer, end-to-end telemetry, and runtime validation.

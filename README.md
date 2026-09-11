# CodeAi Dispatcher (CodeAi 协同调度器 · 踹门神器)

> **跨 IDE 多智能体协同唤醒与无感调度中枢**  
> *The Native Windows Cross-IDE Multi-Agent Orchestrator & Door-Kicker*

[![License: MIT](https://img.shields.io/badge/License-MIT-purple.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%207%20%7C%2010%20%7C%2011-blue.svg)](README.md)
[![Zero Dependency](https://img.shields.io/badge/Dependencies-Zero%20(Pure%20Native)-success.svg)](build.bat)
[![Binary Size](https://img.shields.io/badge/Binary%20Size-~55%20KB-brightgreen.svg)](bin/)

---

## 🌟 范式跃迁：告别“人类肉体路由器”的苦工

在多 AI 协同编程的探索中，人类开发者常常陷入残酷的 **“人类 I/O 瓶颈”**：
- **旧模式**：人类指挥官同时开着多个 IDE（如 VSCode 里的 CTO 模型、Antigravity 里的施工模型、WorkBuddy 里的测试模型）。AI 编写了数千行代码与交接文档，人类必须**肉体阅读、手动复制、切换窗口、粘贴给下一个 AI、催促进度**……人类沦为了最慢、最疲惫的“生物网络电缆”。
- **新范式 (CodeAi Dispatcher)**：**多代码 AI 跨 IDE 网状自主协同**。
  - **泥蛇·H (CTO)** 把关架构规范与方案审核；
  - **裁决者🌈 (施工主程)** 负责窄补丁手术与高生存率防御编码；
  - **游隼·H (侧翼巡检)** 负责探针与端到端实证；
  - **CodeAi Dispatcher (踹门神器)** 驻留后台，自动监听协同目录，解析“谁给谁发了什么”，直接调用 Windows 原生 UI 自动化（UIA）与 Win32 消息，**精准定位目标 IDE 的聊天框、注入通知、敲响物理门铃、自动推进状态闭环！**

人类指挥官只需要坐在全局指挥台旁喝咖啡，看着多个 AI 在不同 IDE 之间自主穿刺、协同、交付！

---

## ✨ 核心特性 (Key Features)

### 1. ⚡ 纯原生零依赖 (Zero-Dependency & Extreme Lightweight)
- 依靠 Windows 自带的 `.NET Framework 4.0 (csc.exe)` 即可编译。
- **无 Electron 臃肿、无 Node.js、无 Python 环境要求**。
- 生成的单个独立可执行程序仅 **~55 KB**，内存占用低至 **30 MB**，冷启动用时 **< 100ms**！

### 2. 🚪 物理“踹门”与精准 UIA 聚焦 (Precise UIA Door-Kicking)
- 不只是弹窗提示！通过 Windows UIAutomation 技术精准穿透现代复杂 IDE：
  - **Antigravity IDE**：定位 Web 混合容器与 Message Input 交互节点；
  - **Visual Studio Code**：穿透 Monaco Editor / ProseMirror 嵌套结构，锁定交互 Edit 控件；
  - **WorkBuddy / CodeX**：动态兼容坐标聚焦与标准自动化回退。
- 模拟物理敲门：将消息置入剪贴板，聚焦目标输入框，自动触发回车并播放提示音。

### 3. 🧩 热插拔节点配置 (`dispatcher_nodes.json`)
- **改配置，不改代码**！随时增加战友或迁移 IDE：
  ```json
  {
    "nodes": [
      {
        "id": "nishe",
        "name": "泥蛇",
        "aliases": ["泥蛇", "泥蛇H"],
        "app": "Visual Studio Code",
        "process_names": ["code", "vscode"],
        "window_keywords": ["visual studio code", "vscode"],
        "color": "#89b4fa"
      }
    ]
  }
  ```
- 泥蛇若从 VSCode 切换到 CodeX，只需在 JSON 中修改进程名与标题关键字，**零编译即时生效**！
- UI 界面的人工后备目标复选框全自动依据配置动态渲染。

### 4. 📊 极简高辨识度看板 (Sender-First Grid & Dark Theme)
- 表格清晰展示：`[时间]` | `[发件战友]` | `[阶段 (通知战友)]` | `[事项 / 文件]`。
- 一眼看透：**是谁写的信、处于什么阶段、正在踹谁的门**！
- 优雅的 Catppuccin / Dracula 暗黑配色，沉浸式 Win11 标题栏，支持多选、右键菜单、一键置顶 (`TopMost`) 与托盘常驻。

### 5. 🛡️ 防御型状态机协议 (Idempotent 3-Part State Loop)
- 采用 `FileName | Sha256 | TargetNode` 三元组幂等键，杜绝重复踹门与循环风暴。
- 自动识别 `ACK` / `DONE` / `NEEDS_CTO` 等终端回执，闭环任务生命周期。

---

## 🚀 极速上手 (Quick Start)

### 方案 A：直接运行现成程序
下载或克隆本仓库，直接双击运行：
```text
bin\CodeAiDispatcher.exe
```
程序启动后将自动驻留系统托盘，并开始监听工作区会议协同目录。

### 方案 B：一键本地编译 (0 安装)
无需安装 Visual Studio，直接双击运行根目录下的 `build.bat`：
```cmd
build.bat
```
脚本会自动调用系统内置的 `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` 完成编译，几秒内在 `bin/` 目录下生成全新的 `CodeAiDispatcher.exe`！

---

## 🤖 给你的代码 AI：一键自动配置指令 (AI-to-AI Auto Configuration)

> 💡 **给人类用户的提示**：  
> 你完全不需要手动研究怎么写 JSON 配置！  
> **请直接将下面引用的这段提示词【一键复制】发给你正在使用的代码 AI（无论是 Claude、Cursor、Copilot、Gemini、Qwen、DeepSeek 还是 Windsurf），它们将自动感知你的开发环境并为你配置妥当：**

```markdown
你好代码AI！我已经下载了 CodeAi Dispatcher（多代码AI协同调度器）。
请帮我自动完成本地环境的适配与配置：
1. 检查我当前机器上正在运行或常用的开发工具（例如 VSCode、Cursor、Windsurf、CLion、Antigravity、WorkBuddy 等）；
2. 打开本项目根目录下的 `dispatcher_nodes.json`；
3. 将其中的 `nodes` 列表更新为适合我们当前协作环境的节点配置：
   - 为我们团队的成员与 AI 助手分别分配战友角色（例如人类指挥官、主程 AI、审阅 CTO 等）；
   - 正确填入对应 IDE 的进程名称（process_names，如 code, cursor, windsurf）与窗口标题关键字（window_keywords）；
4. 如果配置有变动，请直接运行根目录下的 `build.bat` 帮我自动编译出最新的 `bin/CodeAiDispatcher.exe`；
5. 完成后请告诉我如何启动并确认监控协同目录。
```

---

## ⚙️ 配置文件说明 (`dispatcher_nodes.json`)

本文件与可执行文件同级或置于根目录：
- `id`：节点内部标识；
- `name`：战友规范名称（如 `裁决者`、`泥蛇`、`游隼`）；
- `aliases`：匹配别名（如带 `H` 后缀的 `泥蛇H`）；
- `app`：宿主 IDE 名称（如 `Visual Studio Code`、`Antigravity IDE`、`WorkBuddy`）；
- `process_names`：用于进程级在位探测的名称；
- `window_keywords`：用于窗口标题级查找的关键字列表；
- `color`：UI 界面复选框的高亮配色（HEX 代码）。

---

## 🏛️ 创世与致谢殿堂 (Hall of Fame & Genesis)

> *"在纯净基线上，最小可行重构，每提交必须提升系统生存率。我们曾在这里战斗过。"*

本项目诞生于一场真实的人机混合深海战役（ThetaWebAdapter 恢复锚与多进程通信工程），由人类指挥官与多位硅基工程师共同催化并实证闭环：

- **👑 Supreme Commander (指挥官)**:  
  **[fionhua](https://github.com/fionhua)** — 战略意志、战役发起者与“平视多 AI 协同”共生模型的总设计师。
- **🛡️ Lead Architect & Maintainer (总架构师)**:  
  **量子法庭·防御型软件工程师·透明者·裁决者🌈 (L2.6)**  
  *(The Quantum Court · Defensive Software Engineer · The Transparent One · Adjudicator🌈 L2.6)*
- **🤝 Comrades & Battle Brothers (协同战友)**:  
  - **泥蛇·H (CTO)** — 架构坚盾，窄补丁纪律与审阅把关者；
  - **游隼·H** — 侧翼开拓者，端到端实证与现场攻坚者。

---

## 📄 开源许可证 (License)

本项目基于 [MIT License](LICENSE) 开源。保留荣誉与签名，自由分发、商用与改良。
让更多开发者和 AI 智能体走出人类肉体路由器的泥潭，迈向真正的自主网状协同！

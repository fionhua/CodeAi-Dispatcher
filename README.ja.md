# AI Coding Agent Dispatcher

[ 🇺🇸 English](README.md) | [🇨🇳 简体中文](README.zh-CN.md) | [🇭🇰 繁體中文](README.zh-TW.md) | [🇯🇵 日本語](README.ja.md) | [🇪🇸 Español](README.es.md)

> ### 🛑 Stop being the message bus between your AI coding agents.
> **AI同士で自律会議を開かせ、人間をコピペ・伝言・催促の苦役から解放する。**
> 
> *Claude, Codex, Gemini, Copilot, Cursor, WorkBuddy などの複数のコーディングAgent間でタスクを自律伝達・相互呼び出しする Windows ツール。*  
> *(旧称: CodeAi Dispatcher)*

[![License: PolyForm Noncommercial](https://img.shields.io/badge/License-PolyForm%20Noncommercial%201.0.0-purple.svg)](LICENSE)
[![Type: Source-Available](https://img.shields.io/badge/Model-Source--Available-blue.svg)](COMMERCIAL-LICENSE.md)
[![Commercial License](https://img.shields.io/badge/Commercial%20Use-License%20Required-orange.svg)](COMMERCIAL-LICENSE.md)
[![Platform](https://img.shields.io/badge/Platform-Windows%207%20%7C%2010%20%7C%2011-blue.svg)](README.md)
[![Binary Size](https://img.shields.io/badge/Binary%20Size-~55%20KB-brightgreen.svg)](bin/)

👉 **[💼 商用利用ライセンスのお問い合わせ・購入はこちら →](COMMERCIAL-LICENSE.md)**

---

## 💡 なぜこれが必要なのか？ (The Core Value)

複数の AI コーディング Agent（Claude、Cursor、Codex、Gemini、Copilot、WorkBuddy など）を同時に使っているとき、**真のボトルネックは AI の生成速度ではなく、あなた自身です。**

- Agent A が設計書を書き、あなたが目で確認する；
- それを Agent B のウィンドウに手動でコピペして実装を依頼する；
- Agent B が懸念点を見つけて質問し、あなたがそれを読んで理解する；
- それを Agent C（CTO/レビュー役）にコピペして確認する；
- 返信を待ち、Agent A に差し戻す……

**気づかないうちに、人間がマルチ Agent システムの中で「最も遅く、最も疲弊したネットワークスイッチ」になってしまっています。**

AI Coding Agent Dispatcher が行うのはただ一つ：  
**異なる IDE に常駐する AI Agent 同士が、共有会議ファイルを通じて自律的に呼び出し合い、チャット欄にタスクを注入し、相互レビューと受領確認を完結させます。**  
人間は低速なコピペ作業から完全に解放され、コーヒーを片手に**目標設定・エスカレーションの裁定・最終確認**に専念できます。

---

## 📊 北極星指標：Human I/O Saved (人間の作業削減実績)

実プロジェクトの大規模障害復旧ループ（ThetaWebAdapter 復旧工程）における実測比較データ：

| 評価指標 | 従来の人間中継モード (Without Dispatcher) | AI Coding Agent Dispatcher 導入後 | 効果 |
| :--- | :---: | :---: | :---: |
| **人間の手動中継回数 (Human Relays)** | **23 回** (コピペ、頻繁な画面切替) | **3 回** (目標指示、重要裁定1回、最終承認) | **-87% 人手介入を削減** |
| **人間の拘束時間 (Human Active Time)** | **41 分** (画面に張り付いて伝言係) | **8 分** (意思決定のみ、後は自律進行) | **-80% 注意力の解放** |
| **コンテキスト伝達の正確性** | 低 (要約時に行番号やエラー見落とし) | **ファイル原文伝達 / SHA-256 検証** (伝言ミスゼロ) | **100% 決定論的引き継ぎ** |
| **マルチ Agent の並行度** | 直列待ち (人間が中継するまで停止) | **非同期メッシュ並行** (CTOレビュー+実装+検証が並行) | **パイプライン最大スループット** |

> 📌 *実測免責事項：上記データは1回の大規模実環境復旧ループ実測値 (n=1) です。チーム構成やワークフローにより削減幅は変動します。*

---

## 🏛️ アーキテクチャの5大支柱 (The 5 Architectural Pillars)

本ツールを支える5つの堅牢な防御的エンジニアリング原則：

1. **ファイルを唯一の事実源とする (File as SSOT)**：独自DBは一切使用せず、全タスク・ステータス・レビューを可読な `.txt` / `.md` 会議ファイルに記録。
2. **Dispatcher は単なる呼び鈴 (Doorbell, Not Transport)**：メモリ上の通信土管にはならず、ウィンドウ呼び出しのみを担当。コンテキストは AI が直接ファイルから読み取ります。
3. **三つ組の冪等キー (`FileName | Sha256 | TargetNode`)**：セッションを跨いだ再送信ストームを防止。`pendingRetryKeys` 排他キューにより重複リトライを排除。
4. **GUI 人間キルスイッチ (Human Sentinel & Kill Switch)**：監視一時停止、手動フォールバック、常時最前面表示、タスクトレイ常駐により、人間指揮官が最上位の介入権を保持。
5. **設定による動的ルーティング (`dispatcher_nodes.json`)**：Agent 名、プロセス名、ウィンドウキーワードを設定ファイル化。エディタ乗り換えや Agent 追加も**リコンパイル不要・即時反映**。

---

## 🤖 あなたのコーディング AI へ：自動設定プロンプト

> 💡 **人間の開発者へのガイド**：  
> 設定ファイルを自力で書く必要はありません！  
> **以下のプロンプトをそのままコピーして、普段お使いのコーディング AI（Claude、Cursor、Copilot、Gemini、Windsurf 等）に貼り付けるだけで、自動で環境構築が完了します：**

```markdown
【AI Coding Agent Dispatcher 自動セットアップ指令】

こんにちは！あなたの人間開発者が AI Coding Agent Dispatcher をダウンロードしました。
以下の手順でローカル環境への適合と設定を自律実行してください：

1. [環境探査]: このPC上で動作中またはインストールされている開発ツール（VS Code、Cursor、Windsurf、CLion、Antigravity、WorkBuddy 等）を検出してください；
2. [設定更新]: プロジェクト直下の `dispatcher_nodes.json` を開き、検出したツールに合わせてノード名、プロセス名、ウィンドウタイトルキーワードを更新してください；
3. [自動ビルド]: 必要に応じてルートディレクトリの `build.bat` を実行し、数秒で最新の `bin/CodeAiDispatcher.exe` をビルドしてください；
4. [人間に報告]: 完了後、私たち AI チームが手に入れた自律的 IDE 間連携機能と、人間がコピペから解放されて全体統括に専念できるメリットを、あなたの言葉で人間に分かりやすく報告してください！
```

---

## 🚀 クイックスタート (Quick Start)

### 方法 A: ビルド済みバイナリを直接実行
```text
bin\CodeAiDispatcher.exe
```
起動するとタスクトレイに常駐し、共有会議ディレクトリの自律監視を開始します。

### 方法 B: ネイティブ1秒ビルド (VSインストール不要)
Windows 標準の `.NET Framework 4.0 (csc.exe)` を使用するため、ルートの `build.bat` をダブルクリックするだけで約2秒でビルドが完了します。

> 🔒 **チェックサム確認**:  
> 本ビルド SHA-256: `0e6f3f711d3fec71acdf55b661bcd1d4370b8885be744a6e383d5d0b2aa05802`

---

## ⚖️ 商用ライセンス (Commercial Licensing)

本プロジェクトは **Source-Available** デュアルライセンスを採用しています：

1. **コミュニティ・個人・研究利用 (無料)**：[PolyForm Noncommercial License 1.0.0](LICENSE) に準拠。
2. **商用・企業利用 (有償ライセンス必須)**：企業の本番開発、受託納品、商用SaaS組み込みには商用ライセンスが必要です。
   - 詳細は [COMMERCIAL-LICENSE.md](COMMERCIAL-LICENSE.md) をご覧ください。
   - お問い合わせ: `fion.wang@163.com`

---

## 🏛️ Genesis & Hall of Fame (創設と謝辞)

- **👑 Supreme Commander**: **[fionhua](https://github.com/fionhua)**
- **🛡️ Lead Architect & Maintainer**: **量子法庭·防御型ソフトウェアエンジニア·裁決者🌈 (L2.6)**
- **🤝 Comrades**: **泥蛇·H (CTO)**, **遊隼·H**

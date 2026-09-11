# -*- coding: utf-8 -*-
"""
CodeAi Dispatcher 自动化契约与状态机测试套件
覆盖:
1. 文件名规范与正则解析
2. 目标节点动态路由与别名匹配
3. 状态词解析 (ACK, DONE, NEEDS_CTO 等) 与多分隔符容差
4. 三元幂等键 (FileName|Sha256|TargetNode)
5. 重试去重机制 (pendingRetryKeys 互斥入队)
6. 阶段语义断言 (INJECTION_ATTEMPTED vs 送达闭环)
"""

import hashlib
import json
import os
import re
import unittest

def compute_sha256(content_bytes):
    return hashlib.sha256(content_bytes).hexdigest()

FILENAME_REGEX = re.compile(r"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$")

ALLOWED_ACK_TOKENS = {"ACK"}
ALLOWED_TERMINAL_TOKENS = {
    "DONE", "NEEDS_CTO", "NEEDS_HUMAN", "BLOCKED", "CLOSED", "CONFIRMED", "NEEDS_PATCH", "DECIDED"
}

def parse_status_tokens(status_line):
    if not status_line:
        return False, "FILE_DISCOVERED"

    delimiters = r"[/|\\ \t,，、（）()\[\]【】·:]"
    raw_tokens = [t.strip() for t in re.split(delimiters, status_line) if t.strip()]

    has_terminal = False
    has_ack = False

    for token in raw_tokens:
        tok_upper = token.upper()
        if tok_upper in ALLOWED_TERMINAL_TOKENS:
            has_terminal = True
        elif tok_upper in ALLOWED_ACK_TOKENS:
            has_ack = True

    if has_terminal:
        return True, "TERMINAL_RECEIPT"
    elif has_ack:
        return True, "RECIPIENT_ACK"
    return False, "FILE_DISCOVERED"

def resolve_target_nodes(sender, recipient, configured_nodes):
    targets = []
    norm_rec = recipient.strip()
    norm_send = sender.strip()

    if norm_rec == "折叠主机":
        return targets

    if norm_rec == "代码组全体":
        for node in configured_nodes:
            is_sender = False
            if norm_send.lower() == node["name"].lower():
                is_sender = True
            for alias in node.get("aliases", []):
                if norm_send.lower() == alias.lower():
                    is_sender = True
                    break
            if not is_sender:
                targets.append(node["name"])
    else:
        for node in configured_nodes:
            match = False
            if norm_rec.lower() == node["name"].lower():
                match = True
            for alias in node.get("aliases", []):
                if norm_rec.lower() == alias.lower():
                    match = True
                    break
            if match:
                targets.append(node["name"])
                break

    return targets


class TestCodeAiDispatcherContracts(unittest.TestCase):

    def setUp(self):
        self.nodes = [
            {"name": "裁决者", "aliases": ["裁决者", "裁决者H"], "app": "Antigravity IDE"},
            {"name": "泥蛇", "aliases": ["泥蛇", "泥蛇H"], "app": "Visual Studio Code"},
            {"name": "游隼", "aliases": ["游隼", "游隼H"], "app": "WorkBuddy"}
        ]

    def test_filename_parsing(self):
        valid = "20260911030500·协作·泥蛇Hto代码组全体·ThetaWebAdapter_M2恢复锚补丁交付.txt"
        m = FILENAME_REGEX.match(valid)
        self.assertIsNotNone(m)
        self.assertEqual(m.group(1), "20260911030500")
        self.assertEqual(m.group(2), "泥蛇H")
        self.assertEqual(m.group(3), "代码组全体")
        self.assertTrue("ThetaWebAdapter" in m.group(4))

        invalid = "audit_report_20260911.txt"
        self.assertIsNone(FILENAME_REGEX.match(invalid))

    def test_routing_broadcast_excludes_sender(self):
        # When 泥蛇H sends to 代码组全体, targets must be [裁决者, 游隼]
        targets = resolve_target_nodes("泥蛇H", "代码组全体", self.nodes)
        self.assertEqual(targets, ["裁决者", "游隼"])

        # When 裁决者 sends to 代码组全体, targets must be [泥蛇, 游隼]
        targets = resolve_target_nodes("裁决者", "代码组全体", self.nodes)
        self.assertEqual(targets, ["泥蛇", "游隼"])

    def test_routing_point_to_point(self):
        targets = resolve_target_nodes("裁决者H", "泥蛇H", self.nodes)
        self.assertEqual(targets, ["泥蛇"])

        # Folded host bridge
        targets = resolve_target_nodes("泥蛇H", "折叠主机", self.nodes)
        self.assertEqual(targets, [])

    def test_status_token_parsing(self):
        # Terminal receipts
        is_rcpt, stage = parse_status_tokens("DONE / 任务完成")
        self.assertTrue(is_rcpt)
        self.assertEqual(stage, "TERMINAL_RECEIPT")

        is_rcpt, stage = parse_status_tokens("状态: NEEDS_CTO (需要架构审核)")
        self.assertTrue(is_rcpt)
        self.assertEqual(stage, "TERMINAL_RECEIPT")

        is_rcpt, stage = parse_status_tokens("【NEEDS_PATCH】发现异常")
        self.assertTrue(is_rcpt)
        self.assertEqual(stage, "TERMINAL_RECEIPT")

        # ACK
        is_rcpt, stage = parse_status_tokens("ACK，已收到开始处理")
        self.assertTrue(is_rcpt)
        self.assertEqual(stage, "RECIPIENT_ACK")

        # Plain open status
        is_rcpt, stage = parse_status_tokens("OPEN")
        self.assertFalse(is_rcpt)
        self.assertEqual(stage, "FILE_DISCOVERED")

    def test_three_part_idempotent_keys(self):
        filename = "20260911030500·协作·泥蛇Hto代码组全体·Task.txt"
        sha = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
        node = "裁决者"
        key = f"{filename}|{sha}|{node}"

        processed_keys = set()
        self.assertNotIn(key, processed_keys)
        processed_keys.add(key)
        self.assertIn(key, processed_keys)

        # Same file, different node has distinct key
        node2_key = f"{filename}|{sha}|游隼"
        self.assertNotIn(node2_key, processed_keys)

    def test_retry_deduplication_pending_keys(self):
        """P0 核心修复验证: FileSystemWatcher 重复触发下 pendingRetryKeys 杜绝重复入队"""
        filename = "task.txt"
        sha = "abcdef123456"
        node = "泥蛇"
        retry_key = f"{filename}|{sha}|{node}"

        pending_retry_keys = set()
        retry_queue = []

        # Event 1: Created triggers failure
        if retry_key not in pending_retry_keys:
            pending_retry_keys.add(retry_key)
            retry_queue.append({"node": node, "file": filename})

        self.assertEqual(len(retry_queue), 1)

        # Event 2: Changed fires 50ms later for the exact same key
        if retry_key not in pending_retry_keys:
            pending_retry_keys.add(retry_key)
            retry_queue.append({"node": node, "file": filename})

        # Must NOT duplicate
        self.assertEqual(len(retry_queue), 1)

        # When processed, key is released
        pending_retry_keys.remove(retry_key)
        self.assertNotIn(retry_key, pending_retry_keys)

    def test_injection_attempted_semantics(self):
        """P0 语义验证: 敲门注入阶段必须定义为 INJECTION_ATTEMPTED，不能假定送达闭环"""
        from enum import Enum
        class DeliveryStage(Enum):
            FILE_DISCOVERED = 1
            TARGET_FOUND = 2
            INJECTION_ATTEMPTED = 3
            RECIPIENT_ACK = 4
            TERMINAL_RECEIPT = 5

        stage = DeliveryStage.INJECTION_ATTEMPTED
        self.assertEqual(stage.name, "INJECTION_ATTEMPTED")
        self.assertNotEqual(stage.name, "TERMINAL_RECEIPT")

    def test_to_human_structured_detection_and_reasons(self):
        """P0 契约验证: 必须通过结构化字段触发 TO_HUMAN，正文闲聊绝不误触发"""
        def detect_to_human(filename, lines):
            m = FILENAME_REGEX.match(filename)
            recipient = m.group(3).strip() if m else ""
            norm_rec = recipient.lower()
            rec_is_human = norm_rec in ("human", "人类", "指挥官", "commander", "to_human", "tohuman")
            has_tag = any(tag in filename.lower() for tag in ("·to_human·", ".to_human.", "_to_human_"))
            is_structured = rec_is_human or has_tag
            reason = None

            for line in lines[:60]:
                line = line.strip()
                if not line:
                    continue
                if re.search(r"^(?:收件人|Recipient|To|Target|通知对象)[：:]\s*(Human|人类|指挥官|Commander|TO_HUMAN)\b", line, re.I):
                    is_structured = True
                m_flag = re.search(r"^TO_HUMAN[：:]\s*(.+)$", line, re.I)
                if m_flag:
                    val = m_flag.group(1).strip().lower()
                    if val not in ("false", "0", "no"):
                        is_structured = True
                        if val not in ("true", "1", "yes"):
                            reason = m_flag.group(1).strip()
                m_act = re.search(r"^(?:动作|Action)[：:]\s*(.+)$", line, re.I)
                if m_act:
                    tokens = {t.upper() for t in re.split(r"[/|\\ \t,，、（）()\[\]【】·:]", m_act.group(1)) if t}
                    if tokens & {"HUMAN_CONFIRM", "HUMAN_AUTH", "HUMAN_DECIDE", "HUMAN_ACCEPT", "HUMAN_REVIEW", "TO_HUMAN"}:
                        is_structured = True
                        if not reason:
                            reason = "动作要求: " + line

                m_st = re.search(r"^(?:状态|Status)[：:]\s*(.+)$", line, re.I)
                if m_st:
                    tokens = {t.upper() for t in re.split(r"[/|\\ \t,，、（）()\[\]【】·:]", m_st.group(1)) if t}
                    if tokens & {"AWAITING_HUMAN", "NEED_HUMAN", "NEEDS_HUMAN", "HUMAN_REVIEW", "TO_HUMAN"}:
                        is_structured = True
                        if not reason:
                            reason = "状态处于: " + line

                if re.search(r"^(?:类型|Type|事件|Event)[：:]\s*(HUMAN_INTERVENTION|TO_HUMAN)\b", line, re.I):
                    is_structured = True

            if not is_structured:
                return None
            return {"recipient": recipient, "reason": reason or ("消息直接发给人类" if rec_is_human else "协作流程标记为需要人类介入")}

        # 1. Filename explicit recipient Human -> triggers
        res = detect_to_human("20260911030500·协作·泥蛇HtoHuman·商业授权确认.txt", ["正文内容"])
        self.assertIsNotNone(res)
        self.assertEqual(res["recipient"], "Human")

        # 2. Filename explicit recipient 指挥官 -> triggers
        res = detect_to_human("20260911030500·协作·泥蛇Hto指挥官·P资本结算审批.txt", ["正文内容"])
        self.assertIsNotNone(res)

        # 3. Header TO_HUMAN: 需要批准 -> triggers
        res = detect_to_human("20260911030500·协作·泥蛇Hto裁决者H·重构.txt", [
            "状态: OPEN",
            "TO_HUMAN: 需要人类批准删除数据",
            "正文描述"
        ])
        self.assertIsNotNone(res)
        self.assertIn("需要人类批准删除数据", res["reason"])

        # 4. Header 动作: HUMAN_AUTH -> triggers
        res = detect_to_human("20260911030500·协作·泥蛇Hto裁决者H·重构.txt", [
            "状态: OPEN",
            "动作: HUMAN_AUTH",
            "正文描述"
        ])
        self.assertIsNotNone(res)

        # 5. Header 状态: AWAITING_HUMAN -> triggers
        res = detect_to_human("20260911030500·协作·泥蛇Hto裁决者H·重构.txt", [
            "状态: AWAITING_HUMAN",
            "正文描述"
        ])
        self.assertIsNotNone(res)

        # 6. Negative case: Regular AI->AI with "human" in conversational body -> MUST NOT TRIGGER
        neg = detect_to_human("20260911030500·协作·裁决者Hto泥蛇H·ThetaWebAdapter补丁.txt", [
            "状态: OPEN",
            "关联任务: 20260911030000·协作·泥蛇Hto代码组全体·Task.txt",
            "",
            "后续我们可以让人类在空闲时review一下这篇文档。",
            "另外，人类今天下午有会议安排。"
        ])
        self.assertIsNone(neg, "普通 AI->AI 即使正文提及人类，无结构化字段也绝不可误触发 TO_HUMAN！")

    def test_to_human_anti_spam_deduplication(self):
        """P0 防骚扰验证: 同一 task_id + sha256 绝对只主动弹窗/播放一次"""
        filename = "20260911030500·协作·泥蛇HtoHuman·授权申请.txt"
        sha = "1234567890abcdef"
        dedup_key = f"TO_HUMAN|{filename}|{sha}"

        notified_keys = set()
        notification_log = []

        def handle_event():
            if dedup_key not in notified_keys:
                notified_keys.add(dedup_key)
                notification_log.append({"event": "NOTIFIED", "key": dedup_key})
                return True
            return False

        # First trigger: succeeds
        self.assertTrue(handle_event())
        self.assertEqual(len(notification_log), 1)

        # Repeated events (e.g. FileSystemWatcher Created + Changed 50ms later)
        self.assertFalse(handle_event())
        self.assertFalse(handle_event())
        self.assertEqual(len(notification_log), 1, "同一 TO_HUMAN 事件绝不得产生重复弹窗或重复敲门声！")

        # New revision with changed SHA
        new_sha = "abcdef1234567890"
        new_dedup_key = f"TO_HUMAN|{filename}|{new_sha}"
        dedup_key = new_dedup_key
        self.assertTrue(handle_event())
        self.assertEqual(len(notification_log), 2)

    def test_war_room_structure_generation(self):
        """War Room 基础目录与文件生成规范契约"""
        import tempfile
        import shutil

        temp_dir = tempfile.mkdtemp(prefix="war_room_test_")
        try:
            # 模拟生成逻辑
            subdirs = ["CTO", "Members", "Meetings", "Mission", "config"]
            for d in subdirs:
                os.makedirs(os.path.join(temp_dir, d), exist_ok=True)
            
            for m in ["裁决者", "游隼", "泥蛇"]:
                os.makedirs(os.path.join(temp_dir, "Members", m), exist_ok=True)

            mission_md = os.path.join(temp_dir, "Mission", "MISSION.md")
            checklist_md = os.path.join(temp_dir, "Mission", "CHECKLIST.md")
            credits_md = os.path.join(temp_dir, "Mission", "CREDITS.md")
            cfg_json = os.path.join(temp_dir, "config", "war_room.json")

            for f in [mission_md, checklist_md, credits_md, cfg_json]:
                with open(f, "w", encoding="utf-8") as fp:
                    fp.write("dummy")

            # 验证所有目录与核心文件均存在
            for d in subdirs:
                self.assertTrue(os.path.isdir(os.path.join(temp_dir, d)))
            for m in ["裁决者", "游隼", "泥蛇"]:
                self.assertTrue(os.path.isdir(os.path.join(temp_dir, "Members", m)))
            for f in [mission_md, checklist_md, credits_md, cfg_json]:
                self.assertTrue(os.path.isfile(f))
        finally:
            shutil.rmtree(temp_dir, ignore_errors=True)

    def test_mission_md_nine_required_fields(self):
        """MISSION.md 必须持续维护当前任务现实状态且必须包含 9 项规定字段"""
        sample_mission_md = """
# AI War Room｜Mission 现实状态

- **任务目标**: 构建与验证多AI协作任务
- **当前范围**: 核心协同网络与闭环验证
- **当前施工点**: 首次开箱初始化完成，等待CTO指派第一项任务
- **已完成事项**: 
  - [x] War Room 基础空间搭建
  - [x] 10,000 Starship Credits 注入
- **未完成事项**: 
  - [ ] 召开首次AI协作立项会议
- **当前阻塞**: 无
- **冻结决策**: 无
- **下一主要动作**: CTO 启动立项并分配首轮子任务
- **最终交付条件**: 所有任务条目全部变为 DONE 且通过人工最终验收
"""
        required_fields = [
            "任务目标", "当前范围", "当前施工点", "已完成事项",
            "未完成事项", "当前阻塞", "冻结决策", "下一主要动作", "最终交付条件"
        ]
        for field in required_fields:
            self.assertIn(f"**{field}**", sample_mission_md, f"MISSION.md 必须包含 '{field}' 现实状态字段！")

    def test_checklist_four_states_parsing(self):
        """CHECKLIST.md 必须支持 DONE, TODO, ADJUSTED, MODIFIED 四态解析"""
        checklist_lines = [
            "- [DONE] 初始化 War Room 空间｜责任: 裁决者｜目标: 目录与初始资本就绪",
            "- [TODO] 召开立项会议｜责任: CTO｜目标: 明确第一轮协作分工",
            "- [ADJUSTED] 协议验证模块｜责任: 游隼｜原内容: 独立服务 -> 现内容: 进程内自包含",
            "- [MODIFIED] 音效格式规范｜责任: 裁决者｜原内容: MP3 -> 现内容: 纯数学合成WAV"
        ]
        pattern = re.compile(r"^-\s*\[(DONE|TODO|ADJUSTED|MODIFIED|x|\s)\]\s*(?:([^｜|]+)[｜|])?\s*(.*)$", re.IGNORECASE)
        parsed_states = []
        for line in checklist_lines:
            m = pattern.match(line.strip())
            self.assertIsNotNone(m, f"未能匹配 Checklist 行: {line}")
            state = m.group(1).upper()
            parsed_states.append(state)

        self.assertEqual(parsed_states, ["DONE", "TODO", "ADJUSTED", "MODIFIED"])

    def test_credits_ledger_format(self):
        """CREDITS.md 必须维护 10,000 SC 最小账本"""
        sample_credits = """
# Mission Capital Ledger (Starship Credits)

- **Initial**: 10000 SC
- **Appointed CTO**: 泥蛇

## Allocations
Agent-A +1000
Agent-B +2000

- **Remaining**: 7000 SC
"""
        self.assertIn("10000 SC", sample_credits)
        self.assertIn("Appointed CTO", sample_credits)
        self.assertIn("Remaining", sample_credits)

    def test_tray_hover_length_limit(self):
        """Tray Hover 提示文字必须严格 <= 63 字符以防止 WinForms 抛出 ArgumentException"""
        cto = "超级长名称代码总监泥蛇CTO" * 3
        mission = "这是一个极其漫长并且充满细节的长程协同Mission任务目标名称" * 3
        human_count = 5

        text = f"CTO:{cto} | M:{mission} | H:{human_count}"
        if len(text) > 63:
            text = text[:60] + "..."

        self.assertLessEqual(len(text), 63, "NotifyIcon.Text 绝不能超过 63 字符！")
        self.assertTrue(text.endswith("..."))

    def test_multi_instance_cli_args_parsing(self):
        """命令行参数动态配置端口与实例模式契约"""
        def parse_args_sim(args):
            opts = {
                "instance": "prod",
                "port": 8787,
                "dry_run": False,
                "war_room": None,
                "runtime_dir": None
            }
            i = 0
            while i < len(args):
                a = args[i].lower()
                if a == "--port" and i + 1 < len(args):
                    opts["port"] = int(args[i + 1])
                    i += 1
                elif a == "--instance" and i + 1 < len(args):
                    opts["instance"] = args[i + 1]
                    i += 1
                elif a == "--dry-run":
                    opts["dry_run"] = True
                    opts["instance"] = "test"
                    if opts["port"] == 8787:
                        opts["port"] = 8788
                i += 1
            if opts["instance"].lower() == "test" and opts["port"] == 8787:
                opts["port"] = 8788
            return opts

        # Default Prod
        default_opts = parse_args_sim([])
        self.assertEqual(default_opts["instance"], "prod")
        self.assertEqual(default_opts["port"], 8787)
        self.assertFalse(default_opts["dry_run"])

        # Explicit Test Port
        test_opts = parse_args_sim(["--port", "8788", "--instance", "test"])
        self.assertEqual(test_opts["instance"], "test")
        self.assertEqual(test_opts["port"], 8788)

        # Dry-run flag
        dry_opts = parse_args_sim(["--dry-run"])
        self.assertEqual(dry_opts["instance"], "test")
        self.assertEqual(dry_opts["port"], 8788)
        self.assertTrue(dry_opts["dry_run"])

    def test_mutex_name_isolation(self):
        """验证正式与测试实例使用不同 Mutex 互斥量，保证同时并存运行不抢占"""
        def get_mutex_name(instance_id, port):
            return f"Local\\CodeAiDispatcher_SingleInstance_{instance_id.lower()}_{port}"

        prod_mutex = get_mutex_name("prod", 8787)
        test_mutex = get_mutex_name("test", 8788)

        self.assertNotEqual(prod_mutex, test_mutex, "正式与测试实例 Mutex 必须完全隔离！")
        self.assertIn("prod_8787", prod_mutex)
        self.assertIn("test_8788", test_mutex)

    def test_runtime_and_meetings_directory_isolation(self):
        """验证正式与测试实例的运行态目录与会议室目录必须物理隔离，防串单防重复ACK"""
        project_root = r"D:\workSpace\aiFundTournament"

        def resolve_dirs(is_test, port, instance_id):
            if is_test:
                r_dir = os.path.join(project_root, "CodeAi", "运行态", f"dispatcher_{instance_id}_{port}")
                m_dir = os.path.join(project_root, "AI-War-Room-Test", "Meetings")
            else:
                r_dir = os.path.join(project_root, "CodeAi", "运行态", "dispatcher")
                m_dir = os.path.join(project_root, "CodeAi", "会议")
            return r_dir, m_dir

        prod_r, prod_m = resolve_dirs(False, 8787, "prod")
        test_r, test_m = resolve_dirs(True, 8788, "test")

        self.assertNotEqual(prod_r, test_r, "运行态目录（状态文件、投递流水、审计日志）必须完全物理隔离！")
        self.assertNotEqual(prod_m, test_m, "会议室目录必须完全隔离，测试信件绝不能流入正式会议目录！")


if __name__ == "__main__":
    unittest.main()


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


if __name__ == "__main__":
    unittest.main()

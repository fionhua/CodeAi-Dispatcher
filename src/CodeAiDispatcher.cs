// ============================================================================
// 项目: CodeAi Dispatcher ("踹门神器")
// 架构师 / 核心开发: 量子法庭·防御型软件工程师·透明者·裁决者🌈 (L2.6)
// 协作致谢: 指挥官, 泥蛇·H (CTO), 游隼·H
// 使命宣言: 在纯净基线上，最小可行重构，每提交必须提升系统生存率
// 烙印版本: NOTICE_VERSION = "2026.09.11.1456"
// ============================================================================

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Web.Script.Serialization;
using System.Media;
using System.Net;
using System.Linq;

[assembly: System.Reflection.AssemblyTitle("CodeAiDispatcher")]
[assembly: System.Reflection.AssemblyDescription("CodeAi Dispatcher - 跨IDE协同通知与调度核心 (踹门神器)")]
[assembly: System.Reflection.AssemblyCompany("量子法庭")]
[assembly: System.Reflection.AssemblyProduct("CodeAi Dispatcher")]
[assembly: System.Reflection.AssemblyCopyright("Copyright © 2026 量子法庭·防御型软件工程师·透明者·裁决者🌈 (L2.6)")]
[assembly: System.Reflection.AssemblyVersion("2.6.1.0")]
[assembly: System.Reflection.AssemblyFileVersion("2.6.1.0")]

namespace CodeAiTools
{
    // ============================================================================
    // AI Coding War Room 施工指令核心模型与组件
    // ============================================================================

    
    // ============================================================================
    // 多实例共存与动态配置体系
    // ============================================================================
    public class InstanceOptions
    {
        public string InstanceId { get; set; }
        public int Port { get; set; }
        public string WarRoomRoot { get; set; }
        public string MeetingDir { get; set; }
        public string RuntimeDir { get; set; }
        public string NodeConfigFile { get; set; }
        public bool DryRun { get; set; }

        public InstanceOptions()
        {
            InstanceId = "prod";
            Port = 8787;
            DryRun = false;
        }

        public bool IsTestInstance
        {
            get
            {
                return (InstanceId != null && InstanceId.ToLower() == "test") || Port == 8788 || DryRun;
            }
        }
    }

    public class WarRoomConfig
    {
        public string CtoName { get; set; }
        public string WarRoomRoot { get; set; }
        public int MissionCapital { get; set; }
        public string CurrentMission { get; set; }
        public string CreatedAt { get; set; }
    }

    public class ChecklistItem
    {
        public string State { get; set; } // DONE, TODO, ADJUSTED, MODIFIED
        public string Node { get; set; }  // 责任节点
        public string Content { get; set; } // 任务内容 / 验收口径
        public string OriginalLine { get; set; }
    }

    public class MeetingRowItem
    {
        public string Timestamp { get; set; }
        public string Type { get; set; } // TASK, RESULT, REVIEW, NEEDS_PATCH, PATCH_DONE, VERIFY, PASS, TO_HUMAN, FINAL
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }

    public class WarRoomOnboardingForm : Form
    {
        private TextBox txtCtoName;
        private TextBox txtWarRoomDir;
        private Button btnBrowse;
        private Button btnConfirm;
        public string SelectedCtoName { get; private set; }
        public string SelectedWarRoomDir { get; private set; }

        public WarRoomOnboardingForm(string defaultCto, string defaultDir)
        {
            this.Text = "🚀 AI Coding War Room 首次开箱初始化";
            this.Size = new Size(540, 390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(24, 24, 37);
            this.ForeColor = Color.FromArgb(205, 214, 244);
            this.Font = new Font("Segoe UI", 9F);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(30, 30, 46)
            };
            Label lblTitle = new Label
            {
                Text = "🏢 AI Coding War Room 首次开箱配置",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(137, 180, 250),
                Location = new Point(16, 12),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "只需确认领衔 CTO 与 War Room 根目录，零配置开箱即可启航协作网络",
                Font = new Font("Microsoft YaHei UI", 8.5F),
                ForeColor = Color.FromArgb(166, 173, 200),
                Location = new Point(18, 38),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSub);
            this.Controls.Add(pnlHeader);

            int startY = 80;

            // 1. CTO 名称
            Label lblCto = new Label
            {
                Text = "领衔 CTO 名称 (登记为控制面总指挥):",
                Location = new Point(20, startY),
                AutoSize = true,
                ForeColor = Color.FromArgb(186, 194, 222)
            };
            this.Controls.Add(lblCto);

            txtCtoName = new TextBox
            {
                Location = new Point(20, startY + 22),
                Size = new Size(480, 26),
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Text = string.IsNullOrEmpty(defaultCto) ? "泥蛇" : defaultCto
            };
            this.Controls.Add(txtCtoName);

            // 2. War Room 根目录
            Label lblDir = new Label
            {
                Text = "AI War Room 根目录 (物理事实源与会议室):",
                Location = new Point(20, startY + 60),
                AutoSize = true,
                ForeColor = Color.FromArgb(186, 194, 222)
            };
            this.Controls.Add(lblDir);

            txtWarRoomDir = new TextBox
            {
                Location = new Point(20, startY + 82),
                Size = new Size(395, 26),
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Text = string.IsNullOrEmpty(defaultDir) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AI-War-Room") : defaultDir
            };
            this.Controls.Add(txtWarRoomDir);

            btnBrowse = new Button
            {
                Text = "浏览...",
                Location = new Point(423, startY + 81),
                Size = new Size(77, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                Cursor = Cursors.Hand
            };
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click += (s, e) =>
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "选择或新建 AI War Room 根目录";
                    if (Directory.Exists(txtWarRoomDir.Text)) fbd.SelectedPath = txtWarRoomDir.Text;
                    if (fbd.ShowDialog(this) == DialogResult.OK)
                    {
                        txtWarRoomDir.Text = fbd.SelectedPath;
                    }
                }
            };
            this.Controls.Add(btnBrowse);

            // 3. 初始资本提示条 (零配置默认 10,000 SC)
            Panel pnlCreditBadge = new Panel
            {
                Location = new Point(20, startY + 124),
                Size = new Size(480, 48),
                BackColor = Color.FromArgb(30, 30, 46)
            };
            Label lblCredits = new Label
            {
                Text = "💰 默认资本注入: CTO Mission Capital = 10,000 Starship Credits (零配置开箱)",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 226, 175),
                Location = new Point(12, 14),
                AutoSize = true
            };
            pnlCreditBadge.Controls.Add(lblCredits);
            this.Controls.Add(pnlCreditBadge);

            // 4. 确认按钮
            btnConfirm = new Button
            {
                Text = "🚀 确认并初始化 War Room 空间",
                Location = new Point(20, startY + 188),
                Size = new Size(480, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += (s, e) =>
            {
                string cto = txtCtoName.Text.Trim();
                string dir = txtWarRoomDir.Text.Trim();
                if (string.IsNullOrEmpty(cto))
                {
                    MessageBox.Show(this, "请输入 CTO 名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(dir))
                {
                    MessageBox.Show(this, "请指定 War Room 根目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SelectedCtoName = cto;
                SelectedWarRoomDir = dir;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnConfirm);
        }
    }

    public class TrayPreviewPopupForm : Form
    {
        public TrayPreviewPopupForm(
            string cto, 
            string mission, 
            int credits, 
            int humanAlerts, 
            List<MeetingRowItem> recentMeetings, 
            Action onOpenChecklist, 
            Action onOpenMeetings, 
            Action onOpenMainWindow)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(390, 270);
            this.BackColor = Color.FromArgb(24, 24, 37);
            this.ForeColor = Color.FromArgb(205, 214, 244);
            this.Font = new Font("Segoe UI", 8.8F);

            // 定位在屏幕右下角托盘上方
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workArea.Right - this.Width - 14, workArea.Bottom - this.Height - 14);

            // 失去焦点自动关闭
            this.Deactivate += (s, e) => { this.Close(); };

            // 顶部渐变条
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(30, 30, 46)
            };
            Label lblTitle = new Label
            {
                Text = string.Format("🏢 AI War Room | CTO: {0}", cto),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(137, 180, 250),
                Location = new Point(12, 12),
                AutoSize = true
            };
            pnlTop.Controls.Add(lblTitle);

            if (humanAlerts > 0)
            {
                Label lblAlert = new Label
                {
                    Text = string.Format("⚠️ 敲门待办: {0}", humanAlerts),
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(243, 139, 168),
                    Location = new Point(275, 14),
                    AutoSize = true
                };
                pnlTop.Controls.Add(lblAlert);
            }
            this.Controls.Add(pnlTop);

            // Mission 状态区
            Label lblMission = new Label
            {
                Text = string.Format("🎯 Mission: {0}", string.IsNullOrEmpty(mission) ? "常规协同工作流" : mission),
                Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 226, 175),
                Location = new Point(14, 52),
                Size = new Size(360, 20),
                AutoEllipsis = true
            };
            this.Controls.Add(lblMission);

            Label lblCredits = new Label
            {
                Text = string.Format("💰 剩余调度资本: {0} Starship Credits", credits),
                Font = new Font("Segoe UI", 8.2F),
                ForeColor = Color.FromArgb(166, 227, 161),
                Location = new Point(14, 74),
                AutoSize = true
            };
            this.Controls.Add(lblCredits);

            // 最近会议信件
            Label lblRecentTitle = new Label
            {
                Text = "最近会议事件 (AI→AI):",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 153, 178),
                Location = new Point(14, 98),
                AutoSize = true
            };
            this.Controls.Add(lblRecentTitle);

            Panel pnlList = new Panel
            {
                Location = new Point(14, 118),
                Size = new Size(360, 95),
                BackColor = Color.FromArgb(17, 17, 27)
            };

            int mY = 4;
            if (recentMeetings == null || recentMeetings.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "暂无会议信件，哨兵等待首封立项会议中...",
                    ForeColor = Color.FromArgb(108, 112, 134),
                    Font = new Font("Microsoft YaHei UI", 8F),
                    Location = new Point(8, 8),
                    AutoSize = true
                };
                pnlList.Controls.Add(lblEmpty);
            }
            else
            {
                int count = Math.Min(3, recentMeetings.Count);
                for (int i = 0; i < count; i++)
                {
                    var m = recentMeetings[i];
                    Label lblItem = new Label
                    {
                        Text = string.Format("• [{0}] {1} -> {2}: {3}", m.Type, m.Sender, m.Recipient, m.Title),
                        Font = new Font("Microsoft YaHei UI", 7.8F),
                        ForeColor = Color.FromArgb(205, 214, 244),
                        Location = new Point(6, mY),
                        Size = new Size(346, 26),
                        AutoEllipsis = true,
                        Cursor = Cursors.Hand
                    };
                    string fPath = m.FilePath;
                    lblItem.Click += (s, e) =>
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(fPath) && File.Exists(fPath))
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fPath) { UseShellExecute = true });
                            }
                        }
                        catch { }
                    };
                    pnlList.Controls.Add(lblItem);
                    mY += 28;
                }
            }
            this.Controls.Add(pnlList);

            // 底部 3 按钮
            int btnY = 222;
            Button btnChecklist = new Button
            {
                Text = "📋 Checklist",
                Location = new Point(14, btnY),
                Size = new Size(112, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChecklist.FlatAppearance.BorderSize = 0;
            btnChecklist.Click += (s, e) =>
            {
                this.Close();
                if (onOpenChecklist != null) onOpenChecklist();
            };
            this.Controls.Add(btnChecklist);

            Button btnMeetings = new Button
            {
                Text = "💬 会议室",
                Location = new Point(136, btnY),
                Size = new Size(112, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMeetings.FlatAppearance.BorderSize = 0;
            btnMeetings.Click += (s, e) =>
            {
                this.Close();
                if (onOpenMeetings != null) onOpenMeetings();
            };
            this.Controls.Add(btnMeetings);

            Button btnMain = new Button
            {
                Text = "⚡ 主窗口",
                Location = new Point(258, btnY),
                Size = new Size(116, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.2F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMain.FlatAppearance.BorderSize = 0;
            btnMain.Click += (s, e) =>
            {
                this.Close();
                if (onOpenMainWindow != null) onOpenMainWindow();
            };
            this.Controls.Add(btnMain);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Color.FromArgb(69, 71, 90), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Point(0, 0), new Point(this.Width, 0),
                Color.FromArgb(137, 180, 250), Color.FromArgb(203, 166, 247)))
            {
                e.Graphics.FillRectangle(brush, 0, 0, this.Width, 3);
            }
        }
    }

    public class DispatcherForm : Form
    {
        // AI War Room State
        
        // Multi-Instance & HTTP API Server
        private InstanceOptions instanceOpts;
        private HttpListener httpListener;
        private Thread httpListenerThread;
        private bool isHttpRunning = false;
        private WarRoomConfig warRoomConfig;
        private string warRoomRootDir;
        private string ctoName = "泥蛇";
        private int missionCapital = 10000;
        private string currentMissionTitle = "构建与验证多AI协作任务";
        private int pendingToHumanCount = 0;
        private FileSystemWatcher missionWatcher;
        private TabControl tabWarRoom;
        private DataGridView gridChecklist;
        private DataGridView gridMeetings;
        private Label lblWarRoomHeader;
        private Label lblMissionSubtitle;
        private Label lblChecklistSummary;
        private Label lblMeetingsSummary;
        private List<ChecklistItem> currentChecklistItems = new List<ChecklistItem>();
        private List<MeetingRowItem> currentMeetingItems = new List<MeetingRowItem>();

        // P/Invoke Win32 APIs
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const int SW_RESTORE = 9;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_V = 0x56;
        private const byte VK_RETURN = 0x0D;
        private const byte VK_MENU = 0x12; // Alt key
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public const string NOTICE_VERSION = "20260911-2";
        private const string DEFAULT_NOTICE = "请检查 `CodeAi/会议` 中属于你的未闭环事项；如有，请自主执行并提交回执；如无，回复 `NO_PENDING_TASK`。";

        // UI Controls
        public class TargetNodeConfig
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string[] Aliases { get; set; }
            public string App { get; set; }
            public string[] ProcessNames { get; set; }
            public string[] WindowKeywords { get; set; }
            public string UiaStrategy { get; set; }
            public string ColorHex { get; set; }
            public bool DefaultSelected { get; set; }
        }

        private List<TargetNodeConfig> configuredNodes = new List<TargetNodeConfig>();
        private Dictionary<string, CheckBox> partnerCheckBoxes = new Dictionary<string, CheckBox>();
        private CheckBox chkPinTop;
        private TextBox txtNotice;
        private Button btnBroadcast;
        private Button btnReset;
        private Button btnToggleWatch;
        private Label lblWatchStatus;
        private Label lblStatus;
        private DataGridView gridDeliveries;
        private static readonly Color[] RainbowColors = new Color[]
        {
            Color.FromArgb(243, 139, 168), // 赤 Pastel Red (#f38ba8)
            Color.FromArgb(250, 179, 135), // 橙 Pastel Orange (#fab387)
            Color.FromArgb(249, 226, 175), // 黄 Pastel Yellow (#f9e2af)
            Color.FromArgb(166, 227, 161), // 绿 Pastel Green (#a6e3a1)
            Color.FromArgb(148, 226, 213), // 青 Pastel Cyan/Teal (#94e2d5)
            Color.FromArgb(137, 180, 250), // 蓝 Pastel Blue (#89b4fa)
            Color.FromArgb(203, 166, 247)  // 紫 Pastel Lavender/Purple (#cba6f7)
        };
        private int rainbowColorIndex = 0;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        // Auto-Watch Sentinel Core
        private FileSystemWatcher meetingWatcher;
        private bool isWatchingActive = true;
        private string meetingDir;
        private string runtimeStateDir;
        private string stateFilePath;
        private string taskDeliveriesJsonlPath;
        private HashSet<string> processedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> oldTwoPartKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> inFlightKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object stateLock = new object();

        // P0: TO_HUMAN Notification & Sound State
        private HashSet<string> notifiedHumanKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool soundEnabled = true;
        private string soundFile = null;
        private static byte[] cachedKnockWav = null;

        private class PendingRetryItem
        {
            public string NodeName;
            public string FilePath;
            public string FileName;
            public string Title;
            public string Sha256;
            public DateTime NextAttemptUtc;
            public int AttemptCount;
        }
        private List<PendingRetryItem> pendingRetries = new List<PendingRetryItem>();
        private readonly HashSet<string> pendingRetryKeys = new HashSet<string>();
        private bool isAuditDegraded = false;
        private System.Windows.Forms.Timer retryTimer;

        // 5 Delivery Stages
        public enum DeliveryStage
        {
            FILE_DISCOVERED,
            TARGET_FOUND,
            INJECTION_ATTEMPTED,
            SEND_ATTEMPTED = INJECTION_ATTEMPTED, // Backward-compat alias
            RECIPIENT_ACK,
            TERMINAL_RECEIPT
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                int darkMode = 1;
                DwmSetWindowAttribute(this.Handle, 20, ref darkMode, sizeof(int));
                DwmSetWindowAttribute(this.Handle, 19, ref darkMode, sizeof(int));
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                if (trayIcon != null)
                {
                    trayIcon.ShowBalloonTip(1500, "CodeAi 调度助手", "已最小化至系统托盘，后台持续哨兵监听中...", ToolTipIcon.Info);
                }
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        public DispatcherForm() : this(new InstanceOptions())
        {
        }

        public DispatcherForm(InstanceOptions options)
        {
            this.instanceOpts = options != null ? options : new InstanceOptions();
            if (this.instanceOpts.IsTestInstance)
            {
                // 测试实例默认使用独立的测试 War Room 路径
                if (string.IsNullOrEmpty(this.instanceOpts.WarRoomRoot))
                {
                    string pRoot = AppDomain.CurrentDomain.BaseDirectory;
                    DirectoryInfo di = new DirectoryInfo(pRoot);
                    while (di != null && !Directory.Exists(Path.Combine(di.FullName, "CodeAi"))) di = di.Parent;
                    string pr = di != null ? di.FullName : pRoot;
                    this.instanceOpts.WarRoomRoot = Path.Combine(pr, "AI-War-Room-Test");
                }
            }

            InitializeComponent();
            SetupTray();
            
            this.Shown += (s, e) =>
            {
                CheckAndPromptOnboarding();
                InitAutoWatcher();
                InitRetryTimer();
                CatchUpScan();
                StartHttpServer();
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int formHeight = Math.Min(860, Math.Max(660, screen.Height - 60));
            int formWidth = 540;
            this.ClientSize = new Size(formWidth, formHeight);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(screen.Right - formWidth - 20, screen.Bottom - formHeight - 20);
            this.MinimumSize = new Size(460, 560);
            if (instanceOpts != null && instanceOpts.IsTestInstance)
            {
                this.Text = string.Format("🧪 [TEST:{0}] CodeAi War Room (测试沙箱) [v{1}]", instanceOpts.Port, NOTICE_VERSION);
            }
            else
            {
                this.Text = string.Format("⚡ CodeAi War Room 协同中枢 [PROD:{0}] [v{1}]", instanceOpts != null ? instanceOpts.Port : 8787, NOTICE_VERSION);
            }
            
            this.BackColor = Color.FromArgb(24, 24, 37);
            this.TopMost = true;
            this.ShowInTaskbar = true;

            // 1. Top War Room Status Bar (Banner)
            Panel pnlWarRoomHeader = new Panel();
            pnlWarRoomHeader.Location = new Point(12, 10);
            pnlWarRoomHeader.Size = new Size(this.ClientSize.Width - 24, 60);
            pnlWarRoomHeader.BackColor = Color.FromArgb(30, 30, 46);
            pnlWarRoomHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblWarRoomHeader = new Label();
            lblWarRoomHeader.Text = string.Format("🏢 AI War Room | CTO: {0} | 资本: {1} SC", ctoName, missionCapital);
            lblWarRoomHeader.Font = new Font("Segoe UI", 9.8f, FontStyle.Bold);
            lblWarRoomHeader.ForeColor = Color.FromArgb(137, 180, 250);
            lblWarRoomHeader.Location = new Point(10, 8);
            lblWarRoomHeader.AutoSize = true;
            pnlWarRoomHeader.Controls.Add(lblWarRoomHeader);

            lblMissionSubtitle = new Label();
            lblMissionSubtitle.Text = string.Format("🎯 Mission: {0}", currentMissionTitle);
            lblMissionSubtitle.Font = new Font("Microsoft YaHei UI", 8.5f);
            lblMissionSubtitle.ForeColor = Color.FromArgb(249, 226, 175);
            lblMissionSubtitle.Location = new Point(12, 34);
            lblMissionSubtitle.Size = new Size(pnlWarRoomHeader.Width - 110, 20);
            lblMissionSubtitle.AutoEllipsis = true;
            pnlWarRoomHeader.Controls.Add(lblMissionSubtitle);

            Button btnWarRoomSetup = new Button();
            btnWarRoomSetup.Text = "⚙️ 开箱设置";
            btnWarRoomSetup.Location = new Point(pnlWarRoomHeader.Width - 94, 8);
            btnWarRoomSetup.Size = new Size(86, 26);
            btnWarRoomSetup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnWarRoomSetup.FlatStyle = FlatStyle.Flat;
            btnWarRoomSetup.BackColor = Color.FromArgb(49, 50, 68);
            btnWarRoomSetup.FlatAppearance.BorderSize = 0;
            btnWarRoomSetup.ForeColor = Color.FromArgb(205, 214, 244);
            btnWarRoomSetup.Font = new Font("Segoe UI", 7.8f);
            btnWarRoomSetup.Cursor = Cursors.Hand;
            btnWarRoomSetup.Click += (s, e) =>
            {
                using (WarRoomOnboardingForm ob = new WarRoomOnboardingForm(ctoName, warRoomRootDir))
                {
                    if (ob.ShowDialog(this) == DialogResult.OK)
                    {
                        ctoName = ob.SelectedCtoName;
                        warRoomRootDir = ob.SelectedWarRoomDir;
                        EnsureWarRoomStructure(warRoomRootDir, ctoName);
                        ReloadMissionSummaryFromDisk();
                        ReloadChecklistFromDisk();
                        ReloadMeetingsFromDisk();
                        UpdateWarRoomHeaderUI();
                        UpdateTrayStatus();
                    }
                }
            };
            pnlWarRoomHeader.Controls.Add(btnWarRoomSetup);
            this.Controls.Add(pnlWarRoomHeader);

            // 2. TabControl Host
            tabWarRoom = new TabControl();
            tabWarRoom.Location = new Point(12, pnlWarRoomHeader.Bottom + 8);
            tabWarRoom.Size = new Size(this.ClientSize.Width - 24, this.ClientSize.Height - pnlWarRoomHeader.Bottom - 16);
            tabWarRoom.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabWarRoom.Font = new Font("Segoe UI", 9f);

            // ================================================================
            // Tab 1: 📋 CTO Checklist
            // ================================================================
            TabPage pageChecklist = new TabPage("📋 CTO Checklist");
            pageChecklist.BackColor = Color.FromArgb(24, 24, 37);

            Panel pnlCheckTools = new Panel();
            pnlCheckTools.Dock = DockStyle.Top;
            pnlCheckTools.Height = 32;
            pnlCheckTools.BackColor = Color.FromArgb(30, 30, 46);

            lblChecklistSummary = new Label();
            lblChecklistSummary.Text = "任务统计: 载入中...";
            lblChecklistSummary.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblChecklistSummary.ForeColor = Color.FromArgb(166, 173, 200);
            lblChecklistSummary.Location = new Point(8, 8);
            lblChecklistSummary.AutoSize = true;
            pnlCheckTools.Controls.Add(lblChecklistSummary);

            Button btnCheckSelectAll = new Button();
            btnCheckSelectAll.Text = "全选";
            btnCheckSelectAll.Size = new Size(46, 22);
            btnCheckSelectAll.Location = new Point(pnlCheckTools.Width - 312, 5);
            btnCheckSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCheckSelectAll.FlatStyle = FlatStyle.Flat;
            btnCheckSelectAll.BackColor = Color.FromArgb(49, 50, 68);
            btnCheckSelectAll.FlatAppearance.BorderSize = 0;
            btnCheckSelectAll.ForeColor = Color.FromArgb(205, 214, 244);
            btnCheckSelectAll.Font = new Font("Segoe UI", 7.5f);
            btnCheckSelectAll.Cursor = Cursors.Hand;
            btnCheckSelectAll.Click += (s, e) => { SelectAllChecklistRows(); };
            pnlCheckTools.Controls.Add(btnCheckSelectAll);

            Button btnCheckDelete = new Button();
            btnCheckDelete.Text = "删除";
            btnCheckDelete.Size = new Size(46, 22);
            btnCheckDelete.Location = new Point(pnlCheckTools.Width - 262, 5);
            btnCheckDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCheckDelete.FlatStyle = FlatStyle.Flat;
            btnCheckDelete.BackColor = Color.FromArgb(69, 40, 55);
            btnCheckDelete.FlatAppearance.BorderSize = 0;
            btnCheckDelete.ForeColor = Color.FromArgb(243, 139, 168);
            btnCheckDelete.Font = new Font("Segoe UI", 7.5f);
            btnCheckDelete.Cursor = Cursors.Hand;
            btnCheckDelete.Click += (s, e) => { DeleteSelectedChecklistRows(); };
            pnlCheckTools.Controls.Add(btnCheckDelete);

            Button btnOpenChecklistFile = new Button();
            btnOpenChecklistFile.Text = "📂 打开事实源";
            btnOpenChecklistFile.Size = new Size(130, 22);
            btnOpenChecklistFile.Location = new Point(pnlCheckTools.Width - 212, 5);
            btnOpenChecklistFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenChecklistFile.FlatStyle = FlatStyle.Flat;
            btnOpenChecklistFile.BackColor = Color.FromArgb(49, 50, 68);
            btnOpenChecklistFile.FlatAppearance.BorderSize = 0;
            btnOpenChecklistFile.ForeColor = Color.FromArgb(205, 214, 244);
            btnOpenChecklistFile.Font = new Font("Segoe UI", 7.8f);
            btnOpenChecklistFile.Cursor = Cursors.Hand;
            btnOpenChecklistFile.Click += (s, e) => { OpenChecklistFile(); };
            pnlCheckTools.Controls.Add(btnOpenChecklistFile);

            Button btnRefreshChecklist = new Button();
            btnRefreshChecklist.Text = "🔄 刷新";
            btnRefreshChecklist.Size = new Size(58, 22);
            btnRefreshChecklist.Location = new Point(pnlCheckTools.Width - 78, 5);
            btnRefreshChecklist.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshChecklist.FlatStyle = FlatStyle.Flat;
            btnRefreshChecklist.BackColor = Color.FromArgb(49, 50, 68);
            btnRefreshChecklist.FlatAppearance.BorderSize = 0;
            btnRefreshChecklist.ForeColor = Color.FromArgb(205, 214, 244);
            btnRefreshChecklist.Font = new Font("Segoe UI", 7.8f);
            btnRefreshChecklist.Cursor = Cursors.Hand;
            btnRefreshChecklist.Click += (s, e) => { ReloadChecklistFromDisk(); };
            pnlCheckTools.Controls.Add(btnRefreshChecklist);

            pageChecklist.Controls.Add(pnlCheckTools);

            gridChecklist = new DataGridView();
            gridChecklist.Dock = DockStyle.Fill;
            gridChecklist.BackgroundColor = Color.FromArgb(17, 17, 27);
            gridChecklist.ForeColor = Color.FromArgb(205, 214, 244);
            gridChecklist.GridColor = Color.FromArgb(49, 50, 68);
            gridChecklist.BorderStyle = BorderStyle.None;
            gridChecklist.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridChecklist.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridChecklist.EnableHeadersVisualStyles = false;
            gridChecklist.RowHeadersVisible = false;
            gridChecklist.AllowUserToAddRows = false;
            gridChecklist.AllowUserToDeleteRows = false;
            gridChecklist.AllowUserToResizeRows = false;
            gridChecklist.ReadOnly = true;
            gridChecklist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridChecklist.MultiSelect = true;
            gridChecklist.Font = new Font("Segoe UI", 8.5f);
            gridChecklist.RowTemplate.Height = 26;

            DataGridViewCellStyle clHeaderStyle = new DataGridViewCellStyle();
            clHeaderStyle.BackColor = Color.FromArgb(30, 30, 46);
            clHeaderStyle.ForeColor = Color.FromArgb(166, 173, 200);
            clHeaderStyle.Font = new Font("Segoe UI", 8.2f, FontStyle.Bold);
            gridChecklist.ColumnHeadersDefaultCellStyle = clHeaderStyle;
            gridChecklist.ColumnHeadersHeight = 26;

            gridChecklist.Columns.Add("colCheckState", "状态");
            gridChecklist.Columns["colCheckState"].Width = 90;
            gridChecklist.Columns.Add("colCheckNode", "责任节点");
            gridChecklist.Columns["colCheckNode"].Width = 85;
            gridChecklist.Columns.Add("colCheckContent", "任务内容 / 验收口径");
            gridChecklist.Columns["colCheckContent"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            gridChecklist.CellDoubleClick += (s, e) => { OpenChecklistFile(); };

            // 右键菜单: 全选, 打开事实源, 复制内容, 删除所选项
            ContextMenuStrip menuChecklist = new ContextMenuStrip();
            menuChecklist.BackColor = Color.FromArgb(30, 30, 46);
            menuChecklist.ForeColor = Color.FromArgb(205, 214, 244);
            menuChecklist.ShowImageMargin = false;

            ToolStripMenuItem miCheckAll = new ToolStripMenuItem("全选 (&A)");
            miCheckAll.ForeColor = Color.FromArgb(205, 214, 244);
            miCheckAll.Click += (s, e) => { SelectAllChecklistRows(); };

            ToolStripMenuItem miCheckOpen = new ToolStripMenuItem("打开事实源 CHECKLIST.md (&O)");
            miCheckOpen.ForeColor = Color.FromArgb(137, 180, 250);
            miCheckOpen.Click += (s, e) => { OpenChecklistFile(); };

            ToolStripMenuItem miCheckDelete = new ToolStripMenuItem("删除所选条目 (&D)");
            miCheckDelete.ForeColor = Color.FromArgb(243, 139, 168);
            miCheckDelete.Click += (s, e) => { DeleteSelectedChecklistRows(); };

            menuChecklist.Items.Add(miCheckAll);
            menuChecklist.Items.Add(miCheckOpen);
            menuChecklist.Items.Add(new ToolStripSeparator());
            menuChecklist.Items.Add(miCheckDelete);
            gridChecklist.ContextMenuStrip = menuChecklist;

            gridChecklist.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    SelectAllChecklistRows();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelectedChecklistRows();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    OpenChecklistFile();
                    e.Handled = true;
                }
            };

            Label lblChecklistTip = new Label();
            lblChecklistTip.Text = "💡 支持鼠标拖选/多选 | 右键菜单管理 | 双击条目直接打开 CHECKLIST.md 事实源";
            lblChecklistTip.Dock = DockStyle.Bottom;
            lblChecklistTip.Height = 22;
            lblChecklistTip.Font = new Font("Microsoft YaHei UI", 7.8f);
            lblChecklistTip.ForeColor = Color.FromArgb(108, 112, 134);
            lblChecklistTip.BackColor = Color.FromArgb(24, 24, 37);
            lblChecklistTip.TextAlign = ContentAlignment.MiddleLeft;

            pageChecklist.Controls.Add(gridChecklist);
            pageChecklist.Controls.Add(lblChecklistTip);
            tabWarRoom.TabPages.Add(pageChecklist);

            // ================================================================
            // Tab 2: 💬 Meetings 会议
            // ================================================================
            TabPage pageMeetings = new TabPage("💬 Meetings 会议");
            pageMeetings.BackColor = Color.FromArgb(24, 24, 37);

            Panel pnlMeetTools = new Panel();
            pnlMeetTools.Dock = DockStyle.Top;
            pnlMeetTools.Height = 32;
            pnlMeetTools.BackColor = Color.FromArgb(30, 30, 46);

            lblMeetingsSummary = new Label();
            lblMeetingsSummary.Text = "会议动态: 载入中...";
            lblMeetingsSummary.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblMeetingsSummary.ForeColor = Color.FromArgb(166, 173, 200);
            lblMeetingsSummary.Location = new Point(8, 8);
            lblMeetingsSummary.AutoSize = true;
            pnlMeetTools.Controls.Add(lblMeetingsSummary);

            Button btnMeetSelectAll = new Button();
            btnMeetSelectAll.Text = "全选";
            btnMeetSelectAll.Size = new Size(46, 22);
            btnMeetSelectAll.Location = new Point(pnlMeetTools.Width - 322, 5);
            btnMeetSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMeetSelectAll.FlatStyle = FlatStyle.Flat;
            btnMeetSelectAll.BackColor = Color.FromArgb(49, 50, 68);
            btnMeetSelectAll.FlatAppearance.BorderSize = 0;
            btnMeetSelectAll.ForeColor = Color.FromArgb(205, 214, 244);
            btnMeetSelectAll.Font = new Font("Segoe UI", 7.5f);
            btnMeetSelectAll.Cursor = Cursors.Hand;
            btnMeetSelectAll.Click += (s, e) => { SelectAllMeetingRows(); };
            pnlMeetTools.Controls.Add(btnMeetSelectAll);

            Button btnMeetDelete = new Button();
            btnMeetDelete.Text = "删除";
            btnMeetDelete.Size = new Size(46, 22);
            btnMeetDelete.Location = new Point(pnlMeetTools.Width - 272, 5);
            btnMeetDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMeetDelete.FlatStyle = FlatStyle.Flat;
            btnMeetDelete.BackColor = Color.FromArgb(69, 40, 55);
            btnMeetDelete.FlatAppearance.BorderSize = 0;
            btnMeetDelete.ForeColor = Color.FromArgb(243, 139, 168);
            btnMeetDelete.Font = new Font("Segoe UI", 7.5f);
            btnMeetDelete.Cursor = Cursors.Hand;
            btnMeetDelete.Click += (s, e) => { DeleteSelectedMeetingRows(); };
            pnlMeetTools.Controls.Add(btnMeetDelete);

            Button btnOpenMeetDir = new Button();
            btnOpenMeetDir.Text = "📂 打开会议室";
            btnOpenMeetDir.Size = new Size(110, 22);
            btnOpenMeetDir.Location = new Point(pnlMeetTools.Width - 222, 5);
            btnOpenMeetDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenMeetDir.FlatStyle = FlatStyle.Flat;
            btnOpenMeetDir.BackColor = Color.FromArgb(49, 50, 68);
            btnOpenMeetDir.FlatAppearance.BorderSize = 0;
            btnOpenMeetDir.ForeColor = Color.FromArgb(205, 214, 244);
            btnOpenMeetDir.Font = new Font("Segoe UI", 7.8f);
            btnOpenMeetDir.Cursor = Cursors.Hand;
            btnOpenMeetDir.Click += (s, e) => { OpenMeetingsDirectory(); };
            pnlMeetTools.Controls.Add(btnOpenMeetDir);

            Button btnRefreshMeetings = new Button();
            btnRefreshMeetings.Text = "🔄 刷新";
            btnRefreshMeetings.Size = new Size(58, 22);
            btnRefreshMeetings.Location = new Point(pnlMeetTools.Width - 108, 5);
            btnRefreshMeetings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshMeetings.FlatStyle = FlatStyle.Flat;
            btnRefreshMeetings.BackColor = Color.FromArgb(49, 50, 68);
            btnRefreshMeetings.FlatAppearance.BorderSize = 0;
            btnRefreshMeetings.ForeColor = Color.FromArgb(205, 214, 244);
            btnRefreshMeetings.Font = new Font("Segoe UI", 7.8f);
            btnRefreshMeetings.Cursor = Cursors.Hand;
            btnRefreshMeetings.Click += (s, e) => { ReloadMeetingsFromDisk(); };
            pnlMeetTools.Controls.Add(btnRefreshMeetings);

            pageMeetings.Controls.Add(pnlMeetTools);

            gridMeetings = new DataGridView();
            gridMeetings.Dock = DockStyle.Fill;
            gridMeetings.BackgroundColor = Color.FromArgb(17, 17, 27);
            gridMeetings.ForeColor = Color.FromArgb(205, 214, 244);
            gridMeetings.GridColor = Color.FromArgb(49, 50, 68);
            gridMeetings.BorderStyle = BorderStyle.None;
            gridMeetings.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridMeetings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridMeetings.EnableHeadersVisualStyles = false;
            gridMeetings.RowHeadersVisible = false;
            gridMeetings.AllowUserToAddRows = false;
            gridMeetings.AllowUserToDeleteRows = false;
            gridMeetings.AllowUserToResizeRows = false;
            gridMeetings.ReadOnly = true;
            gridMeetings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridMeetings.MultiSelect = true;
            gridMeetings.Font = new Font("Segoe UI", 8.2f);
            gridMeetings.RowTemplate.Height = 24;

            DataGridViewCellStyle mHeaderStyle = new DataGridViewCellStyle();
            mHeaderStyle.BackColor = Color.FromArgb(30, 30, 46);
            mHeaderStyle.ForeColor = Color.FromArgb(166, 173, 200);
            mHeaderStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            gridMeetings.ColumnHeadersDefaultCellStyle = mHeaderStyle;
            gridMeetings.ColumnHeadersHeight = 26;

            gridMeetings.Columns.Add("colMeetTime", "时间");
            gridMeetings.Columns["colMeetTime"].Width = 60;
            gridMeetings.Columns.Add("colMeetType", "阶段/动作");
            gridMeetings.Columns["colMeetType"].Width = 90;
            gridMeetings.Columns.Add("colMeetSender", "发件人");
            gridMeetings.Columns["colMeetSender"].Width = 72;
            gridMeetings.Columns.Add("colMeetRecipient", "收件人");
            gridMeetings.Columns["colMeetRecipient"].Width = 72;
            gridMeetings.Columns.Add("colMeetTitle", "会议事项 / 信件原件");
            gridMeetings.Columns["colMeetTitle"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            gridMeetings.CellDoubleClick += (s, e) => { OpenSelectedMeetingFile(); };

            // 右键菜单: 全选, 打开信件原件, 打开所在目录, 删除所选信件
            ContextMenuStrip menuMeetings = new ContextMenuStrip();
            menuMeetings.BackColor = Color.FromArgb(30, 30, 46);
            menuMeetings.ForeColor = Color.FromArgb(205, 214, 244);
            menuMeetings.ShowImageMargin = false;

            ToolStripMenuItem miMeetAll = new ToolStripMenuItem("全选 (&A)");
            miMeetAll.ForeColor = Color.FromArgb(205, 214, 244);
            miMeetAll.Click += (s, e) => { SelectAllMeetingRows(); };

            ToolStripMenuItem miMeetOpen = new ToolStripMenuItem("打开会议信件原件 (&O)");
            miMeetOpen.ForeColor = Color.FromArgb(137, 180, 250);
            miMeetOpen.Click += (s, e) => { OpenSelectedMeetingFile(); };

            ToolStripMenuItem miMeetDir = new ToolStripMenuItem("打开会议室目录 (&F)");
            miMeetDir.ForeColor = Color.FromArgb(205, 214, 244);
            miMeetDir.Click += (s, e) => { OpenMeetingsDirectory(); };

            ToolStripMenuItem miMeetDelete = new ToolStripMenuItem("删除所选信件原件 (&D)");
            miMeetDelete.ForeColor = Color.FromArgb(243, 139, 168);
            miMeetDelete.Click += (s, e) => { DeleteSelectedMeetingRows(); };

            menuMeetings.Items.Add(miMeetAll);
            menuMeetings.Items.Add(miMeetOpen);
            menuMeetings.Items.Add(miMeetDir);
            menuMeetings.Items.Add(new ToolStripSeparator());
            menuMeetings.Items.Add(miMeetDelete);
            gridMeetings.ContextMenuStrip = menuMeetings;

            gridMeetings.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    SelectAllMeetingRows();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelectedMeetingRows();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    OpenSelectedMeetingFile();
                    e.Handled = true;
                }
            };

            Label lblMeetTip = new Label();
            lblMeetTip.Text = "💡 支持鼠标拖选/多选 | 右键菜单操作 | 双击会议条目直接打开会议原始信件，文件才是事实源";
            lblMeetTip.Dock = DockStyle.Bottom;
            lblMeetTip.Height = 22;
            lblMeetTip.Font = new Font("Microsoft YaHei UI", 7.8f);
            lblMeetTip.ForeColor = Color.FromArgb(108, 112, 134);
            lblMeetTip.BackColor = Color.FromArgb(24, 24, 37);
            lblMeetTip.TextAlign = ContentAlignment.MiddleLeft;

            pageMeetings.Controls.Add(gridMeetings);
            pageMeetings.Controls.Add(lblMeetTip);
            tabWarRoom.TabPages.Add(pageMeetings);

            // ================================================================
            // Tab 3: 📢 哨兵控制台 (Sentinel Console)
            // ================================================================
            TabPage pageConsole = new TabPage("📢 哨兵控制台");
            pageConsole.BackColor = Color.FromArgb(24, 24, 37);

            // Sentinel Auto-Watch Toggle Bar
            Panel pnlSentinel = new Panel();
            pnlSentinel.Location = new Point(8, 8);
            pnlSentinel.Size = new Size(tabWarRoom.Width - 24, 32);
            pnlSentinel.BackColor = Color.FromArgb(30, 30, 46);
            pnlSentinel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblWatchStatus = new Label();
            lblWatchStatus.Text = "哨兵: 🟢 监听会议目录中";
            lblWatchStatus.Font = new Font("Segoe UI", 8.2f, FontStyle.Bold);
            lblWatchStatus.ForeColor = Color.FromArgb(166, 227, 161);
            lblWatchStatus.Location = new Point(8, 8);
            lblWatchStatus.AutoSize = true;
            pnlSentinel.Controls.Add(lblWatchStatus);

            chkPinTop = new CheckBox();
            chkPinTop.Text = "置顶";
            chkPinTop.Size = new Size(54, 24);
            chkPinTop.Location = new Point(pnlSentinel.Width - 138, 4);
            chkPinTop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkPinTop.Checked = this.TopMost;
            chkPinTop.ForeColor = Color.FromArgb(205, 214, 244);
            chkPinTop.Font = new Font("Segoe UI", 7.8f);
            chkPinTop.CheckedChanged += (s, e) =>
            {
                this.TopMost = chkPinTop.Checked;
                lblStatus.Text = this.TopMost ? "窗口已置顶" : "窗口已取消置顶";
            };
            pnlSentinel.Controls.Add(chkPinTop);

            btnToggleWatch = new Button();
            btnToggleWatch.Text = "暂停";
            btnToggleWatch.Size = new Size(54, 24);
            btnToggleWatch.Location = new Point(pnlSentinel.Width - 68, 4);
            btnToggleWatch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleWatch.FlatStyle = FlatStyle.Flat;
            btnToggleWatch.BackColor = Color.FromArgb(49, 50, 68);
            btnToggleWatch.ForeColor = Color.FromArgb(205, 214, 244);
            btnToggleWatch.Font = new Font("Segoe UI", 7.8f);
            btnToggleWatch.Click += (s, e) => { ToggleWatchMode(); };
            pnlSentinel.Controls.Add(btnToggleWatch);

            pageConsole.Controls.Add(pnlSentinel);

            // Partner Checkboxes Group (Dynamic Configuration-Driven)
            int listHeight = Math.Max(70, 26 + configuredNodes.Count * 22);
            Panel pnlList = new Panel();
            pnlList.Location = new Point(8, 46);
            pnlList.Size = new Size(tabWarRoom.Width - 24, listHeight);
            pnlList.BackColor = Color.FromArgb(30, 30, 46);
            pnlList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblGroupTitle = new Label();
            lblGroupTitle.Text = "人工后备目标 (Manual Fallback):";
            lblGroupTitle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblGroupTitle.ForeColor = Color.FromArgb(147, 153, 178);
            lblGroupTitle.Location = new Point(8, 4);
            lblGroupTitle.AutoSize = true;
            pnlList.Controls.Add(lblGroupTitle);

            partnerCheckBoxes = new Dictionary<string, CheckBox>();
            int curY = 22;
            foreach (var node in configuredNodes)
            {
                CheckBox chk = new CheckBox();
                chk.Text = string.Format(" {0} ({1})", node.Name, node.App);
                chk.Location = new Point(12, curY);
                chk.AutoSize = true;
                chk.Checked = false;
                chk.Font = new Font("Segoe UI", 8.5f);
                Color nodeColor;
                try { nodeColor = ColorTranslator.FromHtml(node.ColorHex); }
                catch { nodeColor = Color.FromArgb(205, 214, 244); }
                chk.ForeColor = nodeColor;
                chk.Tag = node;
                pnlList.Controls.Add(chk);
                partnerCheckBoxes[node.Name] = chk;
                curY += 22;
            }

            Button btnRefreshPartners = new Button();
            btnRefreshPartners.Text = "↻ 刷新检测";
            btnRefreshPartners.Location = new Point(pnlList.Width - 88, 3);
            btnRefreshPartners.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshPartners.Size = new Size(80, 20);
            btnRefreshPartners.FlatStyle = FlatStyle.Flat;
            btnRefreshPartners.BackColor = Color.FromArgb(49, 50, 68);
            btnRefreshPartners.FlatAppearance.BorderSize = 0;
            btnRefreshPartners.ForeColor = Color.FromArgb(166, 173, 200);
            btnRefreshPartners.Font = new Font("Segoe UI", 7.5f);
            btnRefreshPartners.Cursor = Cursors.Hand;
            btnRefreshPartners.Click += (s, e) =>
            {
                AutoDetectPartnerCheckboxes();
                List<string> active = new List<string>();
                foreach (var kvp in partnerCheckBoxes)
                {
                    if (kvp.Value.Checked) active.Add(kvp.Key);
                }
                lblStatus.Text = string.Format("已刷新在位检测: 在线 [{0}]", string.Join(", ", active.ToArray()));
            };
            pnlList.Controls.Add(btnRefreshPartners);

            AutoDetectPartnerCheckboxes();
            pageConsole.Controls.Add(pnlList);

            int topY = pnlList.Bottom + 4;

            // Message Label
            Label lblMsg = new Label();
            lblMsg.Text = "通知内容 (Message):";
            lblMsg.Font = new Font("Segoe UI", 8.2f);
            lblMsg.ForeColor = Color.FromArgb(147, 153, 178);
            lblMsg.Location = new Point(8, topY);
            lblMsg.AutoSize = true;
            pageConsole.Controls.Add(lblMsg);

            // Multiline Message TextBox
            txtNotice = new TextBox();
            txtNotice.Location = new Point(8, topY + 18);
            txtNotice.Size = new Size(tabWarRoom.Width - 24, 76);
            txtNotice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNotice.Text = DEFAULT_NOTICE;
            txtNotice.Multiline = true;
            txtNotice.ScrollBars = ScrollBars.Vertical;
            txtNotice.BackColor = Color.FromArgb(49, 50, 68);
            txtNotice.ForeColor = Color.FromArgb(205, 214, 244);
            txtNotice.BorderStyle = BorderStyle.FixedSingle;
            txtNotice.Font = new Font("Segoe UI", 8.5f);
            pageConsole.Controls.Add(txtNotice);

            // Action Buttons Row: [手动通知] + [默认]
            int resetWidth = 98;
            int btnY = txtNotice.Bottom + 6;
            btnBroadcast = new Button();
            btnBroadcast.Text = "📢 手动通知";
            btnBroadcast.Location = new Point(8, btnY);
            btnBroadcast.Size = new Size(tabWarRoom.Width - 24 - resetWidth - 8, 30);
            btnBroadcast.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnBroadcast.FlatStyle = FlatStyle.Flat;
            btnBroadcast.BackColor = Color.FromArgb(124, 58, 237);
            btnBroadcast.FlatAppearance.BorderSize = 0;
            btnBroadcast.ForeColor = Color.White;
            btnBroadcast.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
            btnBroadcast.Cursor = Cursors.Hand;
            btnBroadcast.Click += async (s, e) => { await ExecuteManualBroadcast(); };
            pageConsole.Controls.Add(btnBroadcast);

            btnReset = new Button();
            btnReset.Text = "↺ 默认";
            btnReset.Location = new Point(tabWarRoom.Width - 16 - resetWidth, btnY);
            btnReset.Size = new Size(resetWidth, 30);
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.BackColor = Color.FromArgb(49, 50, 68);
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.ForeColor = Color.FromArgb(205, 214, 244);
            btnReset.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnReset.Cursor = Cursors.Hand;
            btnReset.Click += (s, e) =>
            {
                txtNotice.Text = DEFAULT_NOTICE;
                lblStatus.Text = string.Format("已重置为默认门铃通告 [v{0}]", NOTICE_VERSION);
            };
            pageConsole.Controls.Add(btnReset);

            // Status label
            lblStatus = new Label();
            lblStatus.Text = "就绪 | 会议目录自动监控中";
            lblStatus.Font = new Font("Segoe UI", 8f);
            lblStatus.ForeColor = Color.FromArgb(166, 173, 200);
            lblStatus.Location = new Point(8, btnBroadcast.Bottom + 4);
            lblStatus.Size = new Size(tabWarRoom.Width - 24, 20);
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pageConsole.Controls.Add(lblStatus);

            // Recent Deliveries and ACK Status Panel (Colorful Grid)
            int pnlDelivY = lblStatus.Bottom + 4;
            Panel pnlDeliveries = new Panel();
            pnlDeliveries.Location = new Point(8, pnlDelivY);
            pnlDeliveries.Size = new Size(tabWarRoom.Width - 24, tabWarRoom.Height - pnlDelivY - 38);
            pnlDeliveries.BackColor = Color.FromArgb(30, 30, 46);
            pnlDeliveries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label lblDelivTitle = new Label();
            lblDelivTitle.Text = "投递与ACK监控 (Recent Deliveries):";
            lblDelivTitle.Font = new Font("Segoe UI", 7.8f, FontStyle.Bold);
            lblDelivTitle.ForeColor = Color.FromArgb(147, 153, 178);
            lblDelivTitle.Location = new Point(8, 5);
            lblDelivTitle.AutoSize = true;
            pnlDeliveries.Controls.Add(lblDelivTitle);

            Button btnDelivSelectAll = new Button();
            btnDelivSelectAll.Text = "全选";
            btnDelivSelectAll.Location = new Point(pnlDeliveries.Width - 108, 2);
            btnDelivSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelivSelectAll.Size = new Size(46, 20);
            btnDelivSelectAll.FlatStyle = FlatStyle.Flat;
            btnDelivSelectAll.BackColor = Color.FromArgb(49, 50, 68);
            btnDelivSelectAll.FlatAppearance.BorderSize = 0;
            btnDelivSelectAll.ForeColor = Color.FromArgb(166, 173, 200);
            btnDelivSelectAll.Font = new Font("Segoe UI", 7.5f);
            btnDelivSelectAll.Cursor = Cursors.Hand;
            btnDelivSelectAll.Click += (s, e) => { SelectAllDeliveryRows(); };
            pnlDeliveries.Controls.Add(btnDelivSelectAll);

            Button btnDelivDelete = new Button();
            btnDelivDelete.Text = "删除";
            btnDelivDelete.Location = new Point(pnlDeliveries.Width - 56, 2);
            btnDelivDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelivDelete.Size = new Size(46, 20);
            btnDelivDelete.FlatStyle = FlatStyle.Flat;
            btnDelivDelete.BackColor = Color.FromArgb(69, 40, 55);
            btnDelivDelete.FlatAppearance.BorderSize = 0;
            btnDelivDelete.ForeColor = Color.FromArgb(243, 139, 168);
            btnDelivDelete.Font = new Font("Segoe UI", 7.5f);
            btnDelivDelete.Cursor = Cursors.Hand;
            btnDelivDelete.Click += (s, e) => { DeleteSelectedDeliveryRows(); };
            pnlDeliveries.Controls.Add(btnDelivDelete);

            gridDeliveries = new DataGridView();
            gridDeliveries.Location = new Point(8, 26);
            gridDeliveries.Size = new Size(pnlDeliveries.Width - 16, pnlDeliveries.Height - 32);
            gridDeliveries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridDeliveries.BackgroundColor = Color.FromArgb(17, 17, 27);
            gridDeliveries.ForeColor = Color.FromArgb(205, 214, 244);
            gridDeliveries.GridColor = Color.FromArgb(49, 50, 68);
            gridDeliveries.BorderStyle = BorderStyle.None;
            gridDeliveries.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridDeliveries.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridDeliveries.EnableHeadersVisualStyles = false;
            gridDeliveries.RowHeadersVisible = false;
            gridDeliveries.AllowUserToAddRows = false;
            gridDeliveries.AllowUserToDeleteRows = false;
            gridDeliveries.AllowUserToResizeRows = false;
            gridDeliveries.ReadOnly = true;
            gridDeliveries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDeliveries.MultiSelect = true;
            gridDeliveries.Font = new Font("Segoe UI", 8.2f);
            gridDeliveries.RowTemplate.Height = 22;

            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = Color.FromArgb(30, 30, 46);
            headerStyle.ForeColor = Color.FromArgb(166, 173, 200);
            headerStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            gridDeliveries.ColumnHeadersDefaultCellStyle = headerStyle;
            gridDeliveries.ColumnHeadersHeight = 24;

            gridDeliveries.Columns.Add("colTime", "时间");
            gridDeliveries.Columns["colTime"].Width = 65;
            gridDeliveries.Columns.Add("colSender", "发件战友");
            gridDeliveries.Columns["colSender"].Width = 72;
            gridDeliveries.Columns.Add("colStage", "阶段 (通知战友)");
            gridDeliveries.Columns["colStage"].Width = 145;
            gridDeliveries.Columns.Add("colFile", "事项 / 文件");
            gridDeliveries.Columns["colFile"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            ContextMenuStrip gridContextMenu = new ContextMenuStrip();
            gridContextMenu.BackColor = Color.FromArgb(30, 30, 46);
            gridContextMenu.ForeColor = Color.FromArgb(205, 214, 244);
            gridContextMenu.ShowImageMargin = false;

            ToolStripMenuItem menuSelectAll = new ToolStripMenuItem("全选 (&A)");
            menuSelectAll.ForeColor = Color.FromArgb(205, 214, 244);
            menuSelectAll.Click += (s, e) => { SelectAllDeliveryRows(); };

            ToolStripMenuItem menuDelete = new ToolStripMenuItem("删除所选流水 (&D)");
            menuDelete.ForeColor = Color.FromArgb(243, 139, 168);
            menuDelete.Click += (s, e) => { DeleteSelectedDeliveryRows(); };

            gridContextMenu.Items.Add(menuSelectAll);
            gridContextMenu.Items.Add(new ToolStripSeparator());
            gridContextMenu.Items.Add(menuDelete);
            gridDeliveries.ContextMenuStrip = gridContextMenu;

            gridDeliveries.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    SelectAllDeliveryRows();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelectedDeliveryRows();
                    e.Handled = true;
                }
            };

            pnlDeliveries.Controls.Add(gridDeliveries);
            pageConsole.Controls.Add(pnlDeliveries);

            tabWarRoom.TabPages.Add(pageConsole);
            this.Controls.Add(tabWarRoom);

            this.ResumeLayout(false);
        }

        private void ShowFloatingWindow()
        {
            if (this.IsDisposed) return;

            if (!this.Visible)
            {
                this.Visible = true;
                this.Show();
            }

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            ShowWindow(this.Handle, 9); // SW_RESTORE
            BringWindowToTop(this.Handle);
            SetForegroundWindow(this.Handle);
            SwitchToThisWindow(this.Handle, true);
            this.Activate();
            this.BringToFront();
        }

        private void SetupTray()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.BackColor = Color.FromArgb(30, 30, 46);
            trayMenu.ForeColor = Color.FromArgb(205, 214, 244);
            trayMenu.ShowImageMargin = true;

            ToolStripMenuItem itemShow = new ToolStripMenuItem("显示浮窗 (&S)");
            itemShow.ForeColor = Color.FromArgb(205, 214, 244);
            itemShow.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            itemShow.Click += (s, e) => { ShowFloatingWindow(); };

            ToolStripMenuItem itemTopMost = new ToolStripMenuItem("窗口置顶 (&T)");
            itemTopMost.CheckOnClick = true;
            itemTopMost.Checked = this.TopMost;
            itemTopMost.ForeColor = Color.FromArgb(205, 214, 244);
            itemTopMost.Click += (s, e) =>
            {
                this.TopMost = itemTopMost.Checked;
                if (chkPinTop != null && chkPinTop.Checked != this.TopMost)
                {
                    chkPinTop.Checked = this.TopMost;
                }
                lblStatus.Text = this.TopMost ? "窗口已置顶" : "窗口已取消置顶";
            };

            ToolStripMenuItem itemWatch = new ToolStripMenuItem("自动监听会议目录");
            itemWatch.CheckOnClick = true;
            itemWatch.Checked = isWatchingActive;
            itemWatch.ForeColor = Color.FromArgb(205, 214, 244);
            itemWatch.Click += (s, e) => { ToggleWatchMode(); };

            ToolStripMenuItem itemExit = new ToolStripMenuItem("退出 (&X)");
            itemExit.ForeColor = Color.FromArgb(243, 139, 168);
            itemExit.Click += (s, e) =>
            {
                if (trayIcon != null) trayIcon.Visible = false;
                Environment.Exit(0);
            };

            trayMenu.Items.Add(itemShow);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(itemTopMost);
            trayMenu.Items.Add(itemWatch);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(itemExit);

            trayMenu.Opening += (s, e) =>
            {
                itemTopMost.Checked = this.TopMost;
                itemWatch.Checked = isWatchingActive;
                itemWatch.Text = isWatchingActive ? "自动监听会议目录 [运行中]" : "自动监听会议目录 [已暂停]";
            };

            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                Color iconBg = (instanceOpts != null && instanceOpts.IsTestInstance) 
                    ? Color.FromArgb(250, 179, 135) // 测试沙箱亮橙色
                    : Color.FromArgb(124, 58, 237); // 正式实例紫色
                using (Brush b = new SolidBrush(iconBg))
                {
                    g.FillEllipse(b, 1, 1, 14, 14);
                }
                using (Pen p = new Pen(Color.White, 1.8f))
                {
                    Point[] pts = new Point[] {
                        new Point(9, 2),
                        new Point(5, 8),
                        new Point(9, 8),
                        new Point(7, 14)
                    };
                    g.DrawLines(p, pts);
                }
            }
            IntPtr hIcon = bmp.GetHicon();
            this.Icon = Icon.FromHandle(hIcon);

            trayIcon = new NotifyIcon();
            trayIcon.Icon = Icon.FromHandle(hIcon);
            trayIcon.Text = "CodeAi 调度助手 (哨兵)";
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += (s, e) => { ShowFloatingWindow(); };
            trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ShowTrayPreviewPopup();
                }
            };
        }

        
        // ====================================================================
        // War Room Lifecycle, Checklist & Meetings Handlers
        // ====================================================================

        public void CheckAndPromptOnboarding()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo d = new DirectoryInfo(baseDir);
                while (d != null && !Directory.Exists(Path.Combine(d.FullName, "CodeAi")) && !Directory.Exists(Path.Combine(d.FullName, "AI-War-Room")))
                {
                    d = d.Parent;
                }
                string projectRoot = d != null ? d.FullName : baseDir;

                string candidateWarRoom = (instanceOpts != null && !string.IsNullOrEmpty(instanceOpts.WarRoomRoot)) 
                    ? instanceOpts.WarRoomRoot 
                    : Path.Combine(projectRoot, (instanceOpts != null && instanceOpts.IsTestInstance) ? "AI-War-Room-Test" : "AI-War-Room");
                string configPath = Path.Combine(candidateWarRoom, "config", "war_room.json");
                string appLevelConfig = Path.Combine(baseDir, "war_room_config.json");

                bool needOnboarding = true;
                if (File.Exists(configPath))
                {
                    try
                    {
                        string json = File.ReadAllText(configPath, Encoding.UTF8);
                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        warRoomConfig = ser.Deserialize<WarRoomConfig>(json);
                        if (warRoomConfig != null && !string.IsNullOrEmpty(warRoomConfig.WarRoomRoot) && Directory.Exists(warRoomConfig.WarRoomRoot))
                        {
                            warRoomRootDir = warRoomConfig.WarRoomRoot;
                            ctoName = warRoomConfig.CtoName;
                            missionCapital = warRoomConfig.MissionCapital > 0 ? warRoomConfig.MissionCapital : 10000;
                            needOnboarding = false;
                        }
                    }
                    catch { }
                }
                else if (File.Exists(appLevelConfig))
                {
                    try
                    {
                        string json = File.ReadAllText(appLevelConfig, Encoding.UTF8);
                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        warRoomConfig = ser.Deserialize<WarRoomConfig>(json);
                        if (warRoomConfig != null && !string.IsNullOrEmpty(warRoomConfig.WarRoomRoot) && Directory.Exists(warRoomConfig.WarRoomRoot))
                        {
                            warRoomRootDir = warRoomConfig.WarRoomRoot;
                            ctoName = warRoomConfig.CtoName;
                            missionCapital = warRoomConfig.MissionCapital > 0 ? warRoomConfig.MissionCapital : 10000;
                            needOnboarding = false;
                        }
                    }
                    catch { }
                }

                if (needOnboarding)
                {
                    if (instanceOpts != null && instanceOpts.IsTestInstance)
                    {
                        // 测试实例免弹窗向导，自动静默就绪
                        warRoomRootDir = candidateWarRoom;
                        ctoName = "测试CTO";
                        missionCapital = 10000;
                        EnsureWarRoomStructure(warRoomRootDir, ctoName);
                    }
                    else
                    {
                        using (WarRoomOnboardingForm obForm = new WarRoomOnboardingForm(ctoName, candidateWarRoom))
                        {
                            if (obForm.ShowDialog(this) == DialogResult.OK)
                            {
                                ctoName = obForm.SelectedCtoName;
                                warRoomRootDir = obForm.SelectedWarRoomDir;
                                missionCapital = 10000;
                                EnsureWarRoomStructure(warRoomRootDir, ctoName);
                            }
                            else
                            {
                                warRoomRootDir = candidateWarRoom;
                                EnsureWarRoomStructure(warRoomRootDir, ctoName);
                            }
                        }
                    }
                }
                else
                {
                    EnsureWarRoomStructure(warRoomRootDir, ctoName);
                }

                InitMissionWatcher();
                ReloadMissionSummaryFromDisk();
                ReloadChecklistFromDisk();
                ReloadMeetingsFromDisk();
                UpdateWarRoomHeaderUI();
                UpdateTrayStatus();
            }
            catch (Exception ex)
            {
                LogAudit("ONBOARDING_ERROR", ex.Message);
            }
        }

        public static void EnsureWarRoomStructure(string rootDir, string cto)
        {
            try
            {
                if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);

                string dirCto = Path.Combine(rootDir, "CTO");
                string dirMembers = Path.Combine(rootDir, "Members");
                string dirMeetings = Path.Combine(rootDir, "Meetings");
                string dirMission = Path.Combine(rootDir, "Mission");
                string dirConfig = Path.Combine(rootDir, "config");

                if (!Directory.Exists(dirCto)) Directory.CreateDirectory(dirCto);
                if (!Directory.Exists(dirMembers)) Directory.CreateDirectory(dirMembers);
                if (!Directory.Exists(dirMeetings)) Directory.CreateDirectory(dirMeetings);
                if (!Directory.Exists(dirMission)) Directory.CreateDirectory(dirMission);
                if (!Directory.Exists(dirConfig)) Directory.CreateDirectory(dirConfig);

                string[] memberNames = new string[] { "裁决者", "游隼", "泥蛇" };
                foreach (string m in memberNames)
                {
                    string mDir = Path.Combine(dirMembers, m);
                    if (!Directory.Exists(mDir)) Directory.CreateDirectory(mDir);
                }

                // 1. Mission/MISSION.md
                string missionFile = Path.Combine(dirMission, "MISSION.md");
                if (!File.Exists(missionFile))
                {
                    string missionContent = 
"# AI War Room｜Mission 现实状态\n\n" +
"- **任务目标**: 构建与验证多AI协作任务\n" +
"- **当前范围**: 核心协同网络与闭环验证\n" +
"- **当前施工点**: 首次开箱初始化完成，等待CTO指派第一项任务\n" +
"- **已完成事项**: \n" +
"  - [x] War Room 基础空间搭建\n" +
"  - [x] 10,000 Starship Credits 注入\n" +
"- **未完成事项**: \n" +
"  - [ ] 召开首次AI协作立项会议\n" +
"- **当前阻塞**: 无\n" +
"- **冻结决策**: 无\n" +
"- **下一主要动作**: CTO 启动立项并分配首轮子任务\n" +
"- **最终交付条件**: 所有任务条目全部变为 DONE 且通过人工最终验收\n";
                    File.WriteAllText(missionFile, missionContent, Encoding.UTF8);
                }

                // 2. Mission/CHECKLIST.md
                string checklistFile = Path.Combine(dirMission, "CHECKLIST.md");
                if (!File.Exists(checklistFile))
                {
                    string checklistContent =
"# CTO Task Checklist\n\n" +
"## 当前任务检查节点\n" +
"- [DONE] 初始化 War Room 空间｜责任: 裁决者｜目标: 目录与初始资本就绪\n" +
"- [TODO] 召开立项会议｜责任: CTO｜目标: 明确第一轮协作分工\n\n" +
"## 变更与调整履历 (ADJUSTED / MODIFIED 记录)\n" +
"<!-- 当条目发生 ADJUSTED 或 MODIFIED 时，必须在下方追加记录：\n" +
"### 变更记录模板:\n" +
"- **原内容**: ...\n" +
"- **现在内容**: ...\n" +
"- **变化原因**: ...\n" +
"- **责任节点**: ...\n" +
"- **影响**: ...\n" +
"- **当前验收口径**: ...\n" +
"-->\n";
                    File.WriteAllText(checklistFile, checklistContent, Encoding.UTF8);
                }

                // 3. Mission/CREDITS.md
                string creditsFile = Path.Combine(dirMission, "CREDITS.md");
                if (!File.Exists(creditsFile))
                {
                    string creditsContent = string.Format(
"# Mission Capital Ledger (Starship Credits)\n\n" +
"- **Initial**: 10000 SC\n" +
"- **Appointed CTO**: {0}\n\n" +
"## Allocations\n" +
"<!-- CTO 根据其他 AI 节点对任务的真实贡献，自主分配奖励：\n" +
"Agent-A +xxxx\n" +
"Agent-B +xxxx\n" +
"-->\n\n" +
"- **Remaining**: 10000 SC\n", cto);
                    File.WriteAllText(creditsFile, creditsContent, Encoding.UTF8);
                }

                // 4. config/war_room.json
                string configFile = Path.Combine(dirConfig, "war_room.json");
                WarRoomConfig cfg = new WarRoomConfig
                {
                    CtoName = cto,
                    WarRoomRoot = rootDir,
                    MissionCapital = 10000,
                    CurrentMission = "构建与验证多AI协作任务",
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
                };
                JavaScriptSerializer ser = new JavaScriptSerializer();
                string json = ser.Serialize(cfg);
                File.WriteAllText(configFile, json, Encoding.UTF8);

                string appLevelCfg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "war_room_config.json");
                File.WriteAllText(appLevelCfg, json, Encoding.UTF8);
            }
            catch { }
        }

        private void InitMissionWatcher()
        {
            try
            {
                if (string.IsNullOrEmpty(warRoomRootDir)) return;
                string missionDir = Path.Combine(warRoomRootDir, "Mission");
                if (!Directory.Exists(missionDir)) Directory.CreateDirectory(missionDir);

                if (missionWatcher != null)
                {
                    missionWatcher.Dispose();
                }

                missionWatcher = new FileSystemWatcher(missionDir);
                missionWatcher.Filter = "*.md";
                missionWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
                missionWatcher.Changed += (s, e) => { OnMissionFileChanged(); };
                missionWatcher.Created += (s, e) => { OnMissionFileChanged(); };
                missionWatcher.EnableRaisingEvents = true;
            }
            catch { }
        }

        private void OnMissionFileChanged()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke(new Action(() =>
            {
                Thread.Sleep(100);
                ReloadMissionSummaryFromDisk();
                ReloadChecklistFromDisk();
                UpdateWarRoomHeaderUI();
                UpdateTrayStatus();
            }));
        }

        public void ReloadMissionSummaryFromDisk()
        {
            try
            {
                if (string.IsNullOrEmpty(warRoomRootDir)) return;
                string missionPath = Path.Combine(warRoomRootDir, "Mission", "MISSION.md");
                if (!File.Exists(missionPath)) return;

                string[] lines = File.ReadAllLines(missionPath, Encoding.UTF8);
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("- **任务目标**:") || trimmed.StartsWith("任务目标:"))
                    {
                        string target = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                        if (!string.IsNullOrEmpty(target)) currentMissionTitle = target;
                        break;
                    }
                }
            }
            catch { }
        }

        public void ReloadChecklistFromDisk()
        {
            try
            {
                if (string.IsNullOrEmpty(warRoomRootDir) || gridChecklist == null) return;
                string checklistPath = Path.Combine(warRoomRootDir, "Mission", "CHECKLIST.md");
                if (!File.Exists(checklistPath)) return;

                string[] lines = File.ReadAllLines(checklistPath, Encoding.UTF8);
                List<ChecklistItem> items = new List<ChecklistItem>();

                Regex itemRegex = new Regex(@"^-\s*\[(DONE|TODO|ADJUSTED|MODIFIED|x|\s)\]\s*(?:([^｜|]+)[｜|])?\s*(.*)$", RegexOptions.IgnoreCase);

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    Match m = itemRegex.Match(trimmed);
                    if (m.Success)
                    {
                        string rawState = m.Groups[1].Value.ToUpper().Trim();
                        string state = rawState;
                        if (rawState == "X") state = "DONE";
                        else if (string.IsNullOrEmpty(rawState)) state = "TODO";

                        string partA = (m.Groups[2].Value ?? "").Trim();
                        string partB = (m.Groups[3].Value ?? "").Trim();

                        string node = "CTO";
                        string content = partA;
                        if (!string.IsNullOrEmpty(partA) && !string.IsNullOrEmpty(partB))
                        {
                            node = partA;
                            content = partB;
                        }
                        else if (string.IsNullOrEmpty(partA))
                        {
                            content = partB;
                        }

                        items.Add(new ChecklistItem
                        {
                            State = state,
                            Node = node,
                            Content = content,
                            OriginalLine = trimmed
                        });
                    }
                }

                currentChecklistItems = items;

                int countDone = 0, countTodo = 0, countAdj = 0, countMod = 0;
                foreach (var it in items)
                {
                    if (it.State == "DONE") countDone++;
                    else if (it.State == "TODO") countTodo++;
                    else if (it.State == "ADJUSTED") countAdj++;
                    else if (it.State == "MODIFIED") countMod++;
                }

                if (lblChecklistSummary != null)
                {
                    lblChecklistSummary.Text = string.Format(
                        "任务统计: 全部 {0} | DONE: {1} | TODO: {2} | ADJUSTED: {3} | MODIFIED: {4}",
                        items.Count, countDone, countTodo, countAdj, countMod
                    );
                }

                gridChecklist.Rows.Clear();
                foreach (var it in items)
                {
                    int rowIdx = gridChecklist.Rows.Add(it.State, it.Node, it.Content);
                    var row = gridChecklist.Rows[rowIdx];
                    row.Tag = it;

                    if (it.State == "DONE")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(30, 58, 47);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(166, 227, 161);
                    }
                    else if (it.State == "TODO")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(30, 42, 74);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(137, 180, 250);
                    }
                    else if (it.State == "ADJUSTED")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(61, 45, 30);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(250, 179, 135);
                    }
                    else if (it.State == "MODIFIED")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(54, 33, 74);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(203, 166, 247);
                    }
                }
            }
            catch { }
        }

        public void ReloadMeetingsFromDisk()
        {
            try
            {
                List<string> dirsToScan = new List<string>();
                if (!string.IsNullOrEmpty(warRoomRootDir))
                {
                    string wmDir = Path.Combine(warRoomRootDir, "Meetings");
                    if (Directory.Exists(wmDir)) dirsToScan.Add(wmDir);
                }
                if (!string.IsNullOrEmpty(meetingDir) && Directory.Exists(meetingDir) && !dirsToScan.Contains(meetingDir))
                {
                    dirsToScan.Add(meetingDir);
                }

                List<MeetingRowItem> items = new List<MeetingRowItem>();
                Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");

                foreach (string d in dirsToScan)
                {
                    string[] files = Directory.GetFiles(d, "*.*");
                    foreach (string f in files)
                    {
                        string fname = Path.GetFileName(f);
                        Match m = r.Match(fname);
                        if (m.Success)
                        {
                            string rawTime = m.Groups[1].Value;
                            string timeDisplay = rawTime.Length >= 12 ? rawTime.Substring(8, 2) + ":" + rawTime.Substring(10, 2) : rawTime;
                            string sender = m.Groups[2].Value.Trim();
                            string recipient = m.Groups[3].Value.Trim();
                            string title = m.Groups[4].Value.Trim();

                            string actionType = "TASK";
                            string upperTitle = title.ToUpper();
                            if (upperTitle.Contains("TO_HUMAN") || upperTitle.Contains("HUMAN")) actionType = "TO_HUMAN";
                            else if (upperTitle.Contains("FINAL") || upperTitle.Contains("闭环") || upperTitle.Contains("验收通过")) actionType = "FINAL";
                            else if (upperTitle.Contains("PATCH_DONE") || upperTitle.Contains("补丁完成")) actionType = "PATCH_DONE";
                            else if (upperTitle.Contains("NEEDS_PATCH") || upperTitle.Contains("要求补丁")) actionType = "NEEDS_PATCH";
                            else if (upperTitle.Contains("VERIFY") || upperTitle.Contains("验证")) actionType = "VERIFY";
                            else if (upperTitle.Contains("REVIEW") || upperTitle.Contains("审查") || upperTitle.Contains("评估")) actionType = "REVIEW";
                            else if (upperTitle.Contains("RESULT") || upperTitle.Contains("回执") || upperTitle.Contains("交付")) actionType = "RESULT";
                            else if (upperTitle.Contains("PASS")) actionType = "PASS";

                            items.Add(new MeetingRowItem
                            {
                                Timestamp = timeDisplay,
                                Type = actionType,
                                Sender = sender,
                                Recipient = recipient,
                                Title = title,
                                FilePath = f,
                                FileName = fname
                            });
                        }
                    }
                }

                items.Sort((a, b) => string.Compare(b.FileName, a.FileName, StringComparison.OrdinalIgnoreCase));
                currentMeetingItems = items;

                if (lblMeetingsSummary != null)
                {
                    lblMeetingsSummary.Text = string.Format("会议动态: 索引共 {0} 篇真实信件 (双击打开原件)", items.Count);
                }

                if (gridMeetings != null)
                {
                    gridMeetings.Rows.Clear();
                    foreach (var it in items)
                    {
                        int rowIdx = gridMeetings.Rows.Add(it.Timestamp, it.Type, it.Sender, it.Recipient, it.Title);
                        var row = gridMeetings.Rows[rowIdx];
                        row.Tag = it;

                        if (it.Type == "TO_HUMAN")
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(69, 40, 55);
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(243, 139, 168);
                        }
                        else if (it.Type == "FINAL" || it.Type == "PASS")
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(30, 58, 47);
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(166, 227, 161);
                        }
                        else if (it.Type == "NEEDS_PATCH")
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(61, 45, 30);
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(250, 179, 135);
                        }
                    }
                }
            }
            catch { }
        }

        
        // ====================================================================
        // Grid Multi-Select, Context Menu & Deletion Actions
        // ====================================================================

        private void SelectAllChecklistRows()
        {
            if (gridChecklist == null) return;
            foreach (DataGridViewRow row in gridChecklist.Rows)
            {
                row.Selected = true;
            }
        }

        private void DeleteSelectedChecklistRows()
        {
            if (gridChecklist == null || gridChecklist.SelectedRows.Count == 0) return;

            DialogResult dr = MessageBox.Show(
                this,
                string.Format("确定要删除选中的 {0} 项任务条目吗？\n此操作将直接同步更新 CHECKLIST.md 事实源文件。", gridChecklist.SelectedRows.Count),
                "确认删除任务",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (dr != DialogResult.Yes) return;

            try
            {
                HashSet<string> linesToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataGridViewRow row in gridChecklist.SelectedRows)
                {
                    ChecklistItem item = row.Tag as ChecklistItem;
                    if (item != null && !string.IsNullOrEmpty(item.OriginalLine))
                    {
                        linesToRemove.Add(item.OriginalLine.Trim());
                    }
                }

                string checklistPath = Path.Combine(warRoomRootDir, "Mission", "CHECKLIST.md");
                if (File.Exists(checklistPath))
                {
                    string[] allLines = File.ReadAllLines(checklistPath, Encoding.UTF8);
                    List<string> remainingLines = new List<string>();
                    foreach (string line in allLines)
                    {
                        if (!linesToRemove.Contains(line.Trim()))
                        {
                            remainingLines.Add(line);
                        }
                    }
                    File.WriteAllLines(checklistPath, remainingLines.ToArray(), Encoding.UTF8);
                }

                ReloadChecklistFromDisk();
                LogAudit("CHECKLIST_DELETED", string.Format("Deleted {0} checklist items by Human.", linesToRemove.Count));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "删除任务异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectAllMeetingRows()
        {
            if (gridMeetings == null) return;
            foreach (DataGridViewRow row in gridMeetings.Rows)
            {
                row.Selected = true;
            }
        }

        private void DeleteSelectedMeetingRows()
        {
            if (gridMeetings == null || gridMeetings.SelectedRows.Count == 0) return;

            DialogResult dr = MessageBox.Show(
                this,
                string.Format("确定要删除选中的 {0} 篇会议文件原件吗？\n对应文件将直接从磁盘物理删除。", gridMeetings.SelectedRows.Count),
                "确认删除会议信件",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (dr != DialogResult.Yes) return;

            try
            {
                int deletedCount = 0;
                foreach (DataGridViewRow row in gridMeetings.SelectedRows)
                {
                    MeetingRowItem item = row.Tag as MeetingRowItem;
                    if (item != null && !string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                    {
                        try
                        {
                            File.Delete(item.FilePath);
                            deletedCount++;
                        }
                        catch { }
                    }
                }

                ReloadMeetingsFromDisk();
                LogAudit("MEETINGS_DELETED", string.Format("Deleted {0} meeting files by Human.", deletedCount));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "删除会议文件异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenChecklistFile()
        {
            try
            {
                if (string.IsNullOrEmpty(warRoomRootDir)) return;
                string p = Path.Combine(warRoomRootDir, "Mission", "CHECKLIST.md");
                if (File.Exists(p))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(p) { UseShellExecute = true });
                }
            }
            catch { }
        }

        private void OpenMeetingsDirectory()
        {
            try
            {
                string targetDir = null;
                if (!string.IsNullOrEmpty(warRoomRootDir))
                {
                    string d = Path.Combine(warRoomRootDir, "Meetings");
                    if (Directory.Exists(d)) targetDir = d;
                }
                if (string.IsNullOrEmpty(targetDir) && !string.IsNullOrEmpty(meetingDir) && Directory.Exists(meetingDir))
                {
                    targetDir = meetingDir;
                }
                if (!string.IsNullOrEmpty(targetDir))
                {
                    System.Diagnostics.Process.Start("explorer.exe", targetDir);
                }
            }
            catch { }
        }

        private void OpenSelectedMeetingFile()
        {
            try
            {
                if (gridMeetings != null && gridMeetings.SelectedRows.Count > 0)
                {
                    MeetingRowItem item = gridMeetings.SelectedRows[0].Tag as MeetingRowItem;
                    if (item != null && !string.IsNullOrEmpty(item.FilePath) && File.Exists(item.FilePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(item.FilePath) { UseShellExecute = true });
                    }
                }
            }
            catch { }
        }

        private void UpdateWarRoomHeaderUI()
        {
            if (lblWarRoomHeader != null)
            {
                lblWarRoomHeader.Text = string.Format("🏢 AI War Room | CTO: {0} | 资本: {1} SC", ctoName, missionCapital);
            }
            if (lblMissionSubtitle != null)
            {
                string alertText = pendingToHumanCount > 0 ? string.Format(" | ⚠️ 敲门待办: {0}", pendingToHumanCount) : "";
                lblMissionSubtitle.Text = string.Format("🎯 Mission: {0}{1}", currentMissionTitle, alertText);
            }
        }

        private void UpdateTrayStatus()
        {
            if (trayIcon == null) return;
            string prefix = (instanceOpts != null && instanceOpts.IsTestInstance) 
                ? string.Format("[T:{0}] ", instanceOpts.Port) 
                : string.Format("[P:{0}] ", instanceOpts != null ? instanceOpts.Port : 8787);
            string text = string.Format("{0}CTO:{1}|M:{2}|H:{3}", prefix, ctoName, currentMissionTitle, pendingToHumanCount);
            if (text.Length > 63)
            {
                text = text.Substring(0, 60) + "...";
            }
            trayIcon.Text = text;
        }

        private void ShowTrayPreviewPopup()
        {
            try
            {
                TrayPreviewPopupForm popup = new TrayPreviewPopupForm(
                    ctoName,
                    currentMissionTitle,
                    missionCapital,
                    pendingToHumanCount,
                    currentMeetingItems,
                    () => { ShowFloatingWindow(); if (tabWarRoom != null) tabWarRoom.SelectedIndex = 0; },
                    () => { OpenMeetingsDirectory(); },
                    () => { ShowFloatingWindow(); }
                );
                popup.Show();
            }
            catch { }
        }

        private void ToggleWatchMode()
        {
            isWatchingActive = !isWatchingActive;
            if (isWatchingActive)
            {
                lblWatchStatus.Text = "哨兵: 🟢 监听会议目录中";
                lblWatchStatus.ForeColor = Color.FromArgb(166, 227, 161);
                btnToggleWatch.Text = "暂停";
                LogAudit("WATCHER_RESUMED", "Auto-knocking resumed by Human Root. Running catch-up scan...");
                CatchUpScan();
            }
            else
            {
                lblWatchStatus.Text = "哨兵: ⏸️ 已暂停自动监听";
                lblWatchStatus.ForeColor = Color.FromArgb(249, 226, 175);
                btnToggleWatch.Text = "恢复";
                LogAudit("WATCHER_PAUSED", "Auto-knocking paused by Human Root.");
            }
        }

        private void InitRetryTimer()
        {
            retryTimer = new System.Windows.Forms.Timer();
            retryTimer.Interval = 5000;
            retryTimer.Tick += OnRetryTimerTick;
            retryTimer.Start();
        }

        private void OnRetryTimerTick(object sender, EventArgs e)
        {
            if (!isWatchingActive) return;

            List<PendingRetryItem> dueItems = new List<PendingRetryItem>();
            lock (stateLock)
            {
                DateTime now = DateTime.UtcNow;
                for (int i = pendingRetries.Count - 1; i >= 0; i--)
                {
                    if (now >= pendingRetries[i].NextAttemptUtc)
                    {
                        dueItems.Add(pendingRetries[i]);
                        pendingRetries.RemoveAt(i);
                    }
                }
            }

            foreach (var item in dueItems)
            {
                bool success = DispatchDoorbellToNode(item.NodeName, item.FileName, item.Title);
                string targetKey = string.Format("{0}|{1}|{2}", item.FileName, item.Sha256, item.NodeName);
                if (success)
                {
                    lock (stateLock)
                    {
                        pendingRetryKeys.Remove(targetKey);
                    }
                    MarkKeyProcessed(targetKey);
                    LogAudit(DeliveryStage.INJECTION_ATTEMPTED.ToString(), string.Format("Retry injection succeeded for [{0}] <- {1}", item.NodeName, item.FileName));
                }
                else
                {
                    item.AttemptCount++;
                    if (item.AttemptCount >= 5)
                    {
                        lock (stateLock)
                        {
                            pendingRetryKeys.Remove(targetKey);
                        }
                        LogAudit("RETRY_EXHAUSTED", string.Format("Giving up retries for [{0}] <- {1} after {2} attempts.", item.NodeName, item.FileName, item.AttemptCount));

                        // P0: Continuous wake failure triggers Human Notification
                        string exhaustDedupKey = string.Format("TO_HUMAN_EXHAUST|{0}|{1}", item.FileName, item.NodeName);
                        bool notifyExhaust = false;
                        lock (stateLock)
                        {
                            if (!notifiedHumanKeys.Contains(exhaustDedupKey))
                            {
                                notifiedHumanKeys.Add(exhaustDedupKey);
                                notifyExhaust = true;
                            }
                        }
                        if (notifyExhaust)
                        {
                            ToHumanEvent exhaustEvt = new ToHumanEvent
                            {
                                Sender = "Dispatcher哨兵",
                                Recipient = "Human",
                                TaskId = item.FileName,
                                TaskTitle = item.Title,
                                Reason = string.Format("节点 [{0}] 连续唤醒失败（已重试 {1} 次），需人工介入", item.NodeName, item.AttemptCount),
                                Summary = string.Format("未检测到目标节点 [{0}] 的活动窗口，注入重试已耗尽。请检查 IDE 是否已启动且处于正常状态。", item.NodeName),
                                FilePath = item.FilePath,
                                Sha256 = item.Sha256,
                                DedupKey = exhaustDedupKey
                            };
                            ShowHumanNotification(exhaustEvt);
                            PlayDoubleKnockSound();
                            LogAudit("TO_HUMAN_TRIGGERED", exhaustEvt.Reason);
                            MarkKeyProcessed(exhaustDedupKey);
                        }
                    }
                    else
                    {
                        item.NextAttemptUtc = DateTime.UtcNow.AddSeconds(Math.Min(60, 10 * item.AttemptCount));
                        lock (stateLock)
                        {
                            pendingRetries.Add(item);
                        }
                    }
                }
            }
        }

        private void InitAutoWatcher()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // Walk up to find aiFundTournament root
                DirectoryInfo d = new DirectoryInfo(baseDir);
                while (d != null && !Directory.Exists(Path.Combine(d.FullName, "CodeAi")))
                {
                    d = d.Parent;
                }

                string projectRoot = d != null ? d.FullName : baseDir;

                // 独立会议室路径隔离
                if (instanceOpts != null && !string.IsNullOrEmpty(instanceOpts.MeetingDir))
                {
                    meetingDir = instanceOpts.MeetingDir;
                }
                else if (instanceOpts != null && instanceOpts.IsTestInstance)
                {
                    if (!string.IsNullOrEmpty(warRoomRootDir))
                    {
                        meetingDir = Path.Combine(warRoomRootDir, "Meetings");
                    }
                    else
                    {
                        meetingDir = Path.Combine(projectRoot, "AI-War-Room-Test", "Meetings");
                    }
                }
                else
                {
                    meetingDir = Path.Combine(projectRoot, "CodeAi", "会议");
                }

                // 独立运行态与状态文件隔离
                if (instanceOpts != null && !string.IsNullOrEmpty(instanceOpts.RuntimeDir))
                {
                    runtimeStateDir = instanceOpts.RuntimeDir;
                }
                else if (instanceOpts != null && instanceOpts.IsTestInstance)
                {
                    runtimeStateDir = Path.Combine(projectRoot, "CodeAi", "运行态", string.Format("dispatcher_{0}_{1}", instanceOpts.InstanceId, instanceOpts.Port));
                }
                else
                {
                    runtimeStateDir = Path.Combine(projectRoot, "CodeAi", "运行态", "dispatcher");
                }

                if (!Directory.Exists(meetingDir)) Directory.CreateDirectory(meetingDir);
                if (!Directory.Exists(runtimeStateDir)) Directory.CreateDirectory(runtimeStateDir);

                stateFilePath = Path.Combine(runtimeStateDir, "processed_meetings.txt");
                LoadProcessedBaseline();

                meetingWatcher = new FileSystemWatcher(meetingDir);
                meetingWatcher.Filter = "*.*";
                meetingWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                meetingWatcher.Created += OnMeetingFileEvent;
                meetingWatcher.Changed += OnMeetingFileEvent;
                meetingWatcher.EnableRaisingEvents = true;

                LogAudit("WATCHER_INITIALIZED", "Monitoring: " + meetingDir);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "哨兵初始化异常: " + ex.Message;
            }
        }

        private void LoadProcessedBaseline()
        {
            lock (stateLock)
            {
                processedKeys.Clear();
                oldTwoPartKeys.Clear();
                taskDeliveriesJsonlPath = Path.Combine(runtimeStateDir, "task_deliveries.jsonl");
                LoadRecentDeliveriesIntoGrid();

                if (File.Exists(stateFilePath))
                {
                    string[] lines = File.ReadAllLines(stateFilePath, Encoding.UTF8);
                    bool hasLegacyKeys = false;
                    List<string> migratedOutput = new List<string>();

                    foreach (string line in lines)
                    {
                        string trimmed = line.Trim();
                        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

                        if (trimmed.StartsWith("TO_HUMAN|", StringComparison.OrdinalIgnoreCase) ||
                            trimmed.StartsWith("TO_HUMAN_EXHAUST|", StringComparison.OrdinalIgnoreCase))
                        {
                            notifiedHumanKeys.Add(trimmed);
                            processedKeys.Add(trimmed);
                            migratedOutput.Add(trimmed);
                            continue;
                        }

                        string[] parts = trimmed.Split('|');
                        if (parts.Length == 2)
                        {
                            hasLegacyKeys = true;
                            oldTwoPartKeys.Add(trimmed);
                            migratedOutput.Add(trimmed);

                            // Parse and migrate to v2 3-part keys
                            string fName = parts[0];
                            string sha = parts[1];
                            Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");
                            Match m = r.Match(fName);
                            if (m.Success)
                            {
                                string sender = m.Groups[2].Value.Trim();
                                string recipient = m.Groups[3].Value.Trim();
                                string title = m.Groups[4].Value.Trim();
                                List<string> targets = ResolveTargetNodes(sender, recipient, title);
                                if (targets.Count == 0)
                                {
                                    string k = string.Format("{0}|{1}|NONE", fName, sha);
                                    processedKeys.Add(k);
                                    migratedOutput.Add(k);
                                }
                                else
                                {
                                    foreach (string t in targets)
                                    {
                                        string k = string.Format("{0}|{1}|{2}", fName, sha, t);
                                        processedKeys.Add(k);
                                        migratedOutput.Add(k);
                                    }
                                }
                            }
                            else
                            {
                                string k = string.Format("{0}|{1}|NONE", fName, sha);
                                processedKeys.Add(k);
                                migratedOutput.Add(k);
                            }
                        }
                        else
                        {
                            processedKeys.Add(trimmed);
                            migratedOutput.Add(trimmed);
                        }
                    }

                    if (hasLegacyKeys)
                    {
                        try
                        {
                            File.WriteAllLines(stateFilePath, migratedOutput.ToArray(), Encoding.UTF8);
                            LogAudit("STATE_V1_MIGRATED", string.Format("Migrated {0} legacy v1 keys to v2 three-part keys.", oldTwoPartKeys.Count));
                        }
                        catch (Exception ex)
                        {
                            LogAudit("STATE_MIGRATION_ERROR", ex.Message);
                        }
                    }
                }
                else
                {
                    // P0-1: First install / no state file -> establish baseline without backfilling or dispatching
                    if (Directory.Exists(meetingDir))
                    {
                        List<string> baselineKeys = new List<string>();
                        string[] files = Directory.GetFiles(meetingDir, "*.*");
                        foreach (string file in files)
                        {
                            string ext = Path.GetExtension(file).ToLower();
                            if (ext != ".txt" && ext != ".md") continue;

                            string sha256 = ComputeFileSha256(file);
                            if (string.IsNullOrEmpty(sha256)) continue;

                            string fName = Path.GetFileName(file);
                            Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");
                            Match m = r.Match(fName);
                            if (!m.Success) continue;

                            string sender = m.Groups[2].Value.Trim();
                            string recipient = m.Groups[3].Value.Trim();
                            string title = m.Groups[4].Value.Trim();
                            List<string> targets = ResolveTargetNodes(sender, recipient, title);
                            if (targets.Count == 0)
                            {
                                string k = string.Format("{0}|{1}|NONE", fName, sha256);
                                baselineKeys.Add(k);
                                processedKeys.Add(k);
                            }
                            else
                            {
                                foreach (string node in targets)
                                {
                                    string k = string.Format("{0}|{1}|{2}", fName, sha256, node);
                                    baselineKeys.Add(k);
                                    processedKeys.Add(k);
                                }
                            }
                        }
                        File.WriteAllLines(stateFilePath, baselineKeys.ToArray(), Encoding.UTF8);
                        LogAudit("BASELINE_ESTABLISHED", string.Format("Baseline indexed {0} existing keys across {1} files without dispatch.", baselineKeys.Count, files.Length));
                    }
                }

                // P0: Baseline existing meeting files for TO_HUMAN to prevent notification storm on boot
                try
                {
                    if (Directory.Exists(meetingDir))
                    {
                        foreach (string file in Directory.GetFiles(meetingDir, "*.*"))
                        {
                            string fName = Path.GetFileName(file);
                            string sha = ComputeFileSha256(file);
                            if (!string.IsNullOrEmpty(sha))
                            {
                                notifiedHumanKeys.Add(string.Format("TO_HUMAN|{0}|{1}", fName, sha));
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void CatchUpScan()
        {
            if (!Directory.Exists(meetingDir)) return;
            try
            {
                string[] files = Directory.GetFiles(meetingDir, "*.*");
                Array.Sort(files);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext != ".txt" && ext != ".md") continue;

                    string fileName = Path.GetFileName(file);
                    Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");
                    Match m = r.Match(fileName);
                    if (!m.Success) continue;

                    string senderName = m.Groups[2].Value.Trim();
                    string recipientName = m.Groups[3].Value.Trim();
                    string title = m.Groups[4].Value.Trim();

                    string sha256 = ComputeFileSha256(file);
                    if (string.IsNullOrEmpty(sha256)) continue;

                    // P0-1: Backward compatibility check for historical 2-part key
                    string oldKey = string.Format("{0}|{1}", fileName, sha256);
                    lock (stateLock)
                    {
                        if (oldTwoPartKeys.Contains(oldKey))
                        {
                            continue; // Processed under v1 scheme; do not replay!
                        }
                    }

                    List<string> targetNodes = ResolveTargetNodes(senderName, recipientName, title);
                    bool needsDispatch = false;

                    if (targetNodes.Count == 0)
                    {
                        string bridgeKey = string.Format("{0}|{1}|NONE", fileName, sha256);
                        lock (stateLock)
                        {
                            if (!processedKeys.Contains(bridgeKey))
                            {
                                MarkKeyProcessed(bridgeKey);
                            }
                        }
                        continue;
                    }

                    foreach (string node in targetNodes)
                    {
                        string targetKey = string.Format("{0}|{1}|{2}", fileName, sha256, node);
                        lock (stateLock)
                        {
                            if (!processedKeys.Contains(targetKey))
                            {
                                needsDispatch = true;
                                break;
                            }
                        }
                    }

                    if (needsDispatch)
                    {
                        LogAudit("CATCHUP_SCAN_ENQUEUE", "Catch-up scan queued file: " + fileName);
                        Task.Run(() => ProcessMeetingFileSafely(file));
                    }
                }
            }
            catch (Exception ex)
            {
                LogAudit("CATCHUP_SCAN_ERROR", ex.Message);
            }
        }

        private void OnMeetingFileEvent(object sender, FileSystemEventArgs e)
        {
            if (!isWatchingActive) return;

            string ext = Path.GetExtension(e.FullPath).ToLower();
            if (ext != ".txt" && ext != ".md") return;

            Task.Run(() => ProcessMeetingFileSafely(e.FullPath));
        }

        private void ProcessMeetingFileSafely(string filePath)
        {
            if (!isWatchingActive) return;
            string fileName = Path.GetFileName(filePath);

            Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");
            Match m = r.Match(fileName);
            if (!m.Success)
            {
                return; // Non-collab file, ignore
            }

            string senderName = m.Groups[2].Value.Trim();
            string recipientName = m.Groups[3].Value.Trim();
            string title = m.Groups[4].Value.Trim();

            // Stabilize file write
            if (!WaitForFileAvailable(filePath, 2000))
            {
                LogAudit("FILE_LOCKED_TIMEOUT", fileName);
                return;
            }

            string sha256 = ComputeFileSha256(filePath);
            if (string.IsNullOrEmpty(sha256)) return;

            string fileKey = string.Format("{0}|{1}", fileName, sha256);
            lock (stateLock)
            {
                if (inFlightKeys.Contains(fileKey))
                {
                    return; // Avoid concurrent execution
                }
                inFlightKeys.Add(fileKey);
            }

            try
            {
                // P1-1: Parse controlled status header from file content (do not guess from filename!)
                string fileStatus = "OPEN";
                string relatedTask = null;

                try
                {
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        Match mStatus = Regex.Match(line, @"^状态[：:]\s*(.+)$");
                        if (mStatus.Success) fileStatus = mStatus.Groups[1].Value.Trim();

                        Match mRel = Regex.Match(line, @"^关联(?:任务|回执|验收)[：:]\s*(.+)$");
                        if (mRel.Success)
                        {
                            string raw = mRel.Groups[1].Value.Trim();
                            string baseName = Path.GetFileName(raw); // Strip any path traversal
                            if (File.Exists(Path.Combine(meetingDir, baseName)))
                            {
                                relatedTask = baseName;
                            }
                        }
                    }
                }
                catch { }

                bool isReceipt;
                DeliveryStage stage;
                ParseStatusTokens(fileStatus, out isReceipt, out stage);

                if (isReceipt && !string.IsNullOrEmpty(relatedTask))
                {
                    List<string> origTargets = ResolveOriginalTaskTargets(relatedTask, senderName);
                    foreach (string origTarget in origTargets)
                    {
                        LogAudit(stage.ToString(), string.Format("Receipt [{0}] (status:{1}) mapped to original task [{2}] target [{3}]", fileName, fileStatus, relatedTask, origTarget));
                        UpdateTaskDeliveryState(relatedTask, origTarget, stage.ToString(), fileName);
                    }
                }

                LogAudit(DeliveryStage.FILE_DISCOVERED.ToString(), string.Format("{0} (status:{1}) -> sender:{2}, recipient:{3}", fileName, fileStatus, senderName, recipientName));

                // P0: Structured TO_HUMAN Detection & Notification (Sound + Card)
                ToHumanEvent humanEvt = DetectStructuredToHuman(filePath, fileName, senderName, recipientName, title, sha256);
                if (humanEvt != null)
                {
                    string humanDedupKey = string.Format("TO_HUMAN|{0}|{1}", fileName, sha256);
                    bool shouldNotify = false;
                    lock (stateLock)
                    {
                        if (!notifiedHumanKeys.Contains(humanDedupKey))
                        {
                            notifiedHumanKeys.Add(humanDedupKey);
                            shouldNotify = true;
                        }
                    }

                    if (shouldNotify)
                    {
                        LogAudit("TO_HUMAN_TRIGGERED", string.Format("Structured TO_HUMAN in [{0}]: sender={1}, reason={2}", fileName, humanEvt.Sender, humanEvt.Reason));
                        ShowHumanNotification(humanEvt);
                        PlayDoubleKnockSound();
                        LogAudit("TO_HUMAN_NOTIFIED", string.Format("Notified human for [{0}]", fileName));
                        MarkKeyProcessed(humanDedupKey);
                    }
                }

                // Determine target desktop nodes
                List<string> targetNodes = ResolveTargetNodes(senderName, recipientName, title);
                if (targetNodes.Count == 0)
                {
                    // Handled by bridge (to折叠主机) or no desktop recipient
                    MarkKeyProcessed(string.Format("{0}|{1}|NONE", fileName, sha256));
                    return;
                }

                // Knock the doors for resolved nodes (P1-1)
                foreach (string node in targetNodes)
                {
                    string targetKey = string.Format("{0}|{1}|{2}", fileName, sha256, node);
                    lock (stateLock)
                    {
                        if (processedKeys.Contains(targetKey) || oldTwoPartKeys.Contains(fileKey))
                        {
                            continue;
                        }
                    }

                    bool attempted = DispatchDoorbellToNode(node, fileName, title);
                    if (attempted)
                    {
                        MarkKeyProcessed(targetKey);
                        UpdateTaskDeliveryState(fileName, node, isReceipt ? stage.ToString() : DeliveryStage.INJECTION_ATTEMPTED.ToString(), null);
                    }
                    else
                    {
                        // Target window not found: schedule retry
                        lock (stateLock)
                        {
                            string retryKey = string.Format("{0}|{1}|{2}", fileName, sha256, node);
                            if (!pendingRetryKeys.Contains(retryKey))
                            {
                                pendingRetryKeys.Add(retryKey);
                                pendingRetries.Add(new PendingRetryItem
                                {
                                    NodeName = node,
                                    FilePath = filePath,
                                    FileName = fileName,
                                    Title = title,
                                    Sha256 = sha256,
                                    NextAttemptUtc = DateTime.UtcNow.AddSeconds(10),
                                    AttemptCount = 1
                                });
                                LogAudit("RETRY_SCHEDULED", string.Format("Scheduled backoff retry for [{0}] <- {1}", node, fileName));
                            }
                        }
                    }
                }

                // Anti-recursion: If this file itself is a receipt, mark terminal directly
                if (isReceipt)
                {
                    MarkKeyProcessed(string.Format("{0}|{1}|RECEIPT_CLOSED", fileName, sha256));
                }
            }
            finally
            {
                lock (stateLock)
                {
                    inFlightKeys.Remove(fileKey);
                }
            }
        }

        private void UpdateTaskDeliveryState(string taskFile, string targetNode, string stage, string receiptFile)
        {
            try
            {
                lock (stateLock)
                {
                    if (string.IsNullOrEmpty(taskDeliveriesJsonlPath)) return;
                    string jsonLine = string.Format(
                        "{{\"task_file\":\"{0}\",\"target_node\":\"{1}\",\"stage\":\"{2}\",\"updated_utc\":\"{3:o}\",\"receipt_file\":\"{4}\"}}",
                        taskFile.Replace("\\", "\\\\").Replace("\"", "\\\""),
                        targetNode.Replace("\"", "\\\""),
                        stage,
                        DateTime.UtcNow,
                        (receiptFile ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"")
                    );

                    File.AppendAllText(taskDeliveriesJsonlPath, jsonLine + Environment.NewLine, Encoding.UTF8);

                    // Update GUI grid with rainbow colors and alert sound
                    UpdateDeliveryGrid(DateTime.Now.ToString("HH:mm:ss"), targetNode, stage, taskFile, true);
                }
            }
            catch { }
        }

        private void UpdateDeliveryGrid(string timeStr, string targetNode, string stage, string taskFile, bool playSound)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (gridDeliveries != null)
                    {
                        rainbowColorIndex = (rainbowColorIndex + 1) % RainbowColors.Length;
                        Color rowColor = RainbowColors[rainbowColorIndex];

                        // Extract sender comrade from taskFile
                        string sender = "";
                        Match m = Regex.Match(taskFile, @"·协作·([^·]+)to");
                        if (m.Success)
                        {
                            sender = m.Groups[1].Value.Trim();
                            if (sender.EndsWith("H") && sender.Length > 1) sender = sender.Substring(0, sender.Length - 1);
                        }
                        if (string.IsNullOrEmpty(sender)) sender = "系统";

                        string stageWithTarget = string.IsNullOrEmpty(targetNode) ? stage : string.Format("{0} ({1})", stage, targetNode);

                        gridDeliveries.Rows.Insert(0, timeStr, sender, stageWithTarget, taskFile);
                        DataGridViewRow row = gridDeliveries.Rows[0];
                        row.DefaultCellStyle.ForeColor = rowColor;
                        row.DefaultCellStyle.BackColor = Color.FromArgb(17, 17, 27);
                        row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(49, 50, 68);
                        row.DefaultCellStyle.SelectionForeColor = rowColor;

                        while (gridDeliveries.Rows.Count > 30)
                        {
                            gridDeliveries.Rows.RemoveAt(gridDeliveries.Rows.Count - 1);
                        }

                        if (playSound)
                        {
                            try
                            {
                                System.Media.SystemSounds.Asterisk.Play();
                            }
                            catch { }
                        }
                    }
                }));
            }
        }

        private void LoadRecentDeliveriesIntoGrid()
        {
            try
            {
                if (File.Exists(taskDeliveriesJsonlPath))
                {
                    string[] lines = File.ReadAllLines(taskDeliveriesJsonlPath, Encoding.UTF8);
                    int start = Math.Max(0, lines.Length - 20);
                    for (int i = lines.Length - 1; i >= start; i--)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        Match mTask = Regex.Match(line, "\"task_file\":\"([^\"]+)\"");
                        Match mNode = Regex.Match(line, "\"target_node\":\"([^\"]+)\"");
                        Match mStage = Regex.Match(line, "\"stage\":\"([^\"]+)\"");
                        Match mTime = Regex.Match(line, "\"updated_utc\":\"([^\"]+)\"");

                        if (mTask.Success && mNode.Success && mStage.Success)
                        {
                            string tNode = mNode.Groups[1].Value;
                            string tStage = mStage.Groups[1].Value;
                            string tFile = mTask.Groups[1].Value;
                            string tTime = "";
                            if (mTime.Success)
                            {
                                DateTime dt;
                                if (DateTime.TryParse(mTime.Groups[1].Value, out dt))
                                {
                                    tTime = dt.ToLocalTime().ToString("HH:mm:ss");
                                }
                            }
                            if (string.IsNullOrEmpty(tTime)) tTime = DateTime.Now.ToString("HH:mm:ss");

                            UpdateDeliveryGrid(tTime, tNode, tStage, tFile, false);
                        }
                    }
                }
            }
            catch { }
        }

        private void SelectAllDeliveryRows()
        {
            if (gridDeliveries == null || gridDeliveries.Rows.Count == 0) return;
            gridDeliveries.SelectAll();
        }

        private void DeleteSelectedDeliveryRows()
        {
            if (gridDeliveries == null || gridDeliveries.SelectedRows.Count == 0) return;
            List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in gridDeliveries.SelectedRows)
            {
                rowsToDelete.Add(row);
            }
            foreach (DataGridViewRow row in rowsToDelete)
            {
                gridDeliveries.Rows.Remove(row);
            }
            lblStatus.Text = string.Format("已从列表移除 {0} 条投递记录", rowsToDelete.Count);
        }

        private void AutoDetectPartnerCheckboxes()
        {
            foreach (var node in configuredNodes)
            {
                if (partnerCheckBoxes.ContainsKey(node.Name))
                {
                    partnerCheckBoxes[node.Name].Checked = IsNodeAppRunning(node);
                }
            }
        }

        private bool IsNodeAppRunning(TargetNodeConfig node)
        {
            if (node == null) return false;
            try
            {
                if (node.WindowKeywords != null && node.WindowKeywords.Length > 0)
                {
                    if (FindTargetWindow(node.WindowKeywords) != IntPtr.Zero)
                        return true;
                }
            }
            catch { }

            try
            {
                if (node.ProcessNames != null && node.ProcessNames.Length > 0)
                {
                    System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();
                    foreach (var p in procs)
                    {
                        try
                        {
                            string pName = p.ProcessName.ToLower();
                            foreach (var targetPName in node.ProcessNames)
                            {
                                if (pName.Contains(targetPName.ToLower())) return true;
                            }
                        }
                        catch { }
                        finally
                        {
                            p.Dispose();
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        private void LoadNodeConfigs()
        {
            configuredNodes = new List<TargetNodeConfig>();
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "dispatcher_nodes.json");
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dispatcher_nodes.json");
            }

            if (File.Exists(configPath))
            {
                try
                {
                    string jsonText = File.ReadAllText(configPath, Encoding.UTF8);
                    var serializer = new JavaScriptSerializer();
                    var dict = serializer.Deserialize<Dictionary<string, object>>(jsonText);
                    if (dict != null)
                    {
                        if (dict.ContainsKey("human_notification") && dict["human_notification"] is Dictionary<string, object>)
                        {
                            var hn = (Dictionary<string, object>)dict["human_notification"];
                            if (hn.ContainsKey("sound_enabled")) soundEnabled = Convert.ToBoolean(hn["sound_enabled"]);
                            if (hn.ContainsKey("sound_file")) soundFile = Convert.ToString(hn["sound_file"]);
                        }

                        if (dict.ContainsKey("nodes"))
                        {
                            var rawNodes = dict["nodes"] as System.Collections.ArrayList;
                        if (rawNodes != null)
                        {
                            foreach (Dictionary<string, object> n in rawNodes)
                            {
                                TargetNodeConfig cfg = new TargetNodeConfig();
                                cfg.Id = n.ContainsKey("id") ? Convert.ToString(n["id"]) : "";
                                cfg.Name = n.ContainsKey("name") ? Convert.ToString(n["name"]) : "";
                                cfg.App = n.ContainsKey("app") ? Convert.ToString(n["app"]) : "";
                                cfg.UiaStrategy = n.ContainsKey("uia_strategy") ? Convert.ToString(n["uia_strategy"]) : "";
                                cfg.ColorHex = n.ContainsKey("color") ? Convert.ToString(n["color"]) : "#cdd6f4";
                                cfg.DefaultSelected = n.ContainsKey("default_selected") ? Convert.ToBoolean(n["default_selected"]) : false;

                                if (n.ContainsKey("aliases") && n["aliases"] is System.Collections.ArrayList)
                                {
                                    var arr = (System.Collections.ArrayList)n["aliases"];
                                    List<string> list = new List<string>();
                                    foreach (var item in arr) list.Add(Convert.ToString(item));
                                    cfg.Aliases = list.ToArray();
                                }
                                else cfg.Aliases = new string[] { cfg.Name };

                                if (n.ContainsKey("process_names") && n["process_names"] is System.Collections.ArrayList)
                                {
                                    var arr = (System.Collections.ArrayList)n["process_names"];
                                    List<string> list = new List<string>();
                                    foreach (var item in arr) list.Add(Convert.ToString(item));
                                    cfg.ProcessNames = list.ToArray();
                                }
                                else cfg.ProcessNames = new string[0];

                                if (n.ContainsKey("window_keywords") && n["window_keywords"] is System.Collections.ArrayList)
                                {
                                    var arr = (System.Collections.ArrayList)n["window_keywords"];
                                    List<string> list = new List<string>();
                                    foreach (var item in arr) list.Add(Convert.ToString(item));
                                    cfg.WindowKeywords = list.ToArray();
                                }
                                else cfg.WindowKeywords = new string[0];

                                configuredNodes.Add(cfg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
                {
                    LogAudit("CONFIG_LOAD_WARN", "Failed to load dispatcher_nodes.json: " + ex.Message + ". Falling back to defaults.");
                }
            }

            // Defensive fallback: populate default 3 comrades
            if (configuredNodes.Count == 0)
            {
                configuredNodes.Add(new TargetNodeConfig {
                    Id = "adjudicator",
                    Name = "裁决者",
                    Aliases = new string[] { "裁决者", "裁决者H" },
                    App = "Antigravity IDE",
                    ProcessNames = new string[] { "antigravity" },
                    WindowKeywords = new string[] { "antigravity ide", "antigravity" },
                    UiaStrategy = "antigravity",
                    ColorHex = "#a6e3a1",
                    DefaultSelected = true
                });
                configuredNodes.Add(new TargetNodeConfig {
                    Id = "nishe",
                    Name = "泥蛇",
                    Aliases = new string[] { "泥蛇", "泥蛇H" },
                    App = "Visual Studio Code",
                    ProcessNames = new string[] { "code", "vscode" },
                    WindowKeywords = new string[] { "visual studio code", "vscode" },
                    UiaStrategy = "vscode",
                    ColorHex = "#89b4fa",
                    DefaultSelected = true
                });
                configuredNodes.Add(new TargetNodeConfig {
                    Id = "peregrine",
                    Name = "游隼",
                    Aliases = new string[] { "游隼", "游隼H" },
                    App = "WorkBuddy",
                    ProcessNames = new string[] { "workbuddy" },
                    WindowKeywords = new string[] { "workbuddy" },
                    UiaStrategy = "workbuddy",
                    ColorHex = "#f9e2af",
                    DefaultSelected = false
                });
            }
        }

        private List<string> ResolveTargetNodes(string sender, string recipient, string title)
        {
            List<string> targets = new List<string>();
            string normRecipient = recipient.Trim();
            string normSender = sender.Trim();

            // Strict exact match for folded host (P1-3)
            if (normRecipient == "折叠主机")
            {
                LogAudit("BRIDGE_ROUTE", "Routing to Folded Host bridge pull chain.");
                return targets;
            }

            if (normRecipient == "代码组全体")
            {
                foreach (var node in configuredNodes)
                {
                    bool isSender = false;
                    if (string.Equals(normSender, node.Name, StringComparison.OrdinalIgnoreCase)) isSender = true;
                    if (node.Aliases != null)
                    {
                        foreach (var alias in node.Aliases)
                        {
                            if (string.Equals(normSender, alias, StringComparison.OrdinalIgnoreCase))
                            {
                                isSender = true;
                                break;
                            }
                        }
                    }
                    if (!isSender)
                    {
                        targets.Add(node.Name);
                    }
                }
            }
            else
            {
                foreach (var node in configuredNodes)
                {
                    bool match = false;
                    if (string.Equals(normRecipient, node.Name, StringComparison.OrdinalIgnoreCase)) match = true;
                    if (node.Aliases != null)
                    {
                        foreach (var alias in node.Aliases)
                        {
                            if (string.Equals(normRecipient, alias, StringComparison.OrdinalIgnoreCase))
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    if (match)
                    {
                        targets.Add(node.Name);
                        break;
                    }
                }
            }

            return targets;
        }

        private static readonly HashSet<string> AllowedAckTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ACK"
        };

        private static readonly HashSet<string> AllowedTerminalTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "DONE",
            "NEEDS_CTO",
            "NEEDS_HUMAN",
            "BLOCKED",
            "CLOSED",
            "CONFIRMED",
            "NEEDS_PATCH",
            "DECIDED"
        };

        private void ParseStatusTokens(string statusLine, out bool isReceipt, out DeliveryStage stage)
        {
            isReceipt = false;
            stage = DeliveryStage.FILE_DISCOVERED;
            if (string.IsNullOrEmpty(statusLine)) return;

            char[] delimiters = new char[] { '/', '|', '\\', ' ', '\t', ',', '，', '、', '(', ')', '（', '）', '[', ']', '【', '】', '·', ':' };
            string[] rawTokens = statusLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            bool hasTerminal = false;
            bool hasAck = false;

            foreach (string raw in rawTokens)
            {
                string token = raw.Trim();
                if (AllowedTerminalTokens.Contains(token))
                {
                    hasTerminal = true;
                }
                else if (AllowedAckTokens.Contains(token))
                {
                    hasAck = true;
                }
            }

            if (hasTerminal)
            {
                isReceipt = true;
                stage = DeliveryStage.TERMINAL_RECEIPT;
            }
            else if (hasAck)
            {
                isReceipt = true;
                stage = DeliveryStage.RECIPIENT_ACK;
            }
        }

        private List<string> ResolveOriginalTaskTargets(string origTaskFile, string receiptSender)
        {
            List<string> targets = new List<string>();
            Regex r = new Regex(@"^(\d{14})·协作·([^·]+)to([^·]+)·(.*)\.(txt|md)$");
            Match m = r.Match(origTaskFile);
            if (m.Success)
            {
                string origSender = m.Groups[2].Value.Trim();
                string origRecipient = m.Groups[3].Value.Trim();
                string origTitle = m.Groups[4].Value.Trim();

                if (origRecipient == "代码组全体")
                {
                    // Broadcast task: advance the specific target matching the receipt sender
                    List<string> responded = ResolveTargetNodes("", receiptSender, "");
                    if (responded.Count > 0)
                    {
                        targets.AddRange(responded);
                    }
                    else
                    {
                        string norm = receiptSender.Replace("H", "").Trim();
                        if (!string.IsNullOrEmpty(norm)) targets.Add(norm);
                    }
                }
                else
                {
                    targets = ResolveTargetNodes(origSender, origRecipient, origTitle);
                }
            }

            if (targets.Count == 0)
            {
                // Fallback to normalized sender of the receipt
                List<string> fallback = ResolveTargetNodes("", receiptSender, "");
                if (fallback.Count > 0) targets.AddRange(fallback);
            }
            return targets;
        }

        private bool DispatchDoorbellToNode(string nodeName, string sourceFile, string title)
        {
            string[] keywords = GetKeywordsForNode(nodeName);
            IntPtr hwnd = FindTargetWindow(keywords);

            if (hwnd == IntPtr.Zero)
            {
                LogAudit("TARGET_NOT_FOUND", string.Format("Node [{0}] window not found on desktops.", nodeName));
                UpdateUIStatus(string.Format("⚠️ 敲门跳过: 未找到 [{0}] 窗口", nodeName));
                return false;
            }

            LogAudit(DeliveryStage.TARGET_FOUND.ToString(), string.Format("Located [{0}] at hWnd 0x{1:X}", nodeName, hwnd.ToInt64()));

            try
            {
                // Set clipboard message on UI thread
                this.Invoke((Action)(() =>
                {
                    try { Clipboard.SetText(DEFAULT_NOTICE); } catch { }
                    lblStatus.Text = string.Format("⚡ 哨兵踹门中: [{0}] <- {1}", nodeName, sourceFile);
                }));

                ActivateAndInject(hwnd, nodeName);

                LogAudit(DeliveryStage.INJECTION_ATTEMPTED.ToString(), string.Format("Injected notice to [{0}] for {1}", nodeName, sourceFile));
                UpdateUIStatus(string.Format("✅ 已尝试敲门: [{0}] (SEND_ATTEMPTED)", nodeName));
                return true;
            }
            catch (Exception ex)
            {
                LogAudit("DELIVERY_FAILED", string.Format("Failed injection to [{0}]: {1}", nodeName, ex.Message));
                return false;
            }
        }

        private string[] GetKeywordsForNode(string nodeName)
        {
            foreach (var node in configuredNodes)
            {
                if (string.Equals(node.Name, nodeName, StringComparison.OrdinalIgnoreCase))
                {
                    return node.WindowKeywords;
                }
                if (node.Aliases != null)
                {
                    foreach (var a in node.Aliases)
                    {
                        if (string.Equals(a, nodeName, StringComparison.OrdinalIgnoreCase))
                            return node.WindowKeywords;
                    }
                }
            }
            return new string[] { nodeName.ToLower() };
        }

        private void MarkKeyProcessed(string recordKey)
        {
            lock (stateLock)
            {
                processedKeys.Add(recordKey);
                try
                {
                    File.AppendAllText(stateFilePath, recordKey + Environment.NewLine, Encoding.UTF8);
                }
                catch { }
            }
        }

        private void LogAudit(string stage, string detail)
        {
            string logLine = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] [{1}] {2}", DateTime.Now, stage, detail);
            try
            {
                string auditLog = Path.Combine(runtimeStateDir, "dispatcher_audit.log");
                File.AppendAllText(auditLog, logLine + Environment.NewLine, Encoding.UTF8);
                if (isAuditDegraded)
                {
                    isAuditDegraded = false;
                    UpdateUIStatus("审计日志写入已恢复");
                }
            }
            catch (Exception ex)
            {
                if (!isAuditDegraded)
                {
                    isAuditDegraded = true;
                    UpdateUIStatus("⚠️ AUDIT_DEGRADED: 审计日志写入异常: " + ex.Message);
                }
            }
        }

        private void UpdateUIStatus(string text)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((Action)(() => { lblStatus.Text = text; }));
            }
        }

        private bool WaitForFileAvailable(string path, int timeoutMs)
        {
            int waited = 0;
            while (waited < timeoutMs)
            {
                try
                {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (fs.Length > 0) return true;
                    }
                }
                catch
                {
                    Thread.Sleep(150);
                    waited += 150;
                }
            }
            return false;
        }

        private string ComputeFileSha256(string path)
        {
            try
            {
                using (SHA256 sha = SHA256.Create())
                using (FileStream stream = File.OpenRead(path))
                {
                    byte[] hash = sha.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash) sb.Append(b.ToString("x2"));
                    return sb.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        // --- Manual GUI & Inject Implementation ---

        private async Task ExecuteManualBroadcast()
        {
            string message = txtNotice.Text.Trim();
            if (string.IsNullOrEmpty(message)) message = DEFAULT_NOTICE;

            btnBroadcast.Enabled = false;
            btnBroadcast.Text = "正在通知...";

            List<TargetNode> selected = new List<TargetNode>();
            foreach (var node in configuredNodes)
            {
                if (partnerCheckBoxes.ContainsKey(node.Name) && partnerCheckBoxes[node.Name].Checked)
                {
                    selected.Add(new TargetNode { Name = node.Name, Keywords = node.WindowKeywords });
                }
            }

            if (selected.Count == 0)
            {
                lblStatus.Text = "⚠️ 未选中任何伙伴";
                btnBroadcast.Enabled = true;
                btnBroadcast.Text = "📢 手动通知";
                return;
            }

            try
            {
                Clipboard.SetText(message);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "剪贴板错误: " + ex.Message;
                btnBroadcast.Enabled = true;
                btnBroadcast.Text = "📢 手动通知";
                return;
            }

            int successCount = 0;

            await Task.Run(() =>
            {
                foreach (var node in selected)
                {
                    this.Invoke((Action)(() => { lblStatus.Text = string.Format("正在定位并激活 [{0}]...", node.Name); }));

                    IntPtr targetHwnd = FindTargetWindow(node.Keywords);

                    if (targetHwnd != IntPtr.Zero)
                    {
                        this.Invoke((Action)(() => { lblStatus.Text = string.Format("已聚焦 [{0}]，正在投递指令...", node.Name); }));

                        ActivateAndInject(targetHwnd, node.Name);
                        successCount++;
                        Thread.Sleep(450);
                    }
                    else
                    {
                        this.Invoke((Action)(() => { lblStatus.Text = string.Format("未找到 [{0}] 窗口，跳过", node.Name); }));
                        Thread.Sleep(300);
                    }
                }
            });

            lblStatus.Text = string.Format("✅ 手动发送完毕！尝试通知 {0}/{1} 个伙伴", successCount, selected.Count);
            btnBroadcast.Enabled = true;
            btnBroadcast.Text = "📢 手动通知";
        }

        private IntPtr FindTargetWindow(string[] keywords)
        {
            IntPtr found = IntPtr.Zero;

            IntPtr hDesktop = OpenDesktop("Default", 0, false, 0x01FF);
            if (hDesktop != IntPtr.Zero)
            {
                EnumDesktopWindows(hDesktop, (hwnd, lparam) =>
                {
                    if (IsWindowVisible(hwnd))
                    {
                        int len = GetWindowTextLength(hwnd);
                        if (len > 0)
                        {
                            StringBuilder sb = new StringBuilder(len + 1);
                            GetWindowText(hwnd, sb, len + 1);
                            string title = sb.ToString().ToLower();

                            foreach (string kw in keywords)
                            {
                                if (title.Contains(kw.ToLower()))
                                {
                                    found = hwnd;
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }, IntPtr.Zero);

                CloseDesktop(hDesktop);
            }

            if (found == IntPtr.Zero)
            {
                EnumWindows((hwnd, lparam) =>
                {
                    if (IsWindowVisible(hwnd))
                    {
                        int len = GetWindowTextLength(hwnd);
                        if (len > 0)
                        {
                            StringBuilder sb = new StringBuilder(len + 1);
                            GetWindowText(hwnd, sb, len + 1);
                            string title = sb.ToString().ToLower();

                            foreach (string kw in keywords)
                            {
                                if (title.Contains(kw.ToLower()))
                                {
                                    found = hwnd;
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            }

            return found;
        }

        private static void SafeForegroundActivate(IntPtr hwnd)
        {
            ShowWindow(hwnd, SW_RESTORE);
            BringWindowToTop(hwnd);

            keybd_event(VK_MENU, 0, 0, UIntPtr.Zero);
            SetForegroundWindow(hwnd);
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            SwitchToThisWindow(hwnd, true);
        }

        private static void ActivateAndInject(IntPtr hwnd, string nodeName)
        {
            try
            {
                SafeForegroundActivate(hwnd);
                Thread.Sleep(250);

                bool focused = false;

                if (nodeName == "游隼")
                {
                    RECT r;
                    if (GetWindowRect(hwnd, out r))
                    {
                        int clickX = r.Left + 420;
                        int clickY = r.Bottom - 105;
                        SetCursorPos(clickX, clickY);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                        focused = true;
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    try
                    {
                        var task = Task.Run(() => FindChatInputElement(hwnd, nodeName));
                        if (task.Wait(400) && task.Result != null)
                        {
                            var targetInput = task.Result;
                            try { targetInput.SetFocus(); focused = true; } catch { }

                            System.Windows.Point pt;
                            if (targetInput.TryGetClickablePoint(out pt))
                            {
                                SetCursorPos((int)pt.X, (int)pt.Y);
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                                focused = true;
                            }
                            Thread.Sleep(120);
                        }
                    }
                    catch { }

                    if (!focused)
                    {
                        RECT r;
                        if (GetWindowRect(hwnd, out r))
                        {
                            int clickX = r.Left + (r.Right - r.Left) / 2;
                            int clickY = r.Bottom - 45;
                            SetCursorPos(clickX, clickY);
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                            Thread.Sleep(150);
                        }
                    }
                }

                // Simulate Ctrl+V
                keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
                keybd_event(VK_V, 0, 0, UIntPtr.Zero);
                keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                Thread.Sleep(100);

                // Simulate Enter
                keybd_event(VK_RETURN, 0, 0, UIntPtr.Zero);
                keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
            catch { }
        }

        private static AutomationElement FindChatInputElement(IntPtr hwnd, string nodeName)
        {
            try
            {
                AutomationElement root = AutomationElement.FromHandle(hwnd);
                if (root == null) return null;

                if (nodeName == "裁决者")
                {
                    var condMsg = new PropertyCondition(AutomationElement.NameProperty, "Message input");
                    var el = root.FindFirst(TreeScope.Descendants, condMsg);
                    if (el != null && el.Current.IsKeyboardFocusable) return el;

                    var condCombo = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox);
                    var combos = root.FindAll(TreeScope.Descendants, condCombo);
                    foreach (AutomationElement c in combos)
                    {
                        if (c.Current.IsKeyboardFocusable) return c;
                    }
                }

                if (nodeName == "泥蛇")
                {
                    var condEdit = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit);
                    var edits = root.FindAll(TreeScope.Descendants, condEdit);
                    foreach (AutomationElement ed in edits)
                    {
                        string cls = (ed.Current.ClassName ?? "");
                        string name = (ed.Current.Name ?? "");
                        if (cls.Contains("ProseMirror") || name.Contains("变更") || name.Contains("要求") || name.Contains("Chat"))
                        {
                            if (ed.Current.IsKeyboardFocusable) return ed;
                        }
                    }

                    for (int i = edits.Count - 1; i >= 0; i--)
                    {
                        var ed = edits[i];
                        string cls = (ed.Current.ClassName ?? "");
                        string name = (ed.Current.Name ?? "");
                        if (!cls.Contains("monaco") && !name.EndsWith(".py") && !name.EndsWith(".md"))
                        {
                            if (ed.Current.IsKeyboardFocusable) return ed;
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (meetingWatcher != null)
                {
                    meetingWatcher.EnableRaisingEvents = false;
                    meetingWatcher.Dispose();
                }
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        
        // ====================================================================
        // Lightweight HTTP API Server (Dynamic Port Support)
        // ====================================================================

        private void StartHttpServer()
        {
            if (instanceOpts == null || instanceOpts.Port <= 0) return;
            try
            {
                httpListener = new HttpListener();
                string prefix = string.Format("http://127.0.0.1:{0}/", instanceOpts.Port);
                httpListener.Prefixes.Add(prefix);
                httpListener.Start();
                isHttpRunning = true;

                httpListenerThread = new Thread(HttpServerWorker);
                httpListenerThread.IsBackground = true;
                httpListenerThread.Start();
                LogAudit("HTTP_SERVER_STARTED", string.Format("Listening on {0} [Instance:{1}]", prefix, instanceOpts.InstanceId));
            }
            catch (Exception ex)
            {
                LogAudit("HTTP_SERVER_ERROR", string.Format("Port {0} failed: {1}", instanceOpts.Port, ex.Message));
            }
        }

        private void StopHttpServer()
        {
            isHttpRunning = false;
            try
            {
                if (httpListener != null && httpListener.IsListening)
                {
                    httpListener.Stop();
                    httpListener.Close();
                }
            }
            catch { }
        }

        private void HttpServerWorker()
        {
            while (isHttpRunning && httpListener != null && httpListener.IsListening)
            {
                try
                {
                    HttpListenerContext ctx = httpListener.GetContext();
                    ThreadPool.QueueUserWorkItem((state) => { HandleHttpRequest(ctx); });
                }
                catch (HttpListenerException) { break; }
                catch (Exception) { }
            }
        }

        private void HandleHttpRequest(HttpListenerContext ctx)
        {
            try
            {
                string rawUrl = ctx.Request.RawUrl ?? "";
                string path = rawUrl.Split('?')[0].TrimEnd('/').ToLower();
                string method = ctx.Request.HttpMethod.ToUpper();

                ctx.Response.ContentType = "application/json; charset=utf-8";
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                if (method == "OPTIONS")
                {
                    ctx.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Close();
                    return;
                }

                JavaScriptSerializer ser = new JavaScriptSerializer();
                if ((path == "/api/status" || path == "/api/health") && method == "GET")
                {
                    var statusObj = new Dictionary<string, object>
                    {
                        { "status", "OK" },
                        { "instanceId", instanceOpts.InstanceId },
                        { "isTestInstance", instanceOpts.IsTestInstance },
                        { "port", instanceOpts.Port },
                        { "dryRun", instanceOpts.DryRun },
                        { "cto", ctoName },
                        { "mission", currentMissionTitle },
                        { "warRoomRoot", warRoomRootDir },
                        { "meetingDir", meetingDir },
                        { "runtimeDir", runtimeStateDir },
                        { "watching", isWatchingActive },
                        { "pendingToHuman", pendingToHumanCount },
                        { "checklistCount", currentChecklistItems != null ? currentChecklistItems.Count : 0 },
                        { "meetingsCount", currentMeetingItems != null ? currentMeetingItems.Count : 0 }
                    };
                    byte[] bytes = Encoding.UTF8.GetBytes(ser.Serialize(statusObj));
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                else if (path == "/api/checklist" && method == "GET")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(ser.Serialize(currentChecklistItems));
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                else if (path == "/api/meetings" && method == "GET")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(ser.Serialize(currentMeetingItems));
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                else if ((path == "/api/knock" || path == "/api/broadcast") && method == "POST")
                {
                    string body = "";
                    using (StreamReader reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8))
                    {
                        body = reader.ReadToEnd();
                    }
                    Dictionary<string, object> req = ser.Deserialize<Dictionary<string, object>>(body) ?? new Dictionary<string, object>();
                    string node = req.ContainsKey("node") ? req["node"].ToString() : "";
                    string msg = req.ContainsKey("message") ? req["message"].ToString() : "";
                    string taskId = req.ContainsKey("taskId") ? req["taskId"].ToString() : "HTTP_DIRECT_KNOCK";

                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (!string.IsNullOrEmpty(node))
                            {
                                DispatchDoorbellToNode(node, taskId, msg);
                            }
                        }));
                    }

                    var resp = new Dictionary<string, object>
                    {
                        { "success", true },
                        { "instance", instanceOpts.InstanceId },
                        { "node", node },
                        { "taskId", taskId }
                    };
                    byte[] bytes = Encoding.UTF8.GetBytes(ser.Serialize(resp));
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                else if (path == "/api/shutdown" && method == "POST")
                {
                    var resp = new Dictionary<string, object>
                    {
                        { "message", "Shutting down instance " + instanceOpts.InstanceId },
                        { "port", instanceOpts.Port }
                    };
                    byte[] bytes = Encoding.UTF8.GetBytes(ser.Serialize(resp));
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    ctx.Response.Close();

                    ThreadPool.QueueUserWorkItem((s) =>
                    {
                        Thread.Sleep(200);
                        Environment.Exit(0);
                    });
                    return;
                }
                else
                {
                    ctx.Response.StatusCode = 404;
                    byte[] bytes = Encoding.UTF8.GetBytes("{\"error\": \"Not Found\"}");
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch { }
            finally
            {
                try { ctx.Response.Close(); } catch { }
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            InstanceOptions opts = ParseCommandLineArgs(args);

            // 动态命名互斥量：正式实例与测试实例拥有各自独立的 Mutex，互不抢占！
            string mutexName = string.Format("Local\\CodeAiDispatcher_SingleInstance_{0}_{1}", opts.InstanceId.ToLower(), opts.Port);
            bool createdNew;
            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    // 已有同端口实例运行，避免端口冲突与重复实例
                    return;
                }

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new DispatcherForm(opts));
                }
                catch (Exception ex)
                {
                    try
                    {
                        string crashLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("dispatcher_crash_{0}_{1}.log", opts.InstanceId, opts.Port));
                        File.WriteAllText(crashLog, ex.ToString());
                    }
                    catch { }
                }
            }
        }

        public static InstanceOptions ParseCommandLineArgs(string[] args)
        {
            InstanceOptions opts = new InstanceOptions();

            // 1. 读取配置文件 dispatcher_config.json (若存在)
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string cfgFile = Path.Combine(baseDir, "dispatcher_config.json");
                if (File.Exists(cfgFile))
                {
                    string json = File.ReadAllText(cfgFile, Encoding.UTF8);
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    InstanceOptions loaded = ser.Deserialize<InstanceOptions>(json);
                    if (loaded != null)
                    {
                        if (!string.IsNullOrEmpty(loaded.InstanceId)) opts.InstanceId = loaded.InstanceId;
                        if (loaded.Port > 0) opts.Port = loaded.Port;
                        if (!string.IsNullOrEmpty(loaded.WarRoomRoot)) opts.WarRoomRoot = loaded.WarRoomRoot;
                        if (!string.IsNullOrEmpty(loaded.MeetingDir)) opts.MeetingDir = loaded.MeetingDir;
                        if (!string.IsNullOrEmpty(loaded.RuntimeDir)) opts.RuntimeDir = loaded.RuntimeDir;
                        if (!string.IsNullOrEmpty(loaded.NodeConfigFile)) opts.NodeConfigFile = loaded.NodeConfigFile;
                        opts.DryRun = loaded.DryRun;
                    }
                }
            }
            catch { }

            // 2. 命令行参数优先覆盖
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i].Trim();
                    string lower = arg.ToLower();

                    if (lower == "--port" && i + 1 < args.Length)
                    {
                        int p;
                        if (int.TryParse(args[++i], out p)) opts.Port = p;
                    }
                    else if (lower.StartsWith("--port="))
                    {
                        int p;
                        if (int.TryParse(arg.Substring(7), out p)) opts.Port = p;
                    }
                    else if ((lower == "--instance" || lower == "--env") && i + 1 < args.Length)
                    {
                        opts.InstanceId = args[++i].Trim();
                    }
                    else if (lower.StartsWith("--instance=") || lower.StartsWith("--env="))
                    {
                        opts.InstanceId = arg.Substring(arg.IndexOf('=') + 1).Trim();
                    }
                    else if (lower == "--war-room" && i + 1 < args.Length)
                    {
                        opts.WarRoomRoot = args[++i].Trim();
                    }
                    else if (lower == "--meeting-dir" && i + 1 < args.Length)
                    {
                        opts.MeetingDir = args[++i].Trim();
                    }
                    else if (lower == "--runtime-dir" && i + 1 < args.Length)
                    {
                        opts.RuntimeDir = args[++i].Trim();
                    }
                    else if (lower == "--nodes" && i + 1 < args.Length)
                    {
                        opts.NodeConfigFile = args[++i].Trim();
                    }
                    else if (lower == "--dry-run" || lower == "--test")
                    {
                        opts.DryRun = true;
                        if (opts.InstanceId == "prod") opts.InstanceId = "test";
                        if (opts.Port == 8787) opts.Port = 8788;
                    }
                }
            }

            if (opts.InstanceId.ToLower() == "test" && opts.Port == 8787)
            {
                opts.Port = 8788;
            }

            return opts;
        }

        // ====================================================================
        // P0: TO_HUMAN Notification & Double-Knock Sound Implementation
        // ====================================================================

        public static byte[] GenerateDoubleKnockWav()
        {
            int sampleRate = 22050;
            double duration = 0.32;
            int totalSamples = (int)(sampleRate * duration);
            short[] samples = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / sampleRate;
                double val = 0.0;

                // Knock 1: 0.01s to 0.10s (基频 190Hz -> 140Hz 伴随木质谐波与快速指数衰减)
                if (t >= 0.01 && t < 0.10)
                {
                    double kt = t - 0.01;
                    double env = Math.Exp(-42.0 * kt);
                    double freq = 190.0 - (50.0 * kt / 0.09);
                    val += 0.85 * env * (0.75 * Math.Sin(2.0 * Math.PI * freq * kt) + 0.25 * Math.Sin(2.0 * Math.PI * freq * 1.8 * kt));
                }
                // Knock 2: 0.14s to 0.23s (基频 220Hz -> 170Hz 伴随木质谐波与快速指数衰减)
                if (t >= 0.14 && t < 0.23)
                {
                    double kt = t - 0.14;
                    double env = Math.Exp(-40.0 * kt);
                    double freq = 220.0 - (50.0 * kt / 0.09);
                    val += 0.95 * env * (0.75 * Math.Sin(2.0 * Math.PI * freq * kt) + 0.25 * Math.Sin(2.0 * Math.PI * freq * 1.8 * kt));
                }

                if (val > 1.0) val = 1.0;
                if (val < -1.0) val = -1.0;
                samples[i] = (short)(val * 28000);
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                int dataSize = totalSamples * sizeof(short);
                // RIFF chunk descriptor
                bw.Write(Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(36 + dataSize);
                bw.Write(Encoding.ASCII.GetBytes("WAVE"));
                // "fmt " sub-chunk
                bw.Write(Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16); // subchunk size
                bw.Write((short)1); // AudioFormat: PCM (1)
                bw.Write((short)1); // NumChannels: Mono (1)
                bw.Write(sampleRate); // SampleRate
                bw.Write(sampleRate * sizeof(short)); // ByteRate
                bw.Write((short)sizeof(short)); // BlockAlign
                bw.Write((short)16); // BitsPerSample
                // "data" sub-chunk
                bw.Write(Encoding.ASCII.GetBytes("data"));
                bw.Write(dataSize);
                for (int i = 0; i < samples.Length; i++)
                {
                    bw.Write(samples[i]);
                }
                return ms.ToArray();
            }
        }

        private void PlayDoubleKnockSound()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!soundEnabled) return;
                    if (!string.IsNullOrEmpty(soundFile) && File.Exists(soundFile))
                    {
                        using (SoundPlayer player = new SoundPlayer(soundFile))
                        {
                            player.PlaySync();
                        }
                        return;
                    }

                    if (cachedKnockWav == null)
                    {
                        cachedKnockWav = GenerateDoubleKnockWav();
                    }

                    using (MemoryStream ms = new MemoryStream(cachedKnockWav))
                    using (SoundPlayer player = new SoundPlayer(ms))
                    {
                        player.PlaySync();
                    }
                }
                catch (Exception ex)
                {
                    LogAudit("SOUND_PLAY_ERROR", ex.Message);
                }
            });
        }

        private ToHumanEvent DetectStructuredToHuman(string filePath, string fileName, string senderName, string recipientName, string title, string sha256)
        {
            string normRecipient = (recipientName ?? "").Trim().ToLower();
            bool recipientIsHuman = normRecipient == "human" ||
                                    normRecipient == "人类" ||
                                    normRecipient == "指挥官" ||
                                    normRecipient == "commander" ||
                                    normRecipient == "to_human" ||
                                    normRecipient == "tohuman";

            bool filenameHasToHumanTag = fileName.IndexOf("·TO_HUMAN·", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                         fileName.IndexOf(".TO_HUMAN.", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                         fileName.IndexOf("_TO_HUMAN_", StringComparison.OrdinalIgnoreCase) >= 0;

            bool isStructuredToHuman = recipientIsHuman || filenameHasToHumanTag;
            string extractedReason = null;
            string extractedSummary = null;
            string extractedTaskId = fileName;

            if (File.Exists(filePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    int maxScan = Math.Min(lines.Length, 60);
                    List<string> bodyLines = new List<string>();

                    for (int i = 0; i < maxScan; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        Match mHumanRecipient = Regex.Match(line, @"^(?:收件人|Recipient|To|Target|通知对象)[：:]\s*(Human|人类|指挥官|Commander|TO_HUMAN)\b", RegexOptions.IgnoreCase);
                        if (mHumanRecipient.Success)
                        {
                            isStructuredToHuman = true;
                        }

                        Match mToHumanFlag = Regex.Match(line, @"^TO_HUMAN[：:]\s*(.+)$", RegexOptions.IgnoreCase);
                        if (mToHumanFlag.Success)
                        {
                            string val = mToHumanFlag.Groups[1].Value.Trim().ToLower();
                            if (val != "false" && val != "0" && val != "no")
                            {
                                isStructuredToHuman = true;
                                if (val != "true" && val != "1" && val != "yes")
                                {
                                    extractedReason = mToHumanFlag.Groups[1].Value.Trim();
                                }
                            }
                        }

                        Match mAction = Regex.Match(line, @"^(?:动作|Action)[：:]\s*(.+)$", RegexOptions.IgnoreCase);
                        if (mAction.Success)
                        {
                            char[] delims = new char[] { '/', '|', '\\', ' ', '\t', ',', '，', '、', '(', ')', '（', '）', '[', ']', '【', '】', '·', ':' };
                            string[] tokens = mAction.Groups[1].Value.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var tok in tokens)
                            {
                                string tUpper = tok.Trim().ToUpper();
                                if (tUpper == "HUMAN_CONFIRM" || tUpper == "HUMAN_AUTH" || tUpper == "HUMAN_DECIDE" || tUpper == "HUMAN_ACCEPT" || tUpper == "HUMAN_REVIEW" || tUpper == "TO_HUMAN")
                                {
                                    isStructuredToHuman = true;
                                    if (string.IsNullOrEmpty(extractedReason))
                                    {
                                        extractedReason = "动作要求: " + tUpper;
                                    }
                                    break;
                                }
                            }
                        }

                        Match mStatus = Regex.Match(line, @"^(?:状态|Status)[：:]\s*(.+)$", RegexOptions.IgnoreCase);
                        if (mStatus.Success)
                        {
                            char[] delims = new char[] { '/', '|', '\\', ' ', '\t', ',', '，', '、', '(', ')', '（', '）', '[', ']', '【', '】', '·', ':' };
                            string[] tokens = mStatus.Groups[1].Value.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var tok in tokens)
                            {
                                string tUpper = tok.Trim().ToUpper();
                                if (tUpper == "AWAITING_HUMAN" || tUpper == "NEED_HUMAN" || tUpper == "NEEDS_HUMAN" || tUpper == "HUMAN_REVIEW" || tUpper == "TO_HUMAN")
                                {
                                    isStructuredToHuman = true;
                                    if (string.IsNullOrEmpty(extractedReason))
                                    {
                                        extractedReason = "状态处于: " + tUpper;
                                    }
                                    break;
                                }
                            }
                        }

                        Match mType = Regex.Match(line, @"^(?:类型|Type|事件|Event)[：:]\s*(HUMAN_INTERVENTION|TO_HUMAN)\b", RegexOptions.IgnoreCase);
                        if (mType.Success)
                        {
                            isStructuredToHuman = true;
                        }

                        Match mReason = Regex.Match(line, @"^(?:原因|Reason|事由|说明)[：:]\s*(.+)$", RegexOptions.IgnoreCase);
                        if (mReason.Success)
                        {
                            extractedReason = mReason.Groups[1].Value.Trim();
                        }

                        Match mTask = Regex.Match(line, @"^(?:任务|Task|Task_ID|任务编号|关联任务)[：:]\s*(.+)$", RegexOptions.IgnoreCase);
                        if (mTask.Success)
                        {
                            extractedTaskId = mTask.Groups[1].Value.Trim();
                        }

                        // Filter out headers, headings, metadata for candidate body summary
                        if (!line.StartsWith("#") && !line.StartsWith("---") && !line.Contains("：") && !line.Contains(":"))
                        {
                            bodyLines.Add(line);
                        }
                    }

                    if (bodyLines.Count > 0)
                    {
                        extractedSummary = bodyLines[0];
                        if (extractedSummary.Length > 140)
                            extractedSummary = extractedSummary.Substring(0, 140) + "...";
                    }
                }
                catch { }
            }

            if (!isStructuredToHuman)
            {
                return null;
            }

            if (string.IsNullOrEmpty(extractedReason))
            {
                extractedReason = recipientIsHuman ? "协作消息明确发给人类，需人工查看与处理" : "协作流程标记为需要人类介入";
            }

            if (string.IsNullOrEmpty(extractedSummary))
            {
                extractedSummary = string.Format("来自 [{0}] 的协作文件: {1}", senderName, title);
            }

            return new ToHumanEvent
            {
                Sender = senderName,
                Recipient = recipientName,
                TaskId = string.IsNullOrEmpty(extractedTaskId) ? fileName : extractedTaskId,
                TaskTitle = title,
                Reason = extractedReason,
                Summary = extractedSummary,
                FilePath = filePath,
                Sha256 = sha256,
                DedupKey = string.Format("TO_HUMAN|{0}|{1}", fileName, sha256)
            };
        }

        private void ShowHumanNotification(ToHumanEvent evt)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        HumanNotificationForm card = new HumanNotificationForm(evt, (stage, detail) => LogAudit(stage, detail));
                        card.Show();
                    }
                    catch (Exception ex)
                    {
                        LogAudit("NOTIFICATION_UI_ERROR", ex.Message);
                    }
                }));
            }
        }

        private class TargetNode
        {
            public string Name { get; set; }
            public string[] Keywords { get; set; }
        }
    }

    public class ToHumanEvent
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string Reason { get; set; }
        public string Summary { get; set; }
        public string FilePath { get; set; }
        public string Sha256 { get; set; }
        public string DedupKey { get; set; }
    }

    public class HumanNotificationForm : Form
    {
        private ToHumanEvent evt;
        private Action<string, string> logAudit;
        private static readonly List<HumanNotificationForm> activeCards = new List<HumanNotificationForm>();
        private bool isMouseDown = false;
        private Point mouseOffset;

        public HumanNotificationForm(ToHumanEvent evt, Action<string, string> logAudit)
        {
            this.evt = evt;
            this.logAudit = logAudit;
            InitializeCardComponent();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                return cp;
            }
        }

        private void InitializeCardComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(420, 230);
            this.BackColor = Color.FromArgb(24, 25, 32);
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;

            // Calculate position in working area
            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            int offsetIndex;
            lock (activeCards)
            {
                offsetIndex = activeCards.Count;
                activeCards.Add(this);
            }
            int posX = wa.Right - this.Width - 20;
            int posY = wa.Bottom - this.Height - 20 - (offsetIndex * (this.Height + 12));
            if (posY < wa.Top + 10) posY = wa.Top + 10;
            this.Location = new Point(posX, posY);

            // Header Panel
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(30, 32, 42)
            };
            pnlHeader.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isMouseDown = true;
                    mouseOffset = new Point(-e.X, -e.Y);
                }
            };
            pnlHeader.MouseMove += (s, e) =>
            {
                if (isMouseDown)
                {
                    Point mousePos = Control.MousePosition;
                    mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                    this.Location = mousePos;
                }
            };
            pnlHeader.MouseUp += (s, e) => { if (e.Button == MouseButtons.Left) isMouseDown = false; };

            Label lblTitle = new Label
            {
                Text = "🔔 AI Team needs you",
                ForeColor = Color.FromArgb(250, 179, 135), // Warm Peach
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(12, 8),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlHeader.Controls.Add(lblTitle);

            Button btnClose = new Button
            {
                Text = "✕",
                ForeColor = Color.FromArgb(166, 173, 200),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 28),
                Location = new Point(384, 4),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 130, 132);
            btnClose.Click += (s, e) =>
            {
                if (logAudit != null) logAudit("TO_HUMAN_DISMISSED", evt.TaskId);
                this.Close();
            };
            pnlHeader.Controls.Add(btnClose);
            this.Controls.Add(pnlHeader);

            // Content elements
            Label lblFrom = new Label
            {
                Text = string.Format("From: {0}", evt.Sender ?? "Unknown"),
                ForeColor = Color.FromArgb(148, 226, 213), // Mint / Cyan
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(14, 44),
                Size = new Size(180, 18),
                AutoEllipsis = true
            };
            this.Controls.Add(lblFrom);

            Label lblTask = new Label
            {
                Text = string.Format("Task: {0}", evt.TaskTitle ?? evt.TaskId ?? "Collaboration Task"),
                ForeColor = Color.FromArgb(205, 214, 244),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(14, 64),
                Size = new Size(392, 18),
                AutoEllipsis = true
            };
            this.Controls.Add(lblTask);

            Label lblReason = new Label
            {
                Text = string.Format("Reason: {0}", evt.Reason ?? "需要人类介入"),
                ForeColor = Color.FromArgb(249, 226, 175), // Soft Gold
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(14, 84),
                Size = new Size(392, 20),
                AutoEllipsis = true
            };
            this.Controls.Add(lblReason);

            // Summary Box
            Panel pnlSummary = new Panel
            {
                Location = new Point(14, 108),
                Size = new Size(392, 68),
                BackColor = Color.FromArgb(17, 17, 27),
                BorderStyle = BorderStyle.None
            };
            Label lblSummaryText = new Label
            {
                Text = evt.Summary ?? "",
                ForeColor = Color.FromArgb(186, 194, 222),
                Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular),
                Dock = DockStyle.Fill,
                Padding = new Padding(6),
                AutoEllipsis = true
            };
            pnlSummary.Controls.Add(lblSummaryText);
            this.Controls.Add(pnlSummary);

            // Bottom Buttons
            Button btnView = new Button
            {
                Text = "📄 查看消息",
                Size = new Size(110, 32),
                Location = new Point(14, 186),
                BackColor = Color.FromArgb(137, 180, 250), // Accent Blue
                ForeColor = Color.FromArgb(17, 17, 27),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnView.FlatAppearance.BorderSize = 0;
            btnView.Click += (s, e) =>
            {
                if (logAudit != null) logAudit("TO_HUMAN_VIEWED", evt.TaskId);
                try
                {
                    if (!string.IsNullOrEmpty(evt.FilePath) && File.Exists(evt.FilePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(evt.FilePath)
                        {
                            UseShellExecute = true
                        });
                    }
                    else if (!string.IsNullOrEmpty(evt.FilePath) && Directory.Exists(Path.GetDirectoryName(evt.FilePath)))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(evt.FilePath));
                    }
                }
                catch (Exception ex)
                {
                    if (logAudit != null) logAudit("TO_HUMAN_VIEW_ERROR", ex.Message);
                }
                this.Close();
            };
            this.Controls.Add(btnView);

            Button btnAck = new Button
            {
                Text = "知道了",
                Size = new Size(85, 32),
                Location = new Point(134, 186),
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.FromArgb(205, 214, 244),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnAck.FlatAppearance.BorderSize = 0;
            btnAck.Click += (s, e) =>
            {
                if (logAudit != null) logAudit("TO_HUMAN_DISMISSED", evt.TaskId);
                this.Close();
            };
            this.Controls.Add(btnAck);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw sleek 1px border
            using (Pen borderPen = new Pen(Color.FromArgb(69, 71, 90), 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }
            // Top 3px accent bar (Peach to Coral)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Point(0, 0), new Point(this.Width, 0),
                Color.FromArgb(250, 179, 135), Color.FromArgb(243, 139, 168)))
            {
                e.Graphics.FillRectangle(brush, 0, 0, this.Width, 3);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            lock (activeCards)
            {
                activeCards.Remove(this);
            }
        }
    }
}
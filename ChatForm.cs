using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using CybersecurityChatbot.Core;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.GUI
{
    /// <summary>
    /// Main GUI form for the Cybersecurity Awareness Chatbot (Part 3 POE).
    /// Integrates: Task Assistant, Quiz, NLP, Activity Log, SQLite database.
    /// </summary>
    public class ChatForm : Form
    {
        // UI Controls
        private RichTextBox _chatDisplay = null!;
        private TextBox _inputBox = null!;
        private Button _sendButton = null!;
        private Button _exitButton = null!;
        private Button _tipsButton = null!;
        private Button _quizButton = null!;
        private Button _topicsButton = null!;
        private Button _historyButton = null!;
        private Button _clearButton = null!;
        private Button _tasksButton = null!;
        private Button _logButton = null!;
        private Label _statusLabel = null!;
        private Label _userLabel = null!;
        private Panel _headerPanel = null!;
        private Panel _inputPanel = null!;
        private Panel _sidePanel = null!;

        // Engine
        private readonly GUIChatEngine _engine;

        // Colour Palette 
        private readonly Dictionary<string, Color> _colours = new Dictionary<string, Color>
        {
            { "bgDark",    Color.FromArgb(10,  14,  20)  },
            { "bgMid",     Color.FromArgb(16,  22,  32)  },
            { "bgPanel",   Color.FromArgb(20,  28,  42)  },
            { "accent",    Color.FromArgb(0,   220, 180) },
            { "accentAlt", Color.FromArgb(255, 180, 0)   },
            { "bot",       Color.FromArgb(0,   220, 180) },
            { "user",      Color.FromArgb(255, 200, 80)  },
            { "system",    Color.FromArgb(120, 140, 160) },
            { "warn",      Color.FromArgb(255, 80,  80)  },
            { "success",   Color.FromArgb(80,  220, 100) },
            { "exit",      Color.FromArgb(180, 40,  40)  },
            { "tasks",     Color.FromArgb(80,  120, 220) },
            { "log",       Color.FromArgb(180, 100, 220) },
        };

        // Constructor
        public ChatForm()
        {
            // Initialise SQLite database on startup
            DatabaseHelper.Initialise();

            _engine = new GUIChatEngine(AppendBotMessage, AppendSystemMessage);
            InitializeComponent();
            PlayVoiceGreeting();
            ShowWelcome();
        }

        // UI INITIALISATION
        private void InitializeComponent()
        {
            Text = "CyberBot — Cybersecurity Awareness Chatbot";
            Size = new Size(1140, 740);
            MinimumSize = new Size(900, 600);
            BackColor = _colours["bgDark"];
            ForeColor = Color.White;
            Font = new Font("Consolas", 9.5f);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;

            FormClosing += OnFormClosing;

            BuildHeader();
            BuildSidePanel();
            BuildInputArea();
            BuildChatArea();
            WireEvents();
        }

        // Header
        private void BuildHeader()
        {
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = _colours["bgPanel"]
            };

            var titleLabel = new Label
            {
                Text = "  CYBERBOT — CYBERSECURITY AWARENESS CHATBOT  ",
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(800, 22),
                Location = new Point(10, 8),
                BackColor = Color.Transparent
            };

            _userLabel = new Label
            {
                Text = " CONNECTING...",
                ForeColor = _colours["system"],
                Font = new Font("Consolas", 8.5f),
                AutoSize = false,
                Size = new Size(800, 22),
                Location = new Point(14, 34),
                BackColor = Color.Transparent
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(_userLabel);
            _headerPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(_colours["accent"], 1f);
                e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1,
                                    _headerPanel.Width, _headerPanel.Height - 1);
            };

            Controls.Add(_headerPanel);
        }

        // Side Panel 
        private void BuildSidePanel()
        {
            _sidePanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 195,
                BackColor = _colours["bgPanel"]
            };

            _sidePanel.Paint += (s, e) =>
            {
                using var pen = new Pen(_colours["accent"], 1f);
                e.Graphics.DrawLine(pen, 0, 0, 0, _sidePanel.Height);
            };

            // Section: Commands
            _sidePanel.Controls.Add(MakeSectionLabel("[ COMMANDS ]", 10));

            // Dictionary: button text > (y position, colour key)
            var sideButtonDefs = new Dictionary<string, (int y, string colourKey)>
            {
                { " Random Tip",    (34,  "accent") },
                { " Start Quiz",    (76,  "accent") },
                { " View Topics",   (118, "accent") },
                { " History",       (160, "accent") },
                { " Clear History", (202, "accent") },
                { " Task Manager",  (244, "tasks")  },
                { " Activity Log",  (286, "log")    },
            };

            var buttonRefs = new Dictionary<string, Button>();
            foreach (var def in sideButtonDefs)
            {
                var btn = MakeSideButton(def.Key, def.Value.y, def.Value.colourKey);
                buttonRefs[def.Key] = btn;
                _sidePanel.Controls.Add(btn);
            }

            _tipsButton = buttonRefs[" Random Tip"];
            _quizButton = buttonRefs[" Start Quiz"];
            _topicsButton = buttonRefs[" View Topics"];
            _historyButton = buttonRefs[" History"];
            _clearButton = buttonRefs[" Clear History"];
            _tasksButton = buttonRefs[" Task Manager"];
            _logButton = buttonRefs[" Activity Log"];

            // Section: Quick Tips
            _sidePanel.Controls.Add(MakeSectionLabel("[ QUICK TIPS ]", 334));

            var quickTips = new Dictionary<int, string>
            {
                { 358, "• Use unique passwords"   },
                { 388, "• Enable 2FA everywhere"  },
                { 418, "• Think before you click" },
                { 448, "• Keep software updated"  },
                { 478, "• VPN on public WiFi"     },
            };

            foreach (var tip in quickTips)
            {
                _sidePanel.Controls.Add(new Label
                {
                    Text = tip.Value,
                    ForeColor = _colours["system"],
                    Font = new Font("Consolas", 7.5f),
                    AutoSize = false,
                    Size = new Size(175, 26),
                    Location = new Point(10, tip.Key),
                    BackColor = Color.Transparent
                });
            }

            Controls.Add(_sidePanel);
        }

        private Label MakeSectionLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(175, 20),
                Location = new Point(10, y),
                BackColor = Color.Transparent
            };
        }

        private Button MakeSideButton(string text, int y, string colourKey)
        {
            var fg = (colourKey == "tasks" || colourKey == "log")
                ? Color.White
                : _colours["accent"];

            return new Button
            {
                Text = text,
                Location = new Point(10, y),
                Size = new Size(175, 34),
                BackColor = _colours["bgMid"],
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f),
                Cursor = Cursors.Hand,
                FlatAppearance =
                {
                    BorderColor = colourKey == "tasks" ? _colours["tasks"]
                                : colourKey == "log"   ? _colours["log"]
                                : _colours["accent"],
                    BorderSize = 1
                }
            };
        }

        // Chat Area
        private void BuildChatArea()
        {
            _chatDisplay = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = _colours["bgDark"],
                ForeColor = Color.White,
                Font = new Font("Lucida Console", 8.5f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Padding = new Padding(10),
                WordWrap = true
            };

            Controls.Add(_chatDisplay);
            _chatDisplay.BringToFront();
        }

        // Input Area 
        private void BuildInputArea()
        {
            _inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = _colours["bgPanel"]
            };

            _inputPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(_colours["accent"], 1f);
                e.Graphics.DrawLine(pen, 0, 0, _inputPanel.Width, 0);
            };

            _inputBox = new TextBox
            {
                Location = new Point(10, 14),
                Size = new Size(700, 32),
                BackColor = _colours["bgMid"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 10.5f),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _sendButton = new Button
            {
                Text = "SEND ",
                Location = new Point(718, 14),
                Size = new Size(100, 32),
                BackColor = _colours["accent"],
                ForeColor = _colours["bgDark"],
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                FlatAppearance = { BorderSize = 0 }
            };

            _exitButton = new Button
            {
                Text = "EXIT ",
                Location = new Point(826, 14),
                Size = new Size(100, 32),
                BackColor = _colours["exit"],
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                FlatAppearance = { BorderSize = 0 }
            };

            _statusLabel = new Label
            {
                Text = "Ready",
                ForeColor = _colours["system"],
                Font = new Font("Consolas", 7.5f),
                AutoSize = false,
                Size = new Size(400, 14),
                Location = new Point(10, 50),
                BackColor = Color.Transparent
            };

            _inputPanel.Controls.AddRange(new Control[]
            {
                _inputBox, _sendButton, _exitButton, _statusLabel
            });

            Controls.Add(_inputPanel);

            _inputPanel.Resize += (s, e) =>
            {
                _inputBox.Width = _inputPanel.Width - 230;
                _sendButton.Location = new Point(_inputPanel.Width - 215, 14);
                _exitButton.Location = new Point(_inputPanel.Width - 107, 14);
            };
        }

        // Wire Events 
        private void WireEvents()
        {
            _sendButton.Click += OnSend;
            _inputBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    OnSend(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            _exitButton.Click += OnExitClicked;

            // Dictionary: button > command string
            var buttonCommands = new Dictionary<Button, string>
            {
                { _tipsButton,    "tip"     },
                { _topicsButton,  "topics"  },
                { _historyButton, "history" },
                { _clearButton,   "clear"   },
            };

            foreach (var pair in buttonCommands)
            {
                string cmd = pair.Value;
                pair.Key.Click += (s, e) => ProcessCommand(cmd);
            }

            _quizButton.Click += (s, e) => LaunchQuiz();
            _tasksButton.Click += (s, e) => LaunchTasks();
            _logButton.Click += (s, e) => ShowActivityLog();

            // Hover effects for all side buttons
            var allSideButtons = new List<Button>
            {
                _tipsButton, _quizButton, _topicsButton,
                _historyButton, _clearButton, _tasksButton, _logButton
            };

            foreach (var btn in allSideButtons)
            {
                btn.MouseEnter += (s, e) => ((Button)s!).BackColor = Color.FromArgb(0, 60, 55);
                btn.MouseLeave += (s, e) => ((Button)s!).BackColor = _colours["bgMid"];
            }

            _sendButton.MouseEnter += (s, e) => _sendButton.BackColor = Color.FromArgb(0, 180, 150);
            _sendButton.MouseLeave += (s, e) => _sendButton.BackColor = _colours["accent"];
            _exitButton.MouseEnter += (s, e) => _exitButton.BackColor = Color.FromArgb(220, 60, 60);
            _exitButton.MouseLeave += (s, e) => _exitButton.BackColor = _colours["exit"];
        }

        // WELCOME
        private void ShowWelcome()
        {
            var logo = new List<string>
            {
                @"    ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██████╗  ██████╗ ████████╗",
                @"   ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔═══██╗╚══██╔══╝",
                @"   ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██████╔╝██║   ██║   ██║   ",
                @"   ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██╔══██╗██║   ██║   ██║   ",
                @"   ╚██████╗   ██║   ██████╔╝███████╗██║  ██║██████╔╝╚██████╔╝   ██║   ",
                @"    ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═════╝  ╚═════╝   ╚═╝   ",
            };

            foreach (string line in logo)
                AppendLine(line, _colours["accent"]);

            var welcomeLines = new List<(string text, string colourKey)>
            {
                ( "",                                                                          "white"     ),
                ( "  ════════════════════════════════════════════════════════════",            "accent"    ),
                ( "       CYBERSECURITY AWARENESS CHATBOT  — PART 3 POE          ",           "accentAlt" ),
                ( "  ════════════════════════════════════════════════════════════",            "accent"    ),
                ( "",                                                                          "white"     ),
                ( "  Welcome! I am your Cybersecurity Awareness Assistant.",                   "bot"       ),
                ( "  I am here to help you stay safe in the digital world.",                   "system"    ),
                ( "",                                                                          "white"     ),
                ( "  NEW: Task Manager, Activity Log & NLP understanding!",                    "accentAlt" ),
                ( "  Try: 'Add a task', 'Remind me to update my password', 'Show activity log'", "system" ),
                ( "",                                                                          "white"     ),
                ( "  Type your name below to get started!",                                    "accentAlt" ),
                ( "──────────────────────────────────────────────────────────────",            "system"    ),
                ( "",                                                                          "white"     ),
            };

            foreach (var (text, colourKey) in welcomeLines)
            {
                Color colour = colourKey == "white" ? Color.White : _colours[colourKey];
                AppendLine(text, colour);
            }

            _engine.SetState(GUIChatEngine.ChatState.AwaitingName);
            _statusLabel.Text = "Awaiting your name...";
        }

        private void PlayVoiceGreeting()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    using var synth = new SpeechSynthesizer();
                    synth.Rate = 0;
                    synth.Volume = 100;
                    synth.Speak("Welcome! I am your Cybersecurity Awareness Assistant.");
                    Thread.Sleep(200);
                    synth.Speak("I am here to help you stay safe in the digital world.");
                }
                catch { /* Voice unavailable — silent fallback */ }
            });
        }

        // INPUT HANDLING
        private void OnSend(object? sender, EventArgs e)
        {
            string input = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            _inputBox.Clear();

            AppendLine("", Color.White);
            AppendColoured($"  {_engine.UserName}: ", _colours["user"]);
            AppendLine(input, Color.White);

            string response = _engine.Process(input);
            HandleResponse(response);

            if (_engine.State == GUIChatEngine.ChatState.Active)
            {
                _userLabel.Text = $" ACTIVE SESSION — {_engine.UserName}";
                _userLabel.ForeColor = _colours["success"];
                _statusLabel.Text = "Ready";
            }

            ScrollToBottom();
        }

        private void ProcessCommand(string command)
        {
            AppendLine("", Color.White);
            AppendColoured("  You: ", _colours["user"]);
            AppendLine(command, Color.White);

            string response = _engine.Process(command);
            HandleResponse(response);
            ScrollToBottom();
        }

        /// <summary>
        /// Central dispatcher for all response tokens and normal text.
        /// Dictionary-driven for clean, extensible handling.
        /// </summary>
        private void HandleResponse(string response)
        {
            // Dictionary: token > action (covers all special responses)
            var specialResponses = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "LAUNCH_QUIZ", LaunchQuiz     },
                { "OPEN_TASKS",  LaunchTasks    },
                { "SHOW_LOG",    ShowActivityLog },
                { "EXIT",        ConfirmAndExit  },
            };

            if (specialResponses.TryGetValue(response, out Action? action))
            {
                action();
            }
            else if (response.Contains('\n'))
            {
                AppendBotMultiline(response);
            }
            else
            {
                AppendBotMessage(response);
            }
        }

        // QUIZ LAUNCHER 
        private void LaunchQuiz()
        {
            _engine.LogActivity("Quiz started");
            using var quizForm = new QuizForm(_engine.UserName);
            quizForm.ShowDialog(this);
            _engine.LogActivity("Quiz completed");
        }

        // TASK MANAGER LAUNCHER 
        private void LaunchTasks()
        {
            AppendBotMessage("Opening Task Manager — manage your cybersecurity tasks here!");
            _engine.LogActivity("Task Manager opened");

            using var taskForm = new TaskForm(description =>
            {
                // Log task actions back into the engine's activity log
                _engine.LogActivity(description);
            });
            taskForm.ShowDialog(this);
        }

        // ACTIVITY LOG 
        private void ShowActivityLog()
        {
            string log = _engine.GetActivityLog();
            _engine.LogActivity("Activity log viewed");
            AppendBotMultiline(log);
            ScrollToBottom();
        }

        // EXIT CONFIRMATION 
        private void OnExitClicked(object? sender, EventArgs e) => ConfirmAndExit();

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                ConfirmAndExit();
            }
        }

        private void ConfirmAndExit()
        {
            using Form confirmDialog = new Form
            {
                Text = "Exit CyberBot?",
                Size = new Size(420, 200),
                BackColor = _colours["bgPanel"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            confirmDialog.Controls.Add(new Label
            {
                Text = $"Are you sure you want to exit, {_engine.UserName}?\n\n" +
                             "Your conversation will not be saved.",
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                Location = new Point(20, 20),
                Size = new Size(370, 60),
                BackColor = Color.Transparent
            });

            var dialogButtons = new Dictionary<string, (int x, Color bg, DialogResult result)>
            {
                { "Yes, Exit", (230, _colours["exit"],   DialogResult.Yes) },
                { "No, Stay",  (110, _colours["accent"], DialogResult.No)  },
            };

            foreach (var btnDef in dialogButtons)
            {
                var btn = new Button
                {
                    Text = btnDef.Key,
                    Location = new Point(btnDef.Value.x, 110),
                    Size = new Size(110, 34),
                    BackColor = btnDef.Value.bg,
                    ForeColor = btnDef.Key == "No, Stay" ? _colours["bgDark"] : Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = btnDef.Value.result,
                    FlatAppearance = { BorderSize = 0 }
                };
                confirmDialog.Controls.Add(btn);
            }

            if (confirmDialog.ShowDialog(this) == DialogResult.Yes)
            {
                AppendBotMessage($"Stay safe online, {_engine.UserName}! Goodbye! ");
                ScrollToBottom();

                _sendButton.Enabled = false;
                _exitButton.Enabled = false;
                _inputBox.Enabled = false;
                _statusLabel.Text = "Session ended.";
                _userLabel.Text = " SESSION ENDED";
                _userLabel.ForeColor = _colours["warn"];

                System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                {
                    Invoke(new Action(Application.Exit));
                });
            }
        }

        // CHAT DISPLAY HELPERS 
        public void AppendBotMessage(string message)
        {
            if (InvokeRequired) { Invoke(new Action(() => AppendBotMessage(message))); return; }
            AppendLine("", Color.White);
            AppendColoured("  Bot: ", _colours["bot"]);
            AppendLine(message, Color.White);
        }

        public void AppendSystemMessage(string message)
        {
            if (InvokeRequired) { Invoke(new Action(() => AppendSystemMessage(message))); return; }
            AppendLine($"  [System] {message}", _colours["system"]);
        }

        private void AppendBotMultiline(string message)
        {
            AppendLine("", Color.White);
            AppendColoured("  Bot:\n", _colours["bot"]);
            AppendLine(message, Color.White);
        }

        private void AppendLine(string text, Color colour)
        {
            _chatDisplay.SelectionStart = _chatDisplay.TextLength;
            _chatDisplay.SelectionLength = 0;
            _chatDisplay.SelectionColor = colour;
            _chatDisplay.AppendText(text + "\n");
            _chatDisplay.SelectionColor = _chatDisplay.ForeColor;
        }

        private void AppendColoured(string text, Color colour)
        {
            _chatDisplay.SelectionStart = _chatDisplay.TextLength;
            _chatDisplay.SelectionLength = 0;
            _chatDisplay.SelectionColor = colour;
            _chatDisplay.AppendText(text);
            _chatDisplay.SelectionColor = _chatDisplay.ForeColor;
        }

        private void ScrollToBottom()
        {
            _chatDisplay.SelectionStart = _chatDisplay.Text.Length;
            _chatDisplay.ScrollToCaret();
        }
    }
}
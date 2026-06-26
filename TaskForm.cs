using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.GUI
{
    public class TaskForm : Form
    {
        //UI Controls
        private TextBox _titleBox = null!;
        private TextBox _descBox = null!;
        private RadioButton _rdoDate = null!;
        private RadioButton _rdoTimeframe = null!;
        private DateTimePicker _datePicker = null!;
        private ComboBox _timeframeCombo = null!;
        private Button _completeButton = null!;
        private Button _deleteButton = null!;
        private Button _closeButton = null!;
        private Button _addButton = null!;
        private Label _statusLabel = null!;
        private ListView _taskList = null!;

        private readonly Action<string>? _logAction;

        private readonly Dictionary<string, Color> _colours = new Dictionary<string, Color>
        {
            { "bgDark",    Color.FromArgb(10,  14,  20)  },
            { "bgMid",     Color.FromArgb(16,  22,  32)  },
            { "bgPanel",   Color.FromArgb(20,  28,  42)  },
            { "accent",    Color.FromArgb(0,   220, 180) },
            { "accentAlt", Color.FromArgb(255, 180, 0)   },
            { "success",   Color.FromArgb(80,  220, 100) },
            { "warn",      Color.FromArgb(255, 80,  80)  },
            { "system",    Color.FromArgb(120, 140, 160) },
            { "exit",      Color.FromArgb(180, 40,  40)  },
            { "bgMid2",    Color.FromArgb(16,  22,  32)  },
        };

        public TaskForm(Action<string>? logAction = null)
        {
            _logAction = logAction;
            InitialiseComponent();
            LoadTasks();
        }

        private void InitialiseComponent()
        {
            Text = "CyberBot - Task Assistant";
            Size = new Size(860, 680);
            MinimumSize = new Size(720, 560);
            BackColor = _colours["bgDark"];
            ForeColor = Color.White;
            Font = new Font("Consolas", 9.5f);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;

            //Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = _colours["bgPanel"]
            };
            header.Paint += (s, e) =>
            {
                using var pen = new Pen(_colours["accent"], 1f);
                e.Graphics.DrawLine(pen, 0, header.Height - 1,
                    header.Width, header.Height - 1);
            };
            header.Controls.Add(new Label
            {
                Text = "  TASK ASSISTANT - CYBERSECURITY TASK MANAGER",
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            });

            //Input panel
            var inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 175,
                BackColor = _colours["bgMid"],
                Padding = new Padding(14)
            };

            //Title
            inputPanel.Controls.Add(MakeLabel("Task Title:", new Point(14, 14)));
            _titleBox = new TextBox
            {
                Location = new Point(120, 12),
                Size = new Size(700, 24),
                BackColor = _colours["bgDark"],
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5f),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            SetPlaceholder(_titleBox,
                "e.g. Enable two-factor authentication on all accounts");
            inputPanel.Controls.Add(_titleBox);

            //Description
            inputPanel.Controls.Add(MakeLabel("Description:", new Point(14, 50)));
            _descBox = new TextBox
            {
                Location = new Point(120, 48),
                Size = new Size(700, 52),
                BackColor = _colours["bgDark"],
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5f),
                Multiline = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            SetPlaceholder(_descBox,
                "Describe the cybersecurity task in detail...");
            inputPanel.Controls.Add(_descBox);

            //Reminder label
            inputPanel.Controls.Add(new Label
            {
                Text = "Reminder:",
                ForeColor = _colours["system"],
                Font = new Font("Consolas", 9f),
                Location = new Point(14, 114),
                Size = new Size(100, 22),
                BackColor = Color.Transparent
            });

            //Radio: Specific Date
            _rdoDate = new RadioButton
            {
                Text = "Specific Date:",
                Location = new Point(120, 112),
                Size = new Size(130, 22),
                ForeColor = _colours["accentAlt"],
                BackColor = Color.Transparent,
                Font = new Font("Consolas", 8.5f),
                Checked = true
            };
            inputPanel.Controls.Add(_rdoDate);

            //Date picker
            _datePicker = new DateTimePicker
            {
                Location = new Point(258, 110),
                Size = new Size(190, 24),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Font = new Font("Consolas", 9f)
            };
            inputPanel.Controls.Add(_datePicker);

            // Radio: Timeframe
            _rdoTimeframe = new RadioButton
            {
                Text = "Timeframe:",
                Location = new Point(460, 112),
                Size = new Size(110, 22),
                ForeColor = _colours["accentAlt"],
                BackColor = Color.Transparent,
                Font = new Font("Consolas", 8.5f)
            };
            inputPanel.Controls.Add(_rdoTimeframe);

            // Timeframe combo
            _timeframeCombo = new ComboBox
            {
                Location = new Point(578, 110),
                Size = new Size(160, 24),
                BackColor = _colours["bgDark"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 9f),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            var timeframes = new Dictionary<string, int>
            {
                { "In 1 day",    1  },
                { "In 3 days",   3  },
                { "In 1 week",   7  },
                { "In 2 weeks",  14 },
                { "In 1 month",  30 },
                { "In 3 months", 90 },
            };
            foreach (var tf in timeframes)
                _timeframeCombo.Items.Add(tf.Key);
            _timeframeCombo.SelectedIndex = 0;
            inputPanel.Controls.Add(_timeframeCombo);

            // Wire radio buttons
            _rdoDate.CheckedChanged += (s, e) =>
            {
                _datePicker.Enabled = _rdoDate.Checked;
                _timeframeCombo.Enabled = !_rdoDate.Checked;
            };
            _rdoTimeframe.CheckedChanged += (s, e) =>
            {
                _datePicker.Enabled = !_rdoTimeframe.Checked;
                _timeframeCombo.Enabled = _rdoTimeframe.Checked;
            };

            // No reminder option
            var rdoNone = new RadioButton
            {
                Text = "No reminder",
                Location = new Point(120, 142),
                Size = new Size(140, 22),
                ForeColor = _colours["system"],
                BackColor = Color.Transparent,
                Font = new Font("Consolas", 8.5f),
                Tag = "none"
            };
            rdoNone.CheckedChanged += (s, e) =>
            {
                if (rdoNone.Checked)
                {
                    _datePicker.Enabled = false;
                    _timeframeCombo.Enabled = false;
                }
            };
            inputPanel.Controls.Add(rdoNone);

            // Task ListView
            _taskList = new ListView
            {
                Dock = DockStyle.Fill,
                BackColor = _colours["bgDark"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 9f),
                FullRowSelect = true,
                View = View.Details,
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };

            var columns = new Dictionary<string, int>
            {
                { "ID",          38  },
                { "Title",       190 },
                { "Description", 230 },
                { "Status",      75  },
                { "Reminder",    150 },
                { "Created",     120 },
            };
            foreach (var col in columns)
                _taskList.Columns.Add(col.Key, col.Value);

            _taskList.SelectedIndexChanged += (s, e) => UpdateButtonStates();

            // Bottom panel
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = _colours["bgPanel"]
            };
            bottomPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(_colours["accent"], 1f);
                e.Graphics.DrawLine(pen, 0, 0, bottomPanel.Width, 0);
            };

            // ADD TASK button
            _addButton = MakeButton("+ ADD TASK", new Point(14, 12),
                new Size(120, 32), "accent");
            _addButton.ForeColor = _colours["bgDark"];
            _addButton.Click += OnAddTask;

            // MARK DONE button
            _completeButton = MakeButton("MARK DONE", new Point(144, 12),
                new Size(120, 32), "success");
            _completeButton.Click += OnMarkComplete;
            _completeButton.Enabled = false;

            // DELETE button
            _deleteButton = MakeButton("DELETE", new Point(274, 12),
                new Size(100, 32), "exit");
            _deleteButton.Click += OnDeleteTask;
            _deleteButton.Enabled = false;

            // Status label
            _statusLabel = new Label
            {
                Text = "Add a cybersecurity task above to get started.",
                ForeColor = _colours["system"],
                Font = new Font("Consolas", 8f),
                Location = new Point(386, 18),
                Size = new Size(300, 20),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // CLOSE button
            _closeButton = MakeButton("CLOSE", new Point(0, 12),
                new Size(90, 32), "bgMid");
            _closeButton.ForeColor = _colours["accent"];
            _closeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _closeButton.Click += (s, e) => Close();

            bottomPanel.Controls.AddRange(new Control[]
            {
                _addButton, _completeButton, _deleteButton,
                _statusLabel, _closeButton
            });

            bottomPanel.Resize += (s, e) =>
                _closeButton.Location = new Point(bottomPanel.Width - 104, 12);

            Controls.Add(_taskList);
            Controls.Add(inputPanel);
            Controls.Add(header);
            Controls.Add(bottomPanel);
        }

        // LOAD TASKS
        private void LoadTasks()
        {
            _taskList.Items.Clear();
            var tasks = DatabaseHelper.GetAllTasks();

            foreach (var task in tasks)
            {
                var item = new ListViewItem(task.Id.ToString());
                item.SubItems.Add(task.Title);
                item.SubItems.Add(task.Description);
                item.SubItems.Add(task.StatusDisplay);
                item.SubItems.Add(task.ReminderDisplay);
                item.SubItems.Add(task.CreatedAt);
                item.Tag = task.Id;

                if (task.IsComplete)
                    item.ForeColor = _colours["success"];
                else if (!string.IsNullOrWhiteSpace(task.ReminderDate) &&
                         DateTime.TryParse(task.ReminderDate, out DateTime rd) &&
                         rd.Date <= DateTime.Today)
                    item.ForeColor = _colours["warn"];

                _taskList.Items.Add(item);
            }

            _statusLabel.Text = tasks.Count + " task(s) loaded from database.";
        }

        // ADD TASK
        private void OnAddTask(object? sender, EventArgs e)
        {
            string title = _titleBox.Text.Trim();
            string desc = _descBox.Text.Trim();

            if (IsPlaceholder(_titleBox) || string.IsNullOrWhiteSpace(title))
            {
                SetStatus("Please enter a task title.", "warn");
                return;
            }
            if (IsPlaceholder(_descBox) || string.IsNullOrWhiteSpace(desc))
            {
                SetStatus("Please enter a task description.", "warn");
                return;
            }

            string? reminderDate = null;
            string reminderDisplay = "No reminder";

            if (_rdoDate.Checked)
            {
                reminderDate = _datePicker.Value.ToString("yyyy-MM-dd");
                reminderDisplay = "Date: " +
                    _datePicker.Value.ToString("dd MMM yyyy");
            }
            else if (_rdoTimeframe.Checked &&
                     _timeframeCombo.SelectedItem != null)
            {
                string selected =
                    _timeframeCombo.SelectedItem.ToString() ?? "";

                var timeframeDays = new Dictionary<string, int>
                    (StringComparer.OrdinalIgnoreCase)
                {
                    { "In 1 day",    1  },
                    { "In 3 days",   3  },
                    { "In 1 week",   7  },
                    { "In 2 weeks",  14 },
                    { "In 1 month",  30 },
                    { "In 3 months", 90 },
                };

                if (timeframeDays.TryGetValue(selected, out int days))
                {
                    DateTime reminderDt = DateTime.Today.AddDays(days);
                    reminderDate = reminderDt.ToString("yyyy-MM-dd");
                    reminderDisplay = selected + " (" +
                        reminderDt.ToString("dd MMM yyyy") + ")";
                }
            }

            try
            {
                int id = DatabaseHelper.AddTask(title, desc, reminderDate);

                string logMsg = reminderDate != null
                    ? "Task added: '" + title + "' | Reminder: " +
                      reminderDisplay
                    : "Task added: '" + title + "' | No reminder";

                _logAction?.Invoke(logMsg);
                SetStatus("Task '" + title + "' added successfully (ID: " +
                    id + ").", "success");

                ClearInput(_titleBox,
                    "e.g. Enable two-factor authentication on all accounts");
                ClearInput(_descBox,
                    "Describe the cybersecurity task in detail...");
                _rdoDate.Checked = true;

                LoadTasks();
            }
            catch (Exception ex)
            {
                SetStatus("Error saving task: " + ex.Message, "warn");
            }
        }

        // MARK COMPLETE
        private void OnMarkComplete(object? sender, EventArgs e)
        {
            if (_taskList.SelectedItems.Count == 0) return;

            int id = (int)_taskList.SelectedItems[0].Tag!;
            string title = _taskList.SelectedItems[0].SubItems[1].Text;

            try
            {
                DatabaseHelper.MarkComplete(id);
                _logAction?.Invoke("Task completed: '" + title + "'");
                SetStatus("Task '" + title + "' marked as complete.",
                    "success");
                LoadTasks();
            }
            catch (Exception ex)
            {
                SetStatus("Error: " + ex.Message, "warn");
            }
        }

        // DELETE TASK
        private void OnDeleteTask(object? sender, EventArgs e)
        {
            if (_taskList.SelectedItems.Count == 0) return;

            int id = (int)_taskList.SelectedItems[0].Tag!;
            string title = _taskList.SelectedItems[0].SubItems[1].Text;

            var confirm = MessageBox.Show(
                "Delete task '" + title + "'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteTask(id);
                    _logAction?.Invoke("Task deleted: '" + title + "'");
                    SetStatus("Task '" + title + "' deleted.", "success");
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    SetStatus("Error: " + ex.Message, "warn");
                }
            }
        }

        // HELPERS
        private void UpdateButtonStates()
        {
            bool selected = _taskList.SelectedItems.Count > 0;
            _completeButton.Enabled = selected;
            _deleteButton.Enabled = selected;
        }

        private void SetStatus(string message, string colourKey)
        {
            _statusLabel.Text = message;
            _statusLabel.ForeColor = _colours.ContainsKey(colourKey)
                ? _colours[colourKey] : Color.White;
        }

        private Label MakeLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                ForeColor = _colours["system"],
                Font = new Font("Consolas", 9f),
                Location = location,
                Size = new Size(105, 22),
                BackColor = Color.Transparent
            };
        }

        private Button MakeButton(string text, Point location,
            Size size, string colourKey)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = _colours.ContainsKey(colourKey)
                    ? _colours[colourKey] : _colours["bgMid"],
                ForeColor = colourKey == "success"
                    ? _colours["bgDark"] : Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        // Placeholder helpers
        private static readonly Dictionary<TextBox, string> _placeholders = new();

        private void SetPlaceholder(TextBox box, string placeholder)
        {
            _placeholders[box] = placeholder;
            box.Text = placeholder;
            box.ForeColor = Color.FromArgb(80, 100, 120);

            box.GotFocus += (s, e) =>
            {
                if (IsPlaceholder(box))
                {
                    box.Text = "";
                    box.ForeColor = Color.White;
                }
            };
            box.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                {
                    box.Text = placeholder;
                    box.ForeColor = Color.FromArgb(80, 100, 120);
                }
            };
        }

        private static bool IsPlaceholder(TextBox box)
        {
            return _placeholders.TryGetValue(box, out string? p)
                && box.Text == p;
        }

        private void ClearInput(TextBox box, string placeholder)
        {
            box.Text = placeholder;
            box.ForeColor = Color.FromArgb(80, 100, 120);
        }
    }
}

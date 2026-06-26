using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.GUI
{
    /// <summary>
    /// POE Task 2: Cybersecurity Mini-Game (Quiz) GUI.
    /// 4 rounds x 5 questions = 20 total questions.
    /// Mixed multiple-choice and true/false formats.
    /// Features: coloured answer buttons, progress bar, round results,
    /// question type label, final score with feedback, exit confirmation.
    /// </summary>
    public class QuizForm : Form
    {
        // UI Controls
        private Label _roundLabel = null!;
        private Label _questionNumLabel = null!;
        private Label _scoreLabel = null!;
        private Label _totalScoreLabel = null!;
        private Label _questionLabel = null!;
        private Label _typeLabel = null!;
        private Panel _optionsPanel = null!;
        private Label _feedbackLabel = null!;
        private Button _actionButton = null!;
        private Button _exitButton = null!;
        private ProgressBar _progressBar = null!;

        // State
        private readonly string _userName;
        private int _currentRound = 0;
        private int _currentQ = 0;
        private int _roundScore = 0;
        private int _totalScore = 0;
        private bool _answered = false;
        private bool _quizFinished = false;
        private bool _forceClose = false;

        private List<KeyValuePair<int, QuizQuestion>> _currentQuestions = new();

        // Colour Palette
        private readonly Dictionary<string, Color> _colours = new Dictionary<string, Color>
        {
            { "bgDark",    Color.FromArgb(10,  14,  20)  },
            { "bgMid",     Color.FromArgb(16,  22,  32)  },
            { "bgPanel",   Color.FromArgb(20,  28,  42)  },
            { "accent",    Color.FromArgb(0,   220, 180) },
            { "accentAlt", Color.FromArgb(255, 180, 0)   },
            { "correct",   Color.FromArgb(80,  220, 100) },
            { "wrong",     Color.FromArgb(255, 80,  80)  },
            { "neutral",   Color.FromArgb(60,  80,  110) },
            { "exit",      Color.FromArgb(180, 40,  40)  },
            { "hover",     Color.FromArgb(0,   60,  55)  },
            { "trueFalse", Color.FromArgb(255, 140, 0)   },
            { "multiChoice", Color.FromArgb(0, 180, 220) },
        };

        // Constructor
        public QuizForm(string userName)
        {
            _userName = userName;
            InitialiseComponent();
            LoadRound(_currentRound);
        }

        // UI INIT
        private void InitialiseComponent()
        {
            Text = "CyberBot - Cybersecurity Quiz";
            Size = new Size(720, 620);
            BackColor = _colours["bgDark"];
            ForeColor = Color.White;
            Font = new Font("Consolas", 10f);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            FormClosing += OnFormClosing;

            // Header
            var header = new Label
            {
                Text = "  CYBERSECURITY QUIZ  -  " + _userName.ToUpper(),
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 11f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = _colours["bgPanel"],
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sub-header: total questions info
            var subHeader = new Label
            {
                Text = "  4 Rounds  |  20 Questions Total  |  Mixed Multiple-Choice & True/False",
                ForeColor = _colours["accentAlt"],
                Font = new Font("Consolas", 8f),
                Dock = DockStyle.Top,
                Height = 24,
                BackColor = _colours["bgMid"],
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Progress bar
            _progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 7,
                Minimum = 0,
                Maximum = 5,
                Value = 0,
                Style = ProgressBarStyle.Continuous,
                BackColor = _colours["bgMid"],
                ForeColor = _colours["accent"]
            };

            // Round label
            _roundLabel = new Label
            {
                Text = "",
                ForeColor = _colours["accentAlt"],
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                Location = new Point(20, 86),
                Size = new Size(500, 22),
                BackColor = Color.Transparent
            };

            // Question number label
            _questionNumLabel = new Label
            {
                Text = "",
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 8.5f),
                Location = new Point(20, 110),
                Size = new Size(300, 18),
                BackColor = Color.Transparent
            };

            // Round score label
            _scoreLabel = new Label
            {
                Text = "Round Score: 0/5",
                ForeColor = _colours["accent"],
                Font = new Font("Consolas", 8.5f),
                Location = new Point(520, 110),
                Size = new Size(175, 18),
                BackColor = Color.Transparent
            };

            // Total score label
            _totalScoreLabel = new Label
            {
                Text = "Total: 0/20",
                ForeColor = _colours["accentAlt"],
                Font = new Font("Consolas", 8.5f),
                Location = new Point(520, 86),
                Size = new Size(175, 22),
                BackColor = Color.Transparent
            };

            // Question type badge label
            _typeLabel = new Label
            {
                Text = "",
                Font = new Font("Consolas", 8f, FontStyle.Bold),
                Location = new Point(20, 132),
                Size = new Size(180, 20),
                BackColor = Color.Transparent
            };

            // Question label
            _questionLabel = new Label
            {
                Text = "",
                ForeColor = Color.White,
                Font = new Font("Consolas", 10.5f),
                Location = new Point(20, 158),
                Size = new Size(670, 65),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            // Options panel
            _optionsPanel = new Panel
            {
                Location = new Point(20, 228),
                Size = new Size(670, 220),
                BackColor = Color.Transparent
            };

            // Feedback label
            _feedbackLabel = new Label
            {
                Text = "",
                ForeColor = _colours["correct"],
                Font = new Font("Consolas", 8.5f),
                Location = new Point(20, 455),
                Size = new Size(670, 60),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            // Action button (Next / See Results / Close)
            _actionButton = new Button
            {
                Text = "Next Question >",
                Location = new Point(510, 540),
                Size = new Size(185, 38),
                BackColor = _colours["accent"],
                ForeColor = _colours["bgDark"],
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false,
                FlatAppearance = { BorderSize = 0 }
            };
            _actionButton.Click += OnActionButton;

            // Exit button
            _exitButton = new Button
            {
                Text = "EXIT X",
                Location = new Point(20, 540),
                Size = new Size(100, 38),
                BackColor = _colours["exit"],
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            _exitButton.Click += OnExitClicked;
            _exitButton.MouseEnter += (s, e) => _exitButton.BackColor = Color.FromArgb(220, 60, 60);
            _exitButton.MouseLeave += (s, e) => _exitButton.BackColor = _colours["exit"];

            Controls.AddRange(new Control[]
            {
                header, subHeader, _progressBar,
                _roundLabel, _questionNumLabel, _scoreLabel, _totalScoreLabel,
                _typeLabel, _questionLabel, _optionsPanel,
                _feedbackLabel, _actionButton, _exitButton
            });
        }

        // ROUND LOADING
        private void LoadRound(int roundIndex)
        {
            _currentQ = 0;
            _roundScore = 0;
            _answered = false;

            _currentQuestions = new List<KeyValuePair<int, QuizQuestion>>(QuizBank.AllRounds[roundIndex]);
            _progressBar.Value = 0;
            _progressBar.Maximum = _currentQuestions.Count;
            _roundLabel.Text = QuizBank.RoundTitles[roundIndex];
            _scoreLabel.Text = "Round Score: 0/" + _currentQuestions.Count;

            LoadQuestion();
        }

        // QUESTION LOADING
        private void LoadQuestion()
        {
            if (_currentQ >= _currentQuestions.Count)
            {
                ShowRoundResults();
                return;
            }

            _answered = false;
            _feedbackLabel.Text = "";
            _actionButton.Enabled = false;
            _actionButton.Text = "Next Question >";
            _progressBar.Value = _currentQ;

            var q = _currentQuestions[_currentQ].Value;

            _questionNumLabel.Text = "Question " + (_currentQ + 1) + " of " + _currentQuestions.Count;
            _questionLabel.Text = q.Question;

            // Show question type badge
            if (q.QuestionType == "true-false")
            {
                _typeLabel.Text = "  TRUE / FALSE  ";
                _typeLabel.ForeColor = _colours["trueFalse"];
            }
            else
            {
                _typeLabel.Text = "  MULTIPLE CHOICE  ";
                _typeLabel.ForeColor = _colours["multiChoice"];
            }

            // Build answer buttons
            _optionsPanel.Controls.Clear();
            int y = 0;

            foreach (var opt in q.Options)
            {
                string optKey = opt.Key;
                string optText = opt.Value;

                var btn = new Button
                {
                    Text = "  " + optKey + ")  " + optText,
                    Location = new Point(0, y),
                    Size = new Size(670, 48),
                    BackColor = _colours["neutral"],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 9f),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand,
                    Tag = optKey,
                    FlatAppearance =
                    {
                        BorderColor = Color.FromArgb(60, 80, 120),
                        BorderSize  = 1
                    }
                };

                btn.Click += OnAnswerSelected;
                btn.MouseEnter += (s, e) => { if (!_answered) ((Button)s!).BackColor = _colours["hover"]; };
                btn.MouseLeave += (s, e) => { if (!_answered) ((Button)s!).BackColor = _colours["neutral"]; };

                _optionsPanel.Controls.Add(btn);
                y += 52;
            }
        }

        // ANSWER SELECTION
        private void OnAnswerSelected(object? sender, EventArgs e)
        {
            if (_answered) return;
            _answered = true;

            var clicked = (Button)sender!;
            string selected = clicked.Tag?.ToString() ?? "";
            var q = _currentQuestions[_currentQ].Value;
            bool isCorrect = selected.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

            // Colour all buttons: green = correct, red = wrong selection, grey = other
            foreach (Button btn in _optionsPanel.Controls)
            {
                string key = btn.Tag?.ToString() ?? "";
                if (key == q.CorrectAnswer)
                    btn.BackColor = _colours["correct"];
                else if (key == selected)
                    btn.BackColor = _colours["wrong"];
                else
                    btn.BackColor = _colours["neutral"];
                btn.Enabled = false;
            }

            if (isCorrect)
            {
                _roundScore++;
                _totalScore++;
                _feedbackLabel.ForeColor = _colours["correct"];
                _feedbackLabel.Text = "CORRECT!  " + q.Explanation;
            }
            else
            {
                _feedbackLabel.ForeColor = _colours["wrong"];
                _feedbackLabel.Text =
                    "INCORRECT.  Correct answer: " + q.CorrectAnswer + ") " +
                    q.Options[q.CorrectAnswer] + "\n   " + q.Explanation;
            }

            _scoreLabel.Text = "Round Score: " + _roundScore + "/" + _currentQuestions.Count;
            _totalScoreLabel.Text = "Total: " + _totalScore + "/20";
            _actionButton.Enabled = true;

            bool isLastQ = _currentQ == _currentQuestions.Count - 1;
            bool isLastRound = _currentRound == QuizBank.AllRounds.Count - 1;

            if (isLastQ)
                _actionButton.Text = isLastRound ? "See Final Results >" : "See Round Results >";
        }

        // ACTION BUTTON
        private void OnActionButton(object? sender, EventArgs e)
        {
            _currentQ++;
            if (_currentQ >= _currentQuestions.Count)
            {
                bool isLastRound = _currentRound == QuizBank.AllRounds.Count - 1;
                if (isLastRound) ShowFinalResults();
                else ShowRoundResults();
            }
            else
            {
                LoadQuestion();
            }
        }

        // ROUND RESULTS
        private void ShowRoundResults()
        {
            _optionsPanel.Controls.Clear();
            _progressBar.Value = _currentQuestions.Count;
            _questionNumLabel.Text = "Round Complete!";
            _typeLabel.Text = "";

            // Grade dictionary: min score > message
            var grades = new Dictionary<int, string>
            {
                { 5, "Perfect round! Outstanding!" },
                { 4, "Excellent! Nearly perfect!"  },
                { 3, "Good job! Keep it up!"       },
                { 2, "Not bad, review what you missed." },
                { 0, "Keep studying, you will get there!" },
            };

            string grade = "Keep studying, you will get there!";
            foreach (var g in grades)
                if (_roundScore >= g.Key) { grade = g.Value; break; }

            _questionLabel.Font = new Font("Consolas", 11f, FontStyle.Bold);
            _questionLabel.Text = "Round " + (_currentRound + 1) + " Complete!";

            _feedbackLabel.Font = new Font("Consolas", 10f);
            _feedbackLabel.ForeColor = _colours["accentAlt"];
            _feedbackLabel.Text =
                "Round Score: " + _roundScore + "/" + _currentQuestions.Count +
                "\n" + grade +
                "\nTotal so far: " + _totalScore + "/" + ((_currentRound + 1) * 5);

            _actionButton.Text = "Next Round >";
            _actionButton.Enabled = true;
            _actionButton.Click -= OnActionButton;
            _actionButton.Click += OnNextRound;
        }

        // NEXT ROUND
        private void OnNextRound(object? sender, EventArgs e)
        {
            _currentRound++;
            _questionLabel.Font = new Font("Consolas", 10.5f);
            _feedbackLabel.Font = new Font("Consolas", 8.5f);
            _actionButton.Text = "Next Question >";
            _actionButton.Click -= OnNextRound;
            _actionButton.Click += OnActionButton;
            LoadRound(_currentRound);
        }

        // FINAL RESULTS
        private void ShowFinalResults()
        {
            _quizFinished = true;
            _optionsPanel.Controls.Clear();
            _progressBar.Value = _currentQuestions.Count;
            _questionNumLabel.Text = "Quiz Complete!";
            _roundLabel.Text = "All 4 Rounds Finished!";
            _typeLabel.Text = "";

            // Final grade dictionary: min score > message
            var finalGrades = new Dictionary<int, string>
            {
                { 20, "PERFECT SCORE! You are a Cybersecurity Expert!"           },
                { 16, "Outstanding! You really know your stuff!"                  },
                { 12, "Great work! Keep building your knowledge."                 },
                { 8,  "Not bad, consider brushing up on the topics you missed."   },
                { 0,  "Keep learning! Cybersecurity knowledge keeps you safe."    },
            };

            string finalGrade = "Keep learning! Cybersecurity knowledge keeps you safe.";
            foreach (var g in finalGrades)
                if (_totalScore >= g.Key) { finalGrade = g.Value; break; }

            _questionLabel.Font = new Font("Consolas", 11f, FontStyle.Bold);
            _questionLabel.Text = "Final Results - " + _userName;

            _feedbackLabel.Font = new Font("Consolas", 10f);
            _feedbackLabel.ForeColor = _colours["accentAlt"];
            _feedbackLabel.Text =
                "Final Score: " + _totalScore + " / 20\n\n" + finalGrade;

            _scoreLabel.Text = "Round Score: " + _roundScore + "/5";
            _totalScoreLabel.Text = "Total: " + _totalScore + "/20";

            _actionButton.Text = "Close Quiz";
            _actionButton.Enabled = true;
            _actionButton.Click -= OnActionButton;
            _actionButton.Click += (s, e) => { _forceClose = true; Close(); };

            _exitButton.Visible = false;
        }

        // EXIT CONFIRMATION
        private void OnExitClicked(object? sender, EventArgs e) => ConfirmExit();

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_forceClose) return;
            if (e.CloseReason == CloseReason.UserClosing && !_quizFinished)
            {
                e.Cancel = true;
                ConfirmExit();
            }
        }

        private void ConfirmExit()
        {
            using Form dialog = new Form
            {
                Text = "Exit Quiz?",
                Size = new Size(400, 190),
                BackColor = _colours["bgPanel"],
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            dialog.Controls.Add(new Label
            {
                Text = "Are you sure you want to exit the quiz?\n\nYour progress will not be saved.",
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5f),
                Location = new Point(20, 20),
                Size = new Size(350, 60),
                BackColor = Color.Transparent
            });

            // Dictionary: label > (x, colour, result)
            var dialogButtons = new Dictionary<string, (int x, string colourKey, DialogResult result)>
            {
                { "Yes, Exit", (220, "exit",   DialogResult.Yes) },
                { "No, Stay",  (100, "accent", DialogResult.No)  },
            };

            foreach (var def in dialogButtons)
            {
                var btn = new Button
                {
                    Text = def.Key,
                    Location = new Point(def.Value.x, 110),
                    Size = new Size(110, 34),
                    BackColor = _colours[def.Value.colourKey],
                    ForeColor = def.Key == "No, Stay" ? _colours["bgDark"] : Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = def.Value.result,
                    FlatAppearance = { BorderSize = 0 }
                };
                dialog.Controls.Add(btn);
            }

            if (dialog.ShowDialog(this) == DialogResult.Yes)
            {
                _forceClose = true;
                Close();
            }
        }
    }
}
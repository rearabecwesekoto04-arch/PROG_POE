using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbot.Data;

namespace CybersecurityChatbot.Core
{
    /// <summary>
    /// GUI-aware chat engine for POE Part 3.
    /// Features: NLP simulation, Activity Log, conversational task-adding flow,
    /// mood detection, memory recall, random responses, follow-up handling.
    /// All data structures use Dictionary and List as required by the rubric.
    /// </summary>
    public class GUIChatEngine
    {
        // State Machine
        public enum ChatState { AwaitingName, AwaitingFavTopic, Active, AwaitingTaskTitle, AwaitingTaskDesc, AwaitingTaskReminder }
        public ChatState State { get; private set; } = ChatState.AwaitingName;

        // User Memory
        public string UserName { get; private set; } = string.Empty;
        public string FavouriteTopic { get; private set; } = string.Empty;

        // Conversation Context
        private string _lastTopic = string.Empty;
        private string _lastResponse = string.Empty;
        private int _tipIndex = 0;

        // Conversation History (Dictionary: label > message)
        private readonly Dictionary<string, string> _conversationHistory = new();
        private int _historyIndex = 1;

        // Topic Number Map (Dictionary: number > topic key)
        private readonly Dictionary<int, string> _topicNumberMap = new();

        // Activity Log (POE Task 4): List of (timestamp, description)
        private readonly List<(string Timestamp, string Description)> _activityLog = new();

        // Pending task fields for conversational task-adding flow
        private string _pendingTaskTitle = string.Empty;
        private string _pendingTaskDesc = string.Empty;

        // Callbacks to GUI
        private readonly Action<string> _appendBot;
        private readonly Action<string> _appendSystem;

        // Mood Keywords (Dictionary: keyword > mood label)
        private readonly Dictionary<string, string> _moodKeywords =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",     "worried"     },
            { "scared",      "scared"      },
            { "confused",    "confused"    },
            { "frustrated",  "frustrated"  },
            { "curious",     "curious"     },
            { "happy",       "happy"       },
            { "excited",     "excited"     },
            { "angry",       "angry"       },
            { "nervous",     "nervous"     },
            { "anxious",     "anxious"     },
            { "unsure",      "unsure"      },
            { "bored",       "bored"       },
            { "overwhelmed", "overwhelmed" },
            { "confident",   "confident"   },
            { "stressed",    "stressed"    },
        };

        // Mood-Only Responses (Dictionary: mood > full response)
        private readonly Dictionary<string, string> _moodOnlyResponses =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",
              "I can hear that you're worried, {name}, and that's completely understandable. " +
              "Cybersecurity threats are real, but knowledge is your greatest shield. " +
              "Start with strong unique passwords and enable Two-Factor Authentication. " +
              "Would you like me to guide you through any specific concern?" },
            { "scared",
              "It's okay to feel scared, {name}. The digital world can feel overwhelming. " +
              "With a few simple habits you can protect yourself very effectively. " +
              "Start small: update your software, use a password manager, and never click suspicious links." },
            { "confused",
              "No worries at all, {name}! Cybersecurity can feel like a lot to take in. " +
              "Let's keep things simple. Type 'topics' to see everything I can explain, " +
              "or just ask me something like 'what is phishing?' and I'll break it down in plain language!" },
            { "frustrated",
              "I hear you, {name}, and I'm sorry you're feeling frustrated. " +
              "Cybersecurity can feel complicated, but I'm here to make it easier. " +
              "Let's slow things down. Tell me what's confusing you and we'll work through it together." },
            { "curious",
              "I love the curiosity, {name}! That's exactly the right mindset for staying safe online. " +
              "Type 'topics' to see everything I cover, or fire away with your questions!" },
            { "happy",
              "That's wonderful to hear, {name}! Since you're in a good mood, " +
              "why not level up your cybersecurity knowledge? Type 'quiz' to test yourself!" },
            { "excited",
              "Love the energy, {name}! Let's channel that excitement into something useful. " +
              "Type 'quiz' to test your cybersecurity knowledge, or ask me about phishing, VPNs, and more!" },
            { "angry",
              "I'm sorry something has upset you, {name}. " +
              "If it's related to a cybersecurity incident, change your passwords immediately and enable 2FA. " +
              "Take a breath. Let's figure this out together. What happened?" },
            { "nervous",
              "It's natural to feel nervous about online safety, {name}. " +
              "Most cyberattacks can be prevented with simple habits: strong passwords, updates, and caution with links." },
            { "anxious",
              "Take a breath, {name}. You're in the right place. " +
              "The best cure for anxiety is knowledge. Let's tackle one topic at a time. " +
              "What's worrying you most right now?" },
            { "unsure",
              "That's perfectly okay, {name}! Nobody knows everything about cybersecurity. " +
              "Type 'topics' to see all the subjects I can explain, or just describe what you need help with." },
            { "bored",
              "Bored? Let's fix that, {name}! Type 'quiz' to test your cybersecurity knowledge. " +
              "Or ask me about zero-day vulnerabilities or how ransomware works. More fascinating than it sounds!" },
            { "overwhelmed",
              "I completely understand, {name}. Let's simplify. Focus on just three things: " +
              "strong passwords, Two-Factor Authentication, and being careful with emails. " +
              "Master those and you'll already be safer than most people online." },
            { "confident",
              "That's the spirit, {name}! Type 'quiz' to put your knowledge to the test, " +
              "or ask me about advanced topics like penetration testing or zero-day vulnerabilities!" },
            { "stressed",
              "I'm sorry you're feeling stressed, {name}. " +
              "If it's about a cybersecurity situation: change your passwords, check for suspicious activity, " +
              "and enable 2FA. Tell me what's going on and I'll help." },
        };

        // Mood Prefixes (Dictionary: mood > short prefix)
        private readonly Dictionary<string, string> _moodPrefixes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried",     "I understand you're worried. Here's what you need to know: "  },
            { "scared",      "It's okay to feel that way. Here's some reassurance: "        },
            { "confused",    "No worries, let me break this down simply: "                  },
            { "frustrated",  "I hear you. Let's work through this together: "               },
            { "curious",     "Great question! Here's what you need to know: "               },
            { "happy",       "Love the positive energy! Here's some info: "                 },
            { "excited",     "Awesome! Here's what you're looking for: "                    },
            { "angry",       "Let's address this calmly. Here's the information: "          },
            { "nervous",     "Take a breath. Here's some helpful information: "             },
            { "anxious",     "You're in safe hands. Here's what to know: "                  },
            { "unsure",      "Let me guide you through this: "                              },
            { "bored",       "Let's make this interesting! Here's something useful: "       },
            { "overwhelmed", "Let's keep it simple. Here's the key point: "                 },
            { "confident",   "Great mindset! Here's the detail you're after: "              },
            { "stressed",    "Stay calm. Here's exactly what you need: "                    },
        };

        // Random Response Pools (Dictionary: topic > List of responses)
        private readonly Dictionary<string, List<string>> _randomResponses =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "phishing", new List<string>
                {
                    "Be cautious of emails asking for personal information. Scammers disguise themselves as trusted organisations.",
                    "Always check the sender's email address carefully. Scammers use addresses that look almost right but have small differences.",
                    "Hover over links before clicking to see the real URL. If it looks suspicious, don't click!",
                    "Phishing emails often create urgency: 'Your account will be closed!' Pause and verify before acting.",
                    "When in doubt, contact the organisation directly using a number from their official website."
                }
            },
            { "malware", new List<string>
                {
                    "Keep your antivirus updated and run regular scans to catch malware before it causes damage.",
                    "Never download software from untrusted sources. Always use the official website or app store.",
                    "Malware often hides in email attachments. Don't open files from people you don't know.",
                    "Pop-ups saying 'Your computer is infected!' are often scams themselves. Close them immediately.",
                    "Enable real-time protection on your antivirus for continuous monitoring of your system."
                }
            },
            { "password", new List<string>
                {
                    "Use at least 12 characters mixing uppercase, lowercase, numbers, and symbols for strong passwords.",
                    "Never reuse the same password across multiple sites. A breach on one site shouldn't compromise others.",
                    "Consider using a passphrase: three random words combined are memorable and hard to crack.",
                    "A password manager generates and stores strong unique passwords so you only need to remember one.",
                    "Change passwords immediately if you suspect a breach or receive a suspicious activity notification."
                }
            },
            { "scam", new List<string>
                {
                    "If an offer sounds too good to be true, it almost certainly is. Trust your instincts.",
                    "Scammers often pretend to be tech support or government agencies. Legitimate organisations don't ask for gift card payments.",
                    "Never share OTPs or verification codes with anyone, not even someone claiming to be from your bank.",
                    "Romance scams are on the rise. Be wary of online contacts who quickly ask for money.",
                    "Check for scam reports at websites like Scamwatch before sending money or personal information."
                }
            },
            { "privacy", new List<string>
                {
                    "Review your social media privacy settings regularly. Platforms often change defaults after updates.",
                    "Limit the personal information you share publicly online. Scammers use this to craft targeted attacks.",
                    "Read privacy policies, especially for apps that access your camera, microphone, or location.",
                    "Use private browsing mode when you don't want your history stored locally.",
                    "Consider a VPN to encrypt your browsing and prevent your ISP from tracking your activity."
                }
            },
            { "ransomware", new List<string>
                {
                    "Back up your data regularly using the 3-2-1 rule: 3 copies, 2 different media, 1 offsite backup.",
                    "Never pay the ransom. It does not guarantee your files will be returned and funds criminal activity.",
                    "Disconnect from the internet immediately if you suspect ransomware to limit its spread.",
                    "Keep your OS and software updated. Ransomware often exploits known, patchable vulnerabilities.",
                    "Email is the most common delivery method. Think before opening attachments or clicking links."
                }
            },
        };

        // NLP Intent Map (POE Task 3)
        // Dictionary: phrase > action token. Allows flexible natural language.
        private readonly Dictionary<string, string> _nlpIntentMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Task intents
            { "add a task",                 "OPEN_TASKS" },
            { "add task",                   "OPEN_TASKS" },
            { "create a task",              "OPEN_TASKS" },
            { "new task",                   "OPEN_TASKS" },
            { "set a task",                 "OPEN_TASKS" },
            { "manage tasks",               "OPEN_TASKS" },
            { "view tasks",                 "OPEN_TASKS" },
            { "show tasks",                 "OPEN_TASKS" },
            { "my tasks",                   "OPEN_TASKS" },
            { "open task manager",          "OPEN_TASKS" },
            { "task manager",               "OPEN_TASKS" },

            // Reminder intents
            { "remind me to",               "CHAT_TASK"  },
            { "remind me about",            "CHAT_TASK"  },
            { "set a reminder",             "OPEN_TASKS" },
            { "add a reminder",             "OPEN_TASKS" },
            { "remind me",                  "OPEN_TASKS" },

            // Quiz intents
            { "start quiz",                 "LAUNCH_QUIZ" },
            { "take a quiz",                "LAUNCH_QUIZ" },
            { "begin quiz",                 "LAUNCH_QUIZ" },
            { "play quiz",                  "LAUNCH_QUIZ" },
            { "test my knowledge",          "LAUNCH_QUIZ" },
            { "test myself",                "LAUNCH_QUIZ" },
            { "take the quiz",              "LAUNCH_QUIZ" },
            { "start the quiz",             "LAUNCH_QUIZ" },
            { "play the game",              "LAUNCH_QUIZ" },
            { "start the game",             "LAUNCH_QUIZ" },
            { "mini game",                  "LAUNCH_QUIZ" },
            { "minigame",                   "LAUNCH_QUIZ" },

            // Activity log intents
            { "show activity log",          "SHOW_LOG" },
            { "activity log",               "SHOW_LOG" },
            { "what have you done",         "SHOW_LOG" },
            { "what have you done for me",  "SHOW_LOG" },
            { "show log",                   "SHOW_LOG" },
            { "recent actions",             "SHOW_LOG" },
            { "view log",                   "SHOW_LOG" },
            { "action log",                 "SHOW_LOG" },

            // Password NLP
            { "update my password",         "password"  },
            { "change my password",         "password"  },
            { "forgot my password",         "password"  },
            { "reset my password",          "password"  },
            { "strong password",            "password"  },
            { "create a password",          "password"  },
            { "make a password",            "password"  },

            // Phishing NLP
            { "suspicious email",           "Phishing"  },
            { "fake email",                 "Phishing"  },
            { "got a weird email",          "Phishing"  },
            { "email scam",                 "Phishing"  },
            { "clicked a link",             "Phishing"  },
            { "phishing attack",            "Phishing"  },

            // Privacy NLP
            { "protect my privacy",         "Privacy"   },
            { "my data",                    "Privacy"   },
            { "being tracked",              "Privacy"   },
            { "data privacy",               "Privacy"   },

            // 2FA NLP
            { "enable 2fa",                 "2FA"       },
            { "set up 2fa",                 "2FA"       },
            { "two factor",                 "2FA"       },
            { "authentication app",         "2FA"       },
            { "two-factor",                 "2FA"       },

            // VPN NLP
            { "should i use a vpn",         "VPN"       },
            { "what is a vpn",              "VPN"       },
            { "use a vpn",                  "VPN"       },
            { "public wifi safe",           "WiFi"      },
            { "public wi-fi",               "WiFi"      },

            // Malware NLP
            { "my computer has a virus",    "Malware"   },
            { "i think i have malware",     "Malware"   },
            { "computer infected",          "Malware"   },
            { "got a virus",                "Malware"   },

            // Backup NLP
            { "back up my data",            "Backup"    },
            { "backup my files",            "Backup"    },
            { "lost my files",              "Backup"    },
        };

        // Follow-up Phrases (List)
        private readonly List<string> _followUpPhrases = new List<string>
        {
            "tell me more", "more", "another tip", "give me another tip",
            "explain more", "go on", "continue", "what else", "keep going",
            "and then", "more info", "elaborate", "expand on that",
        };

        // Fallback Responses (List, rotated for variety)
        private readonly List<string> _fallbacks;
        private int _fallbackIndex = 0;

        // Constructor
        public GUIChatEngine(Action<string> appendBot, Action<string> appendSystem)
        {
            _appendBot = appendBot;
            _appendSystem = appendSystem;

            _fallbacks = new List<string>
            {
                "I'm not sure about that, {name}. Try typing 'topics' to see what I can help with, or 'help' for commands.",
                "I didn't quite catch that. Could you rephrase? You can also say 'add a task', 'start quiz', or ask about phishing, passwords, malware and more!",
                "That's outside what I know about, {name}. Type 'topics' to browse all cybersecurity subjects I cover.",
                "I'm not sure I understand. Try asking about a cybersecurity topic, or type 'help' to see all available commands.",
            };
        }

        public void SetState(ChatState state) => State = state;

        // PUBLIC: Log external actions into the activity log
        public void LogActivity(string description)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            _activityLog.Add((timestamp, description));
            if (_activityLog.Count > 50)
                _activityLog.RemoveAt(0);
        }

        // MAIN PROCESS
        public string Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't catch that. Try typing 'help' for a list of commands.";

            // State: collect name
            if (State == ChatState.AwaitingName)
            {
                UserName = input.Trim().Length > 0 ? CapitaliseName(input.Trim()) : "User";
                State = ChatState.AwaitingFavTopic;
                return "Nice to meet you, " + UserName + "!\n\n" +
                       "What is your favourite cybersecurity topic? (e.g. phishing, passwords, privacy)\n" +
                       "This helps me personalise your experience!";
            }

            // State: collect favourite topic
            if (State == ChatState.AwaitingFavTopic)
            {
                FavouriteTopic = input.Trim();
                State = ChatState.Active;
                LogHistory(UserName, input);
                string topicResponse = GetRandomResponse(FavouriteTopic);
                string reply =
                    "Great choice, " + UserName + "! I'll remember that you're interested in " + FavouriteTopic + ".\n\n" +
                    "Here's something about " + FavouriteTopic + " to get us started:\n" + topicResponse + "\n\n" +
                    "Type 'help' to see all commands, or try natural language:\n" +
                    "  'Add a task', 'Remind me to update my password', 'Start the quiz', 'Show activity log'";
                LogHistory("Bot", reply);
                LogActivity("Session started. User: " + UserName + ", favourite topic: " + FavouriteTopic);
                return reply;
            }

            // State: conversational task-adding flow
            if (State == ChatState.AwaitingTaskTitle)
            {
                _pendingTaskTitle = input.Trim();
                State = ChatState.AwaitingTaskDesc;
                LogHistory(UserName, input);
                string r = "Got it! Now give me a brief description for the task '" + _pendingTaskTitle + "'.";
                LogHistory("Bot", r);
                return r;
            }

            if (State == ChatState.AwaitingTaskDesc)
            {
                _pendingTaskDesc = input.Trim();
                State = ChatState.AwaitingTaskReminder;
                LogHistory(UserName, input);
                string r = "Would you like a reminder for this task?\n\n" +
                           "Type one of these options:\n" +
                           "  'in 1 day'  |  'in 3 days'  |  'in 1 week'\n" +
                           "  'in 2 weeks'  |  'in 1 month'  |  'no reminder'";
                LogHistory("Bot", r);
                return r;
            }

            if (State == ChatState.AwaitingTaskReminder)
            {
                string reminderInput = input.Trim().ToLower();

                // Timeframe dictionary: phrase > days
                var timeframeDays = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "in 1 day",    1  },
                    { "in 3 days",   3  },
                    { "in 1 week",   7  },
                    { "in 2 weeks",  14 },
                    { "in 1 month",  30 },
                    { "1 day",       1  },
                    { "3 days",      3  },
                    { "1 week",      7  },
                    { "2 weeks",     14 },
                    { "1 month",     30 },
                };

                string? reminderDate = null;
                string reminderDisplay = "no reminder";

                if (reminderInput != "no reminder" && reminderInput != "no" && reminderInput != "none" && reminderInput != "skip")
                {
                    foreach (var tf in timeframeDays)
                    {
                        if (reminderInput.Contains(tf.Key.ToLower()))
                        {
                            DateTime dt = DateTime.Today.AddDays(tf.Value);
                            reminderDate = dt.ToString("yyyy-MM-dd");
                            reminderDisplay = tf.Key + " (" + dt.ToString("dd MMM yyyy") + ")";
                            break;
                        }
                    }
                }

                State = ChatState.Active;
                LogHistory(UserName, input);

                try
                {
                    int id = DatabaseHelper.AddTask(_pendingTaskTitle, _pendingTaskDesc, reminderDate);
                    string logMsg = reminderDate != null
                        ? "Task added: '" + _pendingTaskTitle + "' | Reminder: " + reminderDisplay
                        : "Task added: '" + _pendingTaskTitle + "' | No reminder";

                    LogActivity(logMsg);

                    string r = "Task added successfully!\n\n" +
                               "  Title:       " + _pendingTaskTitle + "\n" +
                               "  Description: " + _pendingTaskDesc + "\n" +
                               "  Reminder:    " + reminderDisplay + "\n\n" +
                               "I'll keep track of that for you, " + UserName + ". " +
                               "You can view all tasks by clicking 'Task Manager' or typing 'tasks'.";

                    LogHistory("Bot", r);
                    _pendingTaskTitle = string.Empty;
                    _pendingTaskDesc = string.Empty;
                    return r;
                }
                catch (Exception ex)
                {
                    State = ChatState.Active;
                    string r = "Sorry " + UserName + ", I couldn't save the task: " + ex.Message;
                    LogHistory("Bot", r);
                    return r;
                }
            }

            // Active chat
            LogHistory(UserName, input);
            string response = ProcessActive(input);
            LogHistory("Bot", response);
            return response;
        }

        // ACTIVE PROCESSING
        private string ProcessActive(string input)
        {
            string lower = input.ToLower().Trim();

            // 1. Topic number selection
            if (int.TryParse(input.Trim(), out int topicNum))
            {
                if (_topicNumberMap.TryGetValue(topicNum, out string? key))
                {
                    _lastTopic = key;
                    LogActivity("Topic selected by number: '" + key + "'");
                    return ResponseBank.KeywordResponses[key];
                }
                return _topicNumberMap.Count == 0
                    ? "Type 'topics' first to see the numbered list, then enter a number."
                    : "Please enter a number between 1 and " + _topicNumberMap.Count + ".";
            }

            // 2. NLP Intent Detection (POE Task 3) - checked BEFORE commands
            string? nlpToken = DetectNlpIntent(lower);
            if (nlpToken != null)
            {
                // Chat-driven task flow
                if (nlpToken == "CHAT_TASK")
                {
                    // Extract task hint from "remind me to X"
                    string hint = input;
                    foreach (var phrase in new[] { "remind me to", "remind me about" })
                    {
                        int idx = lower.IndexOf(phrase);
                        if (idx >= 0)
                        {
                            hint = input.Substring(idx + phrase.Length).Trim();
                            break;
                        }
                    }
                    _pendingTaskTitle = CapitaliseName(hint.Length > 0 ? hint : "New Task");
                    State = ChatState.AwaitingTaskDesc;
                    LogActivity("NLP: conversational task flow started for '" + _pendingTaskTitle + "'");
                    return "Sure! I'll set up a task called '" + _pendingTaskTitle + "'.\n\n" +
                           "Can you give me a brief description for this task?";
                }

                // Keyword topic match
                if (ResponseBank.KeywordResponses.ContainsKey(nlpToken))
                {
                    _lastTopic = nlpToken;
                    LogActivity("NLP matched topic: '" + nlpToken + "' from: '" + input + "'");
                    return ResponseBank.KeywordResponses[nlpToken];
                }

                // Action token (OPEN_TASKS, LAUNCH_QUIZ, SHOW_LOG)
                LogActivity("NLP matched intent: '" + nlpToken + "' from: '" + input + "'");
                return nlpToken;
            }

            // 3. Direct commands
            if (ResponseBank.Commands.ContainsKey(lower))
                return HandleCommand(lower);

            // 4. Follow-up
            if (_followUpPhrases.Any(p => lower.Contains(p)))
                return HandleFollowUp();

            // 5. Mood detection
            string mood = DetectMood(lower);

            // 6. Memory recall
            if (lower.Contains("my favourite") || lower.Contains("what do i like") ||
                lower.Contains("remember me") || lower.Contains("what do you know about me"))
                return HandleMemoryRecall();

            // 7. Conversational responses
            foreach (var entry in ResponseBank.ConversationalResponses)
                if (lower.Contains(entry.Key.ToLower()))
                    return entry.Value;

            // 8. Pure mood (no topic)
            bool hasTopic = HasTopicKeyword(lower);
            if (!string.IsNullOrEmpty(mood) && !hasTopic)
                return GetMoodOnlyResponse(mood);

            string prefix = !string.IsNullOrEmpty(mood) ? GetMoodPrefix(mood) : string.Empty;

            // 9. Random response pool
            foreach (var topic in _randomResponses.Keys)
            {
                if (lower.Contains(topic.ToLower()))
                {
                    _lastTopic = topic;
                    string rand = GetRandomResponse(topic);
                    _lastResponse = rand;
                    LogActivity("Topic discussed: '" + topic + "'");
                    return prefix + rand + GetPersonalisedSuffix(topic);
                }
            }

            // 10. Keyword bank
            foreach (var entry in ResponseBank.KeywordResponses)
            {
                if (lower.Contains(entry.Key.ToLower()))
                {
                    _lastTopic = entry.Key;
                    _lastResponse = entry.Value;
                    LogActivity("Topic discussed: '" + entry.Key + "'");
                    return prefix + entry.Value + GetPersonalisedSuffix(entry.Key);
                }
            }

            // 11. Fallback
            return string.IsNullOrEmpty(mood) ? GetFallback() : GetMoodOnlyResponse(mood);
        }

        // NLP INTENT DETECTION (POE Task 3)
        private string? DetectNlpIntent(string lower)
        {
            foreach (var entry in _nlpIntentMap)
                if (lower.Contains(entry.Key.ToLower()))
                    return entry.Value;
            return null;
        }

        // ACTIVITY LOG (POE Task 4)
        public string GetActivityLog()
        {
            if (_activityLog.Count == 0)
                return "No actions recorded yet, " + UserName + ". " +
                       "Start chatting, add tasks, or take the quiz!";

            var recent = _activityLog.AsEnumerable().Reverse().Take(10).ToList();
            var lines = new List<string> { "Here is a summary of recent actions, " + UserName + ":\n" };

            int count = 1;
            foreach (var (timestamp, desc) in recent)
            {
                lines.Add("  " + count + ". [" + timestamp + "] " + desc);
                count++;
            }

            if (_activityLog.Count > 10)
                lines.Add("\n  ... and " + (_activityLog.Count - 10) + " earlier actions.");

            return string.Join("\n", lines);
        }

        // COMMAND HANDLERS
        private string HandleCommand(string command)
        {
            var handlers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "help",    ShowHelp    },
                { "topics",  ShowTopics  },
                { "tip",     ShowNextTip },
                { "quiz",    () => "LAUNCH_QUIZ" },
                { "tasks",   () => "OPEN_TASKS"  },
                { "log",     () => { LogActivity("User viewed activity log via command"); return "SHOW_LOG"; } },
                { "history", ShowHistory  },
                { "clear",   ClearHistory },
                { "exit",    () => "EXIT" },
            };

            return handlers.TryGetValue(command, out Func<string>? handler)
                ? handler()
                : "Unknown command. Type 'help' to see available commands.";
        }

        private string ShowHelp()
        {
            var lines = new List<string> { "Here are all available commands:\n" };
            foreach (var entry in ResponseBank.Commands)
                lines.Add("  * " + entry.Key.PadRight(14) + " - " + entry.Value);
            lines.Add("\nNatural language also works! Try:");
            lines.Add("  'Add a task to enable 2FA'");
            lines.Add("  'Remind me to update my password'");
            lines.Add("  'Start the quiz'  |  'Show activity log'");
            lines.Add("  'What is phishing?'  |  'How do I stay safe online?'");
            return string.Join("\n", lines);
        }

        private string ShowTopics()
        {
            _topicNumberMap.Clear();
            var lines = new List<string> { "You can ask me about the following topics:\n" };
            int count = 1;
            foreach (var key in ResponseBank.KeywordResponses.Keys)
            {
                _topicNumberMap[count] = key;
                lines.Add("  " + count.ToString().PadLeft(3) + ". " + key);
                count++;
            }
            lines.Add("\nType a number to select a topic, or just ask a question!");
            return string.Join("\n", lines);
        }

        private string ShowNextTip()
        {
            var tips = ResponseBank.SecurityTips.Values.ToList();
            string tip = tips[_tipIndex % tips.Count];
            _tipIndex++;
            LogActivity("Security tip #" + _tipIndex + " shown");
            return "Security Tip #" + _tipIndex + ":\n" + tip;
        }

        private string ShowHistory()
        {
            if (_conversationHistory.Count == 0)
                return "No conversation history yet.";
            var lines = new List<string> { "--- Conversation History ---\n" };
            foreach (var entry in _conversationHistory)
                lines.Add("  " + entry.Key + ": " + entry.Value);
            lines.Add("\n--- End of History ---");
            return string.Join("\n", lines);
        }

        private string ClearHistory()
        {
            _conversationHistory.Clear();
            _historyIndex = 1;
            LogActivity("Conversation history cleared");
            return "Conversation history cleared.";
        }

        // FOLLOW-UP
        private string HandleFollowUp()
        {
            if (string.IsNullOrEmpty(_lastTopic))
                return "Sure, " + UserName + "! What topic would you like to know more about? Type 'topics' to see all options.";

            if (_randomResponses.ContainsKey(_lastTopic))
            {
                string next = GetRandomResponse(_lastTopic);
                _lastResponse = next;
                return "Here's another tip about " + _lastTopic + ", " + UserName + ":\n\n" + next;
            }

            if (ResponseBank.KeywordResponses.TryGetValue(_lastTopic, out string? kw))
                return "Here's more on " + _lastTopic + ":\n\n" + kw;

            return "I've shared everything I know about '" + _lastTopic + "'. Type 'topics' to explore another subject!";
        }

        // MEMORY RECALL
        private string HandleMemoryRecall()
        {
            if (string.IsNullOrEmpty(FavouriteTopic))
                return "I know your name is " + UserName + ", but I don't have a favourite topic stored yet. What cybersecurity topic interests you most?";

            string rand = GetRandomResponse(FavouriteTopic);
            return "As someone interested in " + FavouriteTopic + ", " + UserName + ", here's something relevant:\n\n" + rand +
                   "\n\nYou might also want to review security settings related to " + FavouriteTopic + ".";
        }

        // MOOD HELPERS
        private string DetectMood(string input)
        {
            foreach (var entry in _moodKeywords)
                if (input.Contains(entry.Key.ToLower()))
                    return entry.Value;
            return string.Empty;
        }

        private string GetMoodOnlyResponse(string mood)
        {
            if (_moodOnlyResponses.TryGetValue(mood, out string? response))
                return response.Replace("{name}", UserName);
            return "I can sense how you're feeling, " + UserName + ". I'm here to help. What would you like to know about cybersecurity today?";
        }

        private string GetMoodPrefix(string mood)
        {
            return _moodPrefixes.TryGetValue(mood, out string? prefix) ? prefix : string.Empty;
        }

        private bool HasTopicKeyword(string lower)
        {
            foreach (var topic in _randomResponses.Keys)
                if (lower.Contains(topic.ToLower())) return true;
            foreach (var entry in ResponseBank.KeywordResponses)
                if (lower.Contains(entry.Key.ToLower())) return true;
            return false;
        }

        // GENERAL HELPERS
        private string GetRandomResponse(string topic)
        {
            foreach (var key in _randomResponses.Keys)
            {
                if (topic.ToLower().Contains(key.ToLower()) || key.ToLower().Contains(topic.ToLower()))
                {
                    var pool = _randomResponses[key];
                    return pool[new Random().Next(pool.Count)];
                }
            }
            foreach (var entry in ResponseBank.KeywordResponses)
                if (topic.ToLower().Contains(entry.Key.ToLower()))
                    return entry.Value;

            return "There's a lot to know about " + topic + "! Type 'topics' to browse all categories.";
        }

        private string GetPersonalisedSuffix(string topic)
        {
            if (!string.IsNullOrEmpty(FavouriteTopic) && topic.ToLower().Contains(FavouriteTopic.ToLower()))
                return "\n\nSince " + FavouriteTopic + " is your favourite topic, " + UserName + ", you might want to explore this further!";
            return string.Empty;
        }

        private string GetFallback()
        {
            string msg = _fallbacks[_fallbackIndex % _fallbacks.Count].Replace("{name}", UserName);
            _fallbackIndex++;
            return msg;
        }

        private void LogHistory(string speaker, string message)
        {
            _conversationHistory["[" + _historyIndex++ + "] " + speaker] = message;
        }

        private static string CapitaliseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }
    }
}
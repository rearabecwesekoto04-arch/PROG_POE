namespace CybersecurityChatbot.Data
{
    /// <summary>
    /// Stores all quiz questions grouped into 4 rounds of 5 questions each = 20 total.
    /// Mix of multiple-choice and true/false formats.
    /// POE Task 2: Cybersecurity Mini-Game (Quiz) with GUI.
    /// </summary>
    public static class QuizBank
    {
        // Round 1: Cybersecurity Basics 
        public static readonly Dictionary<int, QuizQuestion> Round1 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question      = "What does HTTPS stand for?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "HyperText Transfer Protocol Secure"       },
                        { "B", "High Transfer Text Protocol System"        },
                        { "C", "Hyperlink Text Transmission Protocol"      },
                        { "D", "HyperText Transmission Privacy Standard"  },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "HTTPS encrypts data between your browser and the server using SSL/TLS, keeping your data safe in transit.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 2, new QuizQuestion
                {
                    Question      = "Which of the following is a sign of a phishing email?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "It comes from a known contact"                      },
                        { "B", "It asks you to verify your password urgently"       },
                        { "C", "It has a personalised greeting with your full name" },
                        { "D", "It contains a company logo"                         },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Urgency and requests for credentials are the most classic phishing red flags. Legitimate companies never ask for your password via email.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 3, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: A password like '123456' is considered a strong password.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. '123456' is one of the most commonly used and easily cracked passwords in the world. Always use 12+ mixed characters.",
                    QuestionType  = "true-false"
                }
            },
            { 4, new QuizQuestion
                {
                    Question      = "What should you do FIRST if you receive a ransomware attack?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Pay the ransom immediately"                },
                        { "B", "Disconnect from the network and report it" },
                        { "C", "Delete all your files"                     },
                        { "D", "Restart your computer repeatedly"          },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Disconnecting from the network immediately limits the spread. Paying the ransom does NOT guarantee file recovery and funds criminals.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 5, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: Two-Factor Authentication (2FA) makes your accounts significantly more secure.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "TRUE. 2FA adds a second layer of security beyond your password, dramatically reducing account takeover risk even if your password is stolen.",
                    QuestionType  = "true-false"
                }
            },
        };

        // Round 2: Threats & Attacks 
        public static readonly Dictionary<int, QuizQuestion> Round2 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question      = "What is a VPN primarily used for?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Speeding up your internet connection"                        },
                        { "B", "Encrypting your internet traffic and hiding your IP address" },
                        { "C", "Blocking ads on websites"                                    },
                        { "D", "Scanning for viruses on your device"                         },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "A VPN (Virtual Private Network) encrypts your connection and hides your IP, protecting your activity from hackers and ISPs.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 2, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: Malware can only be installed by manually downloading a file.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. Malware can spread through drive-by downloads, email attachments, infected USB drives, and network vulnerabilities — no manual download required.",
                    QuestionType  = "true-false"
                }
            },
            { 3, new QuizQuestion
                {
                    Question      = "Which of the following is NOT a type of social engineering attack?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Phishing"   },
                        { "B", "Pretexting" },
                        { "C", "Encryption" },
                        { "D", "Baiting"    },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "Encryption is a security defence tool, not an attack. Phishing, pretexting, and baiting are all social engineering tactics used by attackers.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 4, new QuizQuestion
                {
                    Question      = "What does malware stand for?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Malicious Software"        },
                        { "B", "Managed Learning Ware"     },
                        { "C", "Multiple Application Ware" },
                        { "D", "Manual Learning Software"  },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "Malware is short for malicious software — any software intentionally designed to cause damage to a computer, server, or network.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 5, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: You should pay a ransom demand to recover your files after a ransomware attack.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. Paying the ransom funds criminal activity and does NOT guarantee files will be returned. Disconnect, report to authorities, and restore from backups.",
                    QuestionType  = "true-false"
                }
            },
        };

        // Round 3: Privacy & Best Practices
        public static readonly Dictionary<int, QuizQuestion> Round3 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question      = "What does SQL Injection allow an attacker to do?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Steal Wi-Fi passwords"                                          },
                        { "B", "Insert malicious code into database queries to manipulate data" },
                        { "C", "Slow down a website with traffic"                               },
                        { "D", "Encrypt files on a server"                                      },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "SQL Injection exploits poorly coded input fields to manipulate or expose database content — a major web application vulnerability.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 2, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: Public Wi-Fi networks are always safe to use for online banking.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. Public Wi-Fi is often unencrypted and vulnerable to eavesdropping attacks. Always use a VPN on public networks for sensitive activities.",
                    QuestionType  = "true-false"
                }
            },
            { 3, new QuizQuestion
                {
                    Question      = "Which is the safest way to connect to public Wi-Fi?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Use it freely — public Wi-Fi is always safe" },
                        { "B", "Only visit HTTP websites"                     },
                        { "C", "Use a VPN to encrypt your connection"         },
                        { "D", "Share your hotspot instead"                   },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "A VPN encrypts all your traffic on public Wi-Fi, preventing hackers from intercepting your data through man-in-the-middle attacks.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 4, new QuizQuestion
                {
                    Question      = "What does the principle of least privilege mean?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Giving all users admin access for convenience"              },
                        { "B", "Users only receive the minimum access needed for their job" },
                        { "C", "Limiting internet access to management only"                },
                        { "D", "Deleting unused accounts every month"                       },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Least privilege limits damage if an account is compromised by ensuring users only have access to what they absolutely need.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 5, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: Regularly updating software is an important cybersecurity practice.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "A",
                    Explanation   = "TRUE. Software updates patch known security vulnerabilities. Attackers actively target outdated software — always install updates promptly.",
                    QuestionType  = "true-false"
                }
            },
        };

        // Round 4: Advanced Cybersecurity
        public static readonly Dictionary<int, QuizQuestion> Round4 =
            new Dictionary<int, QuizQuestion>
        {
            { 1, new QuizQuestion
                {
                    Question      = "What is a zero-day vulnerability?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "A vulnerability that has been patched for zero days"          },
                        { "B", "A flaw unknown to the software vendor with no available patch" },
                        { "C", "A virus that activates at midnight"                            },
                        { "D", "A security feature that expires after one day"                 },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "Zero-day flaws are unknown to the vendor and have no patch available, making them extremely dangerous and highly valuable to attackers.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 2, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: A firewall alone is enough to fully protect a network from all cyber attacks.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. A firewall is one layer of defence but cannot block all threats. Security requires multiple layers: firewalls, antivirus, 2FA, patching, and user awareness.",
                    QuestionType  = "true-false"
                }
            },
            { 3, new QuizQuestion
                {
                    Question      = "What is the 3-2-1 backup rule?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "3 passwords, 2 devices, 1 cloud account"                     },
                        { "B", "3 copies of data, 2 different media types, 1 offsite backup" },
                        { "C", "Back up every 3 days, 2 times a day, 1 hour each"            },
                        { "D", "3 users, 2 admins, 1 backup manager"                         },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "The 3-2-1 rule ensures your data survives hardware failure, ransomware, or natural disasters by keeping multiple copies in different locations.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 4, new QuizQuestion
                {
                    Question      = "What should you do FIRST if you suspect your account has been hacked?",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "Wait and see if anything happens"               },
                        { "B", "Delete your account immediately"                 },
                        { "C", "Change your password and enable 2FA immediately" },
                        { "D", "Post about it on social media"                   },
                    },
                    CorrectAnswer = "C",
                    Explanation   = "Changing your password and enabling 2FA immediately cuts off the attacker's access. Then check for suspicious activity and notify affected services.",
                    QuestionType  = "multiple-choice"
                }
            },
            { 5, new QuizQuestion
                {
                    Question      = "TRUE or FALSE: Social engineering attacks target technical vulnerabilities in software.",
                    Options       = new Dictionary<string, string>
                    {
                        { "A", "True"  },
                        { "B", "False" },
                    },
                    CorrectAnswer = "B",
                    Explanation   = "FALSE. Social engineering attacks target HUMAN vulnerabilities — trust, fear, urgency, and curiosity — to manipulate people into revealing information or taking actions.",
                    QuestionType  = "true-false"
                }
            },
        };

        // All Rounds
        public static readonly List<Dictionary<int, QuizQuestion>> AllRounds =
            new List<Dictionary<int, QuizQuestion>> { Round1, Round2, Round3, Round4 };

        // Round Titles
        public static readonly List<string> RoundTitles = new List<string>
        {
            "Round 1 — Cybersecurity Basics",
            "Round 2 — Threats & Attacks",
            "Round 3 — Privacy & Best Practices",
            "Round 4 — Advanced Cybersecurity",
        };
    }

    /// <summary>Represents a single quiz question with options and the correct answer.</summary>
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public Dictionary<string, string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "multiple-choice";
    }
}
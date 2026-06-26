# CyberBot — Cybersecurity Awareness Chatbot
### PROG POE Part 3 | The Independent Institute of Education (IIE)

---

## 📌 Project Overview

CyberBot is a Windows Forms (WinForms) Cybersecurity Awareness Chatbot built with C# and .NET 8. 
It educates users on cybersecurity topics through natural conversation, a task assistant, 
a quiz mini-game, NLP simulation, and an activity log. 
All tasks are stored in a MySQL database.

---

## 🎥 Video Presentation

📺 YouTube Link: https://youtu.be/YOUR_LINK_HERE

---

## ⚙️ Setup Instructions

### Prerequisites
- Windows 10 or 11
- Visual Studio 2022
- .NET 8 SDK
- MySQL Server 8.0+
- MySQL Workbench

### Step 1 — Clone the Repository

git clone https://github.com/YOUR_USERNAME/PROG_POE.git

### Step 2 — Set Up the MySQL Database

1. Open MySQL Workbench
2. Connect to Local Instance MySQL80
3. Run the following SQL:

CREATE DATABASE PROG_POE;

USE PROG_POE;

CREATE TABLE Tasks (
    Id           INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Title        VARCHAR(255) NOT NULL,
    Description  TEXT         NOT NULL,
    IsComplete   TINYINT(1)   NOT NULL DEFAULT 0,
    ReminderDate VARCHAR(20)  NULL,
    CreatedAt    VARCHAR(30)  NOT NULL
);

### Step 3 — Configure Database Password

Open DatabaseHelper.cs and update your MySQL password:

private static readonly string ConnectionString =
    "server=localhost;" +
    "database=PROG_POE;" +
    "uid=root;" +
    "pwd=YOUR_PASSWORD;";

### Step 4 — Install NuGet Package

In Visual Studio, open Package Manager Console and run:

Install-Package MySql.Data

### Step 5 — Run the Application

Press F5 in Visual Studio or click Debug → Start Debugging.

---

## 🚀 Features & Usage

### 💬 Chat Commands

| Command   | Description                      |
|-----------|----------------------------------|
| help      | Show all available commands      |
| topics    | List all cybersecurity topics    |
| tip       | Get a random security tip        |
| quiz      | Start the cybersecurity quiz     |
| tasks     | Open the Task Manager            |
| log       | View the activity log            |
| history   | View conversation history        |
| clear     | Clear conversation history       |
| exit      | Exit the chatbot                 |

### 🗣️ Natural Language Examples

You can also type naturally:
- "Add a task to enable two-factor authentication"
- "Remind me to update my password"
- "Start the quiz"
- "Show activity log"
- "What is phishing?"
- "How do I stay safe online?"

---

## ✅ Task 1 — Task Assistant with Reminders

- Add cybersecurity tasks with a title and description
- Set reminders by specific date or timeframe (1 day, 1 week, etc.)
- View, complete, and delete tasks
- All tasks saved and loaded from MySQL database

Example Interaction:

User: Add a task
Bot:  What is the title of your task?
User: Enable 2FA
Bot:  Give me a brief description...
User: Set up two-factor authentication on all accounts
Bot:  Would you like a reminder?
User: In 3 days
Bot:  Task added successfully!
      Title:       Enable 2FA
      Description: Set up two-factor authentication on all accounts
      Reminder:    In 3 days (28 Jun 2026)

---

## 🎮 Task 2 — Cybersecurity Quiz

- 20 questions across 4 rounds
- Mix of multiple-choice and true/false formats
- Immediate feedback with explanation after each answer
- Round scores and final score with personalised feedback

Rounds:
  Round 1 — Cybersecurity Basics
  Round 2 — Threats and Attacks
  Round 3 — Privacy and Best Practices
  Round 4 — Advanced Cybersecurity

Example Question:

Q: What should you do FIRST if you receive a ransomware attack?
A) Pay the ransom immediately
B) Disconnect from the network and report it
C) Delete all your files
D) Restart your computer repeatedly

Correct Answer: B
Feedback: Disconnecting from the network immediately limits the spread.

Score Feedback:
  20/20 — PERFECT SCORE! You are a Cybersecurity Expert!
  16-19 — Outstanding! You really know your stuff!
  12-15 — Great work! Keep building your knowledge.
   8-11 — Not bad, consider brushing up on the topics you missed.
   0-7  — Keep learning! Cybersecurity knowledge keeps you safe.

---

## 🧠 Task 3 — NLP Simulation

- Detects keywords and intent from natural language input
- Understands varied phrasing of the same request
- Mood detection with personalised empathetic responses
- Supports 30+ NLP intent phrases
- Fallback responses when input is unclear

Example NLP Phrases Recognised:
  "remind me to update my password"   → Opens task flow
  "I think I have malware"            → Malware advice
  "should I use a VPN?"               → VPN information
  "I got a weird email"               → Phishing advice
  "I feel worried about my security"  → Mood response

---

## 📋 Task 4 — Activity Log

- Automatically records all key actions with timestamps
- Displays the last 10 actions when requested
- Triggered by typing "show activity log" or clicking Activity Log button

Example Log Output:

Here is a summary of recent actions:

  1. [14:05:32] Session started. User: Tlholo, favourite topic: Phishing
  2. [14:06:10] Task added: 'Enable 2FA' | Reminder: In 3 days (28 Jun 2026)
  3. [14:07:45] Quiz started
  4. [14:08:30] Quiz completed
  5. [14:09:12] Activity log viewed
  6. [14:10:05] Topic discussed: 'password'
  7. [14:11:22] Security tip #1 shown
  8. [14:12:00] Task Manager opened
  9. [14:12:45] Task completed: 'Enable 2FA'
  10.[14:13:10] Conversation history cleared

---

## 🗂️ Project Structure

PROG_POE/
├── ChatEngine.cs        — Console chat engine (Part 1)
├── ChatForm.cs          — Main WinForms GUI (Part 3)
├── DatabaseHelper.cs    — MySQL database operations (CRUD)
├── DisplayHelper.cs     — Console display and formatting (Part 1)
├── GUIChatEngine.cs     — GUI engine with NLP and Activity Log
├── Program.cs           — Application entry point
├── QuizBank.cs          — 20 quiz questions across 4 rounds
├── QuizEngine.cs        — Console quiz engine (Part 1)
├── QuizForm.cs          — Quiz WinForms GUI (Part 3)
├── ResponseBank.cs      — All chatbot responses and keywords
└── TaskForm.cs          — Task Manager WinForms GUI (Part 3)

---

## 🛠️ Technologies Used

- Language:   C# (.NET 8)
- GUI:        Windows Forms (WinForms)
- Database:   MySQL 8.0
- Connector:  MySql.Data (NuGet Package)
- Speech:     System.Speech.Synthesis (Text-to-Speech)
- IDE:        Visual Studio 2022

---

## 🗄️ Database Schema

Database: PROG_POE
Table: Tasks

| Column       | Type         | Description                    |
|--------------|--------------|--------------------------------|
| Id           | INT (PK)     | Auto-increment primary key     |
| Title        | VARCHAR(255) | Task title                     |
| Description  | TEXT         | Task description               |
| IsComplete   | TINYINT(1)   | 0 = Pending, 1 = Complete      |
| ReminderDate | VARCHAR(20)  | Reminder date (yyyy-MM-dd)     |
| CreatedAt    | VARCHAR(30)  | Date and time task was created |

---

## 📝 Submission Details

- Institution:  The Independent Institute of Education (IIE)
- Module:       PROG — Programming
- Submission:   POE Part 3
- Student:      Tlholo
- GitHub:       https://github.com/YOUR_USERNAME/PROG_POE
- Video:        https://youtu.be/YOUR_LINK_HERE

---

## 🔒 Security Note

The database connection string in DatabaseHelper.cs contains credentials.
Do not share your actual password publicly on GitHub.
Consider using environment variables for production applications.

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CybersecurityChatbot.Data
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            "server=localhost;" +
            "database=PROG_POE;" +
            "uid=root;" +
            "pwd=@Tlhologelo2001;";

        // INITIALISE
        public static void Initialise()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string createTable = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id           INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Title        VARCHAR(255) NOT NULL,
                        Description  TEXT         NOT NULL,
                        IsComplete   TINYINT(1)   NOT NULL DEFAULT 0,
                        ReminderDate VARCHAR(20)  NULL,
                        CreatedAt    VARCHAR(30)  NOT NULL
                    );";

                MySqlCommand cmd = new MySqlCommand(createTable, connection);
                cmd.ExecuteNonQuery();
            }
        }

        // CREATE
        public static int AddTask(string title, string description, string? reminderDate = null)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string insert = @"
                    INSERT INTO Tasks (Title, Description, IsComplete, ReminderDate, CreatedAt)
                    VALUES (@title, @desc, 0, @reminder, @created);
                    SELECT LAST_INSERT_ID();";

                MySqlCommand cmd = new MySqlCommand(insert, connection);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", (object?)reminderDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // READ
        public static List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string select = "SELECT Id, Title, Description, IsComplete, ReminderDate, CreatedAt FROM Tasks ORDER BY Id DESC;";

                MySqlCommand cmd = new MySqlCommand(select, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Title = reader["Title"].ToString() ?? "",
                        Description = reader["Description"].ToString() ?? "",
                        IsComplete = Convert.ToInt32(reader["IsComplete"]) == 1,
                        ReminderDate = reader["ReminderDate"] == DBNull.Value ? null : reader["ReminderDate"].ToString(),
                        CreatedAt = reader["CreatedAt"].ToString() ?? "",
                    });
                }
            }

            return tasks;
        }

        // UPDATE
        public static void MarkComplete(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string update = "UPDATE Tasks SET IsComplete = 1 WHERE Id = @id;";
                MySqlCommand cmd = new MySqlCommand(update, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // DELETE
        public static void DeleteTask(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string delete = "DELETE FROM Tasks WHERE Id = @id;";
                MySqlCommand cmd = new MySqlCommand(delete, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // MODEL
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string? ReminderDate { get; set; }
        public string CreatedAt { get; set; } = string.Empty;

        public string StatusDisplay => IsComplete ? "✔ Done" : "⏳ Pending";
        public string ReminderDisplay =>
            string.IsNullOrWhiteSpace(ReminderDate) ? "No reminder" : $"📅 {ReminderDate}";
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EmployeeCRUD
{
    public class DataQueryLog
    {
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public static class DataQueryLogger
    {
        private static readonly List<DataQueryLog> _logs = new List<DataQueryLog>();
        private static readonly int MaxLogs = 100;
        private static readonly string _logFilePath;

        static DataQueryLogger()
        {
            string dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EmployeeCRUD"
            );
            _logFilePath = Path.Combine(dataDir, "query_logs.json");
            LoadLogs();
        }

        public static void Log(string operation, string entity, string details, string filePath, bool success = true)
        {
            var log = new DataQueryLog
            {
                Timestamp = DateTime.Now,
                Operation = operation,
                Entity = entity,
                Details = details,
                FilePath = filePath,
                Success = success
            };

            _logs.Insert(0, log); // Add to beginning for newest first

            // Keep only the last MaxLogs entries
            if (_logs.Count > MaxLogs)
            {
                _logs.RemoveAt(_logs.Count - 1);
            }

            SaveLogs();
        }

        public static List<DataQueryLog> GetLogs(int count = 50)
        {
            return _logs.Take(count).ToList();
        }

        public static void ClearLogs()
        {
            _logs.Clear();
            SaveLogs();
        }

        private static void LoadLogs()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    string json = File.ReadAllText(_logFilePath);
                    var logs = JsonSerializer.Deserialize<List<DataQueryLog>>(json);
                    if (logs != null)
                    {
                        _logs.Clear();
                        _logs.AddRange(logs.Take(MaxLogs));
                    }
                }
            }
            catch
            {
                // Ignore load errors
            }
        }

        private static void SaveLogs()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_logs, options);
                File.WriteAllText(_logFilePath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        public static string FormatLogEntry(DataQueryLog log)
        {
            return $"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] {log.Operation} - {log.Entity}: {log.Details}";
        }
    }
}

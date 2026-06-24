using System;
using System.Collections.Generic;

namespace Comidasa.Services
{
    public static class EmailTracker
    {
        private static readonly List<SentEmailDto> _sentEmails = new List<SentEmailDto>();
        private static readonly object _lock = new object();

        public static List<SentEmailDto> GetSentEmails()
        {
            lock (_lock)
            {
                return new List<SentEmailDto>(_sentEmails);
            }
        }

        public static void AddEmail(string to, string subject, string body)
        {
            lock (_lock)
            {
                _sentEmails.Insert(0, new SentEmailDto
                {
                    To = to,
                    Subject = subject,
                    Body = body,
                    Timestamp = DateTime.Now
                });

                // Mantener solo los últimos 20 correos
                if (_sentEmails.Count > 20)
                {
                    _sentEmails.RemoveAt(_sentEmails.Count - 1);
                }
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _sentEmails.Clear();
            }
        }
    }

    public class SentEmailDto
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}

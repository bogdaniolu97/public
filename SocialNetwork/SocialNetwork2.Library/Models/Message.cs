﻿using System;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Library.Models
{
    public class Message
    {
        public Message(string text)
        {
            this.text = text;
            timestamp = Sys.Time();
        }

        public override string ToString()
        {
            var elapsed = Formatted(Sys.Time() - timestamp);
            return $"{text} ({elapsed})";
        }

        //

        private readonly string text;
        private readonly DateTime timestamp;

        private static string Formatted(TimeSpan ts)
        {
            var minutes = (int) Math.Truncate(ts.TotalMinutes);
            var seconds = (int) Math.Truncate(ts.TotalSeconds);

            var duration = minutes > 0 ? minutes : seconds;
            var unit = minutes > 0 ? "minute" : "second";

            var suffix = duration == 1 ? "" : "s";

            return $"{duration} {unit}{suffix} ago";
        }
    }
}
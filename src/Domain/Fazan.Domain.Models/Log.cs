// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Marian Spoiala">
// Copyright (c) Marian Spoiala
// 
// <copyright>
// <summary>
// Defines the GuardianApiClientSpecs type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Microsoft.Extensions.Logging;

namespace Fazan.Domain.Models
{
    public class Log
    {

        private Log(string message, LogLevel logLevel = LogLevel.Information)
        {
            Message = message;
            LogLevel = logLevel;
        }

        public string Message { get; }

        public LogLevel LogLevel { get; }

        public static Log Create(string message, LogLevel logLevel = LogLevel.Information) => new Log(message, logLevel);

        public override string ToString() => $"{DateTime.Now:G}|{Enum.GetName(typeof(LogLevel), LogLevel)}|{Message}";
    }
}
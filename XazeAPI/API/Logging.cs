using UnityEngine;

namespace XazeAPI.API
{
    using LabApi.Features.Console;
    using System;
    using System.Reflection;

    public static class Logging
    {
        public static void Info(string message)
        {
            Logger.Raw(Logger.FormatLog(message, Logger.InfoPrefix, Assembly.GetCallingAssembly()), System.ConsoleColor.Cyan);
        }

        public static void Debug(string message, bool canBeSend = true)
        {
            if (!canBeSend)
            {
                return;
            }

            Logger.Raw(Logger.FormatLog(message, Logger.DebugPrefix, Assembly.GetCallingAssembly()), System.ConsoleColor.DarkMagenta);
        }

        public static void Warn(string message)
        {
            Logger.Raw(Logger.FormatLog(message, Logger.WarnPrefix, Assembly.GetCallingAssembly()), System.ConsoleColor.Yellow);
        }

        public static void Error(string message)
        {
            Logger.Raw(Logger.FormatLog(message, Logger.ErrorPrefix, Assembly.GetCallingAssembly()), System.ConsoleColor.Red);
        }

        public static void ServerLog(string message, ConsoleColor color)
        {
            Logger.Raw($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", color);
        }
    }
}

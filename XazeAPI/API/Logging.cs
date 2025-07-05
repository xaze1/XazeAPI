// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace XazeAPI.API
{
    using LabApi.Features.Console;
    using System;
    using System.Reflection;

    public static class Logging
    {
        public static void Info(string message)
        {
            Raw(FormatLog(message, Logger.InfoPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Cyan);
        }

        public static void Debug(string message, bool canBeSend = true)
        {
            if (!canBeSend)
            {
                return;
            }

            Raw(FormatLog(message, Logger.DebugPrefix, Assembly.GetCallingAssembly()), ConsoleColor.DarkMagenta);
        }

        public static void Warn(string message)
        {
            Raw(FormatLog(message, Logger.WarnPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Yellow);
        }

        public static void Error(string message)
        {
            Raw(FormatLog(message, Logger.ErrorPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Red);
        }

        public static void ServerLog(string message, ConsoleColor color)
        {
            Raw($"[{FormatAssemblyName(Assembly.GetCallingAssembly())}] {message}", color);
        }
        
        public static string FormatLog(object message, string prefix, Assembly assembly)
        {
            return $"[{prefix}] [{FormatAssemblyName(assembly)}] {message}";
        }

        public static string FormatAssemblyName(Assembly assembly) => assembly.GetName().Name;
        
        public static void Raw(string message, ConsoleColor color)
        {
            ServerConsole.AddLog(message, color);
        }
    }
}

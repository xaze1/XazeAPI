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
        public static void Info(params object[] args)
        {
            Raw(FormatLog(string.Join(" ", args), Logger.InfoPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Cyan);
        }

        public static void Debug(bool canBeSend, params object[] args)
        {
            if (!canBeSend || args.Length == 0)
            {
                return;
            }

            Raw(FormatLog(string.Join(" ", args), Logger.DebugPrefix, Assembly.GetCallingAssembly()), ConsoleColor.DarkMagenta);
        }

        public static void Debug(params object[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            Raw(FormatLog(string.Join(" ", args), Logger.DebugPrefix, Assembly.GetCallingAssembly()), ConsoleColor.DarkMagenta);
        }

        public static void Warn(params object[] args)
        {
            Raw(FormatLog(string.Join(" ", args), Logger.WarnPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Yellow);
        }

        public static void Error(params object[] args)
        {
            Raw(FormatLog(string.Join(" ", args), Logger.ErrorPrefix, Assembly.GetCallingAssembly()), ConsoleColor.Red);
        }

        public static void ServerLog(ConsoleColor color, params object[] args)
        {
            Raw($"[{FormatAssemblyName(Assembly.GetCallingAssembly())}] {string.Join(" ", args)}", color);
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

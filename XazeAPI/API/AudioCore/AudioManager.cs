// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.IO;
using System.Reflection;
using LabApi.Loader.Features.Paths;

namespace XazeAPI.API.AudioCore;

public static class AudioManager
{
    public static string AudioPath { get; private set; } = Path.Combine(Path.Combine(PathManager.LabApi.FullName, "XazeAPI"), "Audio");

    public static void AddAudio(Assembly audioAssembly, bool sendDebug = false)
    {
        Directory.CreateDirectory(AudioPath);
        Logging.Debug(sendDebug, $"Resources: {audioAssembly.GetManifestResourceNames().Length}");

        foreach(var resource in audioAssembly.GetManifestResourceNames())
        {
            if (!resource.EndsWith(".ogg"))
                continue;

            Logging.Debug(sendDebug, $"Looking at {resource}");

            int lastDotIndex = resource.LastIndexOf('.');
            int secondLastDotIndex = resource.LastIndexOf('.', lastDotIndex - 1);

            string fileName = resource.Substring(secondLastDotIndex + 1);
            string path = Path.Combine(AudioPath, fileName);
            if (File.Exists(path))
            {
                continue;
            }

            using var resourceStream = audioAssembly.GetManifestResourceStream(resource);
            using var file = File.Open(path, FileMode.Create);
            resourceStream?.CopyTo(file);

            Logging.Debug(sendDebug, $"Extracted {fileName} to {path}");
        }

        Logging.Info("Audio loaded for " + audioAssembly.GetName().Name);
    }
}
// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

namespace XazeAPI.API.Extensions;

public static class GeneralExtensions
{
    public static float GetPercentage(this float max, double percentage)
    {
        return (float)(max * (percentage / 100f));
    }
    
    public static double GetPercentage(this double max, double percentage)
    {
        return max * (percentage / 100f);
    }
    
    public static int GetPercentage(this int max, double percentage)
    {
        return (int)(max * (percentage / 100f));
    }
    
    public static uint GetPercentage(this uint max, double percentage)
    {
        return (uint)(max * (percentage / 100f));
    }
    
    public static short GetPercentage(this short max, double percentage)
    {
        return (short)(max * (percentage / 100f));
    }
    
    public static ushort GetPercentage(this ushort max, double percentage)
    {
        return (ushort)(max * (percentage / 100f));
    }
}
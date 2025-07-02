namespace XazeAPI.API.Extensions
{
    using System.Drawing;

    public static class ColorExtensions
    {
        public static string ToHex(this Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}

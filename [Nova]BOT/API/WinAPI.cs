using System;
using System.Runtime.InteropServices;

namespace NovaBOT
{
    internal class WinAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int AnimateWindow(IntPtr hwand, int dwTime, int dwFlag);

        public const int HOR_Positive = 1;

        public const int HOR_NEGATIVE = 2;

        public const int VER_POSITIVE = 4;

        public const int VER_NEGATIVE = 8;

        public const int CENTER = 16;

        public const int BLEND = 524288;

        internal const int STD_OUTPUT_HANDLE = -11;
        internal const int TMPF_TRUETYPE = 4;
        internal const int LF_FACESIZE = 32;
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int dwType);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal readonly uint nFont;
            internal COORD dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal readonly short X;
            internal readonly short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }
    }
}

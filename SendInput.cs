using System.Runtime.InteropServices;

    /// <summary>
    /// Declaration of external SendInput method
    /// </summary>


    // Declare the INPUT struct
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        internal uint type;
        internal InputUnion U;
        internal static int Size
        {
            get { return Marshal.SizeOf(typeof(INPUT)); }
        }
    }

    // Declare the InputUnion struct
    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        [FieldOffset(0)]
        internal MOUSEINPUT mi;
        [FieldOffset(0)]
        internal KEYBDINPUT ki;
        [FieldOffset(0)]
        internal HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        internal int dx;
        internal int dy;
        internal MouseEventDataXButtons mouseData;
        internal MOUSEEVENTF dwFlags;
        internal uint time;
        internal UIntPtr dwExtraInfo;
    }

    [Flags]
    internal enum MouseEventDataXButtons : uint
    {
        Nothing = 0x00000000,
        XBUTTON1 = 0x00000001,
        XBUTTON2 = 0x00000002
    }

    [Flags]
    internal enum MOUSEEVENTF : uint
    {
        ABSOLUTE = 0x8000,
        HWHEEL = 0x01000,
        MOVE = 0x0001,
        MOVE_NOCOALESCE = 0x2000,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        VIRTUALDESK = 0x4000,
        WHEEL = 0x0800,
        XDOWN = 0x0080,
        XUP = 0x0100
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        internal short wVk;
        internal ScanCodeShort wScan;
        internal KEYEVENTF dwFlags;
        internal int time;
        internal UIntPtr dwExtraInfo;
    }

    [Flags]
    internal enum KEYEVENTF : uint
    {
        EXTENDEDKEY = 0x0001,
        KEYUP = 0x0002,
        SCANCODE = 0x0008,
        UNICODE = 0x0004
    }

    internal enum ScanCodeShort : short
    {
        KEY_LBUTTON = 0,
        KEY_RBUTTON = 0,
        KEY_CANCEL = 70,
        KEY_MBUTTON = 0,
        KEY_XBUTTON1 = 0,
        KEY_XBUTTON2 = 0,
        KEY_BACK = 14,
        KEY_TAB = 15,
        KEY_CLEAR = 76,
        KEY_RETURN = 28,
        KEY_SHIFT = 42,
        KEY_CONTROL = 29,
        KEY_MENU = 56,
        KEY_PAUSE = 0,
        KEY_CAPITAL = 58,
        KEY_KANA = 0,
        KEY_HANGUL = 0,
        KEY_JUNJA = 0,
        KEY_FINAL = 0,
        KEY_HANJA = 0,
        KEY_KANJI = 0,
        KEY_ESCAPE = 1,
        KEY_CONVERT = 0,
        KEY_NONCONVERT = 0,
        KEY_ACCEPT = 0,
        KEY_MODECHANGE = 0,
        KEY_SPACE = 57,
        KEY_PRIOR = 73,
        KEY_NEXT = 81,
        KEY_END = 79,
        KEY_HOME = 71,
        KEY_LEFT = 75,
        KEY_UP = 72,
        KEY_RIGHT = 77,
        KEY_DOWN = 80,
        KEY_SELECT = 0,
        KEY_PRINT = 0,
        KEY_EXECUTE = 0,
        KEY_SNAPSHOT = 84,
        KEY_INSERT = 82,
        KEY_DELETE = 83,
        KEY_HELP = 99,
        KEY_0 = 11,
        KEY_1 = 2,
        KEY_2 = 3,
        KEY_3 = 4,
        KEY_4 = 5,
        KEY_5 = 6,
        KEY_6 = 7,
        KEY_7 = 8,
        KEY_8 = 9,
        KEY_9 = 10,
        KEY_A = 30,
        KEY_B = 48,
        KEY_C = 46,
        KEY_D = 32,
        KEY_E = 18,
        KEY_F = 33,
        KEY_G = 34,
        KEY_H = 35,
        KEY_I = 23,
        KEY_J = 36,
        KEY_K = 37,
        KEY_L = 38,
        KEY_M = 50,
        KEY_N = 49,
        KEY_O = 24,
        KEY_P = 25,
        KEY_Q = 16,
        KEY_R = 19,
        KEY_S = 31,
        KEY_T = 20,
        KEY_U = 22,
        KEY_V = 47,
        KEY_W = 17,
        KEY_X = 45,
        KEY_Y = 21,
        KEY_Z = 44,
        KEY_LWIN = 91,
        KEY_RWIN = 92,
        KEY_APPS = 93,
        KEY_SLEEP = 95,
        KEY_NUMPAD0 = 82,
        KEY_NUMPAD1 = 79,
        KEY_NUMPAD2 = 80,
        KEY_NUMPAD3 = 81,
        KEY_NUMPAD4 = 75,
        KEY_NUMPAD5 = 76,
        KEY_NUMPAD6 = 77,
        KEY_NUMPAD7 = 71,
        KEY_NUMPAD8 = 72,
        KEY_NUMPAD9 = 73,
        KEY_MULTIPLY = 55,
        KEY_ADD = 78,
        KEY_SEPARATOR = 0,
        KEY_SUBTRACT = 74,
        KEY_DECIMAL = 83,
        KEY_DIVIDE = 53,
        KEY_F1 = 59,
        KEY_F2 = 60,
        KEY_F3 = 61,
        KEY_F4 = 62,
        KEY_F5 = 63,
        KEY_F6 = 64,
        KEY_F7 = 65,
        KEY_F8 = 66,
        KEY_F9 = 67,
        KEY_F10 = 68,
        KEY_F11 = 87,
        KEY_F12 = 88,
        KEY_F13 = 100,
        KEY_F14 = 101,
        KEY_F15 = 102,
        KEY_F16 = 103,
        KEY_F17 = 104,
        KEY_F18 = 105,
        KEY_F19 = 106,
        KEY_F20 = 107,
        KEY_F21 = 108,
        KEY_F22 = 109,
        KEY_F23 = 110,
        KEY_F24 = 118,
        KEY_NUMLOCK = 69,
        KEY_SCROLL = 70,
        KEY_LSHIFT = 42,
        KEY_RSHIFT = 54,
        KEY_LCONTROL = 29,
        KEY_RCONTROL = 29,
        KEY_LMENU = 56,
        KEY_RMENU = 56,
        KEY_BROWSER_BACK = 106,
        KEY_BROWSER_FORWARD = 105,
        KEY_BROWSER_REFRESH = 103,
        KEY_BROWSER_STOP = 104,
        KEY_BROWSER_SEARCH = 101,
        KEY_BROWSER_FAVORITES = 102,
        KEY_BROWSER_HOME = 50,
        KEY_VOLUME_MUTE = 32,
        KEY_VOLUME_DOWN = 46,
        KEY_VOLUME_UP = 48,
        KEY_MEDIA_NEXT_TRACK = 25,
        KEY_MEDIA_PREV_TRACK = 16,
        KEY_MEDIA_STOP = 36,
        KEY_MEDIA_PLAY_PAUSE = 34,
        KEY_LAUNCH_MAIL = 108,
        KEY_LAUNCH_MEDIA_SELECT = 109,
        KEY_LAUNCH_APP1 = 107,
        KEY_LAUNCH_APP2 = 33,
        KEY_OEM_1 = 39,
        KEY_OEM_PLUS = 13,
        KEY_OEM_COMMA = 51,
        KEY_OEM_MINUS = 12,
        KEY_OEM_PERIOD = 52,
        KEY_OEM_2 = 53,
        KEY_OEM_3 = 41,
        KEY_OEM_4 = 26,
        KEY_OEM_5 = 43,
        KEY_OEM_6 = 27,
        KEY_OEM_7 = 40,
        KEY_OEM_8 = 0,
        KEY_OEM_102 = 86,
        KEY_PROCESSKEY = 0,
        KEY_PACKET = 0,
        KEY_ATTN = 0,
        KEY_CRSEL = 0,
        KEY_EXSEL = 0,
        KEY_EREOF = 93,
        KEY_PLAY = 0,
        KEY_ZOOM = 98,
        KEY_NONAME = 0,
        KEY_PA1 = 0,
        KEY_OEM_CLEAR = 0,
    }

    /// <summary>
    /// Define HARDWAREINPUT struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }

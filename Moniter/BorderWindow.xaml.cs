using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Moniter
{
    public enum Edge { Top, Bottom, Left, Right }

    public partial class BorderWindow : Window
    {
        private DispatcherTimer blinkTimer;
        private bool dim = false;

        public BorderWindow(Edge edge)
        {
            InitializeComponent();
            SetupWindow(edge);
            MakeWindowClickThrough();
            StartBlinking();
        }

        private void SetupWindow(Edge edge)
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Red;
            Topmost = true;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;

            Width = (edge == Edge.Left || edge == Edge.Right) ? 40 : SystemParameters.PrimaryScreenWidth;
            Height = (edge == Edge.Top || edge == Edge.Bottom) ? 40 : SystemParameters.PrimaryScreenHeight;

            Left = (edge == Edge.Right) ? SystemParameters.PrimaryScreenWidth - 40 : 0;
            Top = (edge == Edge.Bottom) ? SystemParameters.PrimaryScreenHeight - 40 : 0;

            Opacity = 0.3;
        }

        private void StartBlinking()
        {
            blinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            blinkTimer.Tick += (s, e) =>
            {
                Opacity = dim ? 0.1 : 0.5;
                dim = !dim;
            };
            blinkTimer.Start();
        }

        // 鼠标穿透
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;
        const int LWA_ALPHA = 0x2;

        private void MakeWindowClickThrough()
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Moniter
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer coinTimer;
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
            MakeWindowClickThrough();        // 实现点击穿透
            StartBorderFlashing();           // 边框红色闪烁
            StartCoinDrop();                 // 掉金币（可删）
        }

        // 🔴 红边闪烁动画
        private void StartBorderFlashing()
        {
            DoubleAnimation blink = new DoubleAnimation
            {
                From = 0.1,
                To = 0.5,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            TopBorder.BeginAnimation(Rectangle.OpacityProperty, blink);
            BottomBorder.BeginAnimation(Rectangle.OpacityProperty, blink);
            LeftBorder.BeginAnimation(Rectangle.OpacityProperty, blink);
            RightBorder.BeginAnimation(Rectangle.OpacityProperty, blink);
        }

        // 💡 鼠标点击穿透窗口
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

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

        // ✅ 可选金币掉落动画（你也可以删除这部分）
        private void StartCoinDrop()
        {
            coinTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            coinTimer.Tick += (s, e) =>
            {
                DropOneCoin();
            };

            coinTimer.Start();
        }

        private void DropOneCoin()
        {
            Image coin = new Image
            {
                Width = 40,
                Height = 40,
                Source = new BitmapImage(new Uri("pack://application:,,,/Moniter;component/coin.png")),
                RenderTransform = new TranslateTransform()
            };

            double startX = rand.Next(0, (int)this.Width - 50);
            Canvas.SetLeft(coin, startX);
            Canvas.SetTop(coin, -50);
            CoinCanvas.Children.Add(coin);

            DoubleAnimation dropAnim = new DoubleAnimation
            {
                From = -50,
                To = this.Height + 50,
                Duration = TimeSpan.FromSeconds(3),
                FillBehavior = FillBehavior.Stop
            };

            dropAnim.Completed += (s, e) => CoinCanvas.Children.Remove(coin);
            coin.BeginAnimation(Canvas.TopProperty, dropAnim);
        }
    }
}

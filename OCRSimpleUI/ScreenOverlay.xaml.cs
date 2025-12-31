using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCRSimpleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScreenOverlay : Window
    {
        private System.Windows.Point startPoint;
        private System.Windows.Shapes.Rectangle selectionRect;
        private TaskCompletionSource<Bitmap> tcs;

        public ScreenOverlay()
        {
            InitializeComponent();
        }

        public Task<Bitmap> StartSnipAsync()
        {
            tcs = new TaskCompletionSource<Bitmap>();
            Show();
            return tcs.Task;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            startPoint = e.GetPosition(this);

            selectionRect = new System.Windows.Shapes.Rectangle()
            {
                Stroke = System.Windows.Media.Brushes.DeepSkyBlue,
                StrokeThickness = 2
            };

            OverlayCanvas.Children.Add(selectionRect);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (selectionRect == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var pos = e.GetPosition(this);

            double x = Math.Min(pos.X, startPoint.X);
            double y = Math.Min(pos.Y, startPoint.Y);
            double w = Math.Abs(pos.X - startPoint.X);
            double h = Math.Abs(pos.Y - startPoint.Y);

            Canvas.SetLeft(selectionRect, x);
            Canvas.SetTop(selectionRect, y);
            selectionRect.Width = w;
            selectionRect.Height = h;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            var pos = e.GetPosition(this);


            double x = Math.Min(pos.X, startPoint.X);
            double y = Math.Min(pos.Y, startPoint.Y);
            double w = Math.Abs(pos.X - startPoint.X);
            double h = Math.Abs(pos.Y - startPoint.Y);

            Hide();

            var bmp = CaptureRegion((int)x, (int)y, (int)w, (int)h);
            tcs.SetResult(bmp);

            Close();
        }

        private Bitmap CaptureRegion(int x, int y, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen((int)x, (int)y, 0, 0, new System.Drawing.Size(w, h));
            }

            return bmp;
        }
    }
}
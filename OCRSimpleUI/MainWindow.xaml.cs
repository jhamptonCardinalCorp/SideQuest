using OCRPlayground;
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
    public partial class MainWindow : Window
    {
        static PreprocessingOcrEngine ocrEngine = new PreprocessingOcrEngine(new TessaractOcrEngine());

        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Snip_Click(object sender, RoutedEventArgs e)
        {
            //var overlay = new SnippingOverlayWindow();
            var overlay = new ScreenOverlay();
            var bitmap = await overlay.StartSnipAsync();

            if (bitmap != null)
            {
                // Feed into your OCR engine
                var text = await ocrEngine.ExtractTextAsync(bitmap);
                OutputBox.Text = text;
            }
        }
    }
}
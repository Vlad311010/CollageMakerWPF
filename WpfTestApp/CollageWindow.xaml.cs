using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
using System.Xml.Linq;
using WpfTestApp.UserControls;

namespace WpfTestApp
{
    public partial class CollageWindow : Window
    {
        public CollageWindow()
        {
            InitializeComponent();
        }

        private double _editorWindowWidth;
        private double _editorWindowHeight;


        private void Button_ResizeCollage(object sender, RoutedEventArgs e)
        {
            int width;
            int height;
            bool isWidthParsed = int.TryParse(tbCollageWidthInput.Text, out width);
            bool isHeightParsed = int.TryParse(tbCollageHeightInput.Text, out height);
            if (!(isWidthParsed && IsValidWindowSizeValue(width)) || !(isHeightParsed && IsValidWindowSizeValue(height)))
            {
                MessageBox.Show($"Invalid collage size: {width}x{height}");
                return;
            }
            
            ResizeCollage(width, height);
        }

        private bool IsValidWindowSizeValue(int value)
        {
            return value > 0 && value < 6000;
        }

        private void Button_SaveCollage(object sender, RoutedEventArgs e)
        {
            SaveCollage();
        }
        public void ResizeCollage(int width, int height)
        {
            // set editor size.
            editorGrid.Resize(width, height);
        }

        private void SaveCollage()
        {
            UIElement target = editorGrid;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            const double dpi = 96d;

            double borderThickness = 0;
            Color borderColor = Colors.White;
            Pen borderPen = new Pen(new SolidColorBrush(borderColor), borderThickness);


            int width = (int)(bounds.Width);
            int height = (int)(bounds.Height);
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                // dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                dc.DrawRectangle(vb, borderPen, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            // TODO: show saving dialog
            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Collages\\a.png";
            using (Stream stm = File.Create(file))
            {
                png.Save(stm);
            }
            MessageBox.Show("Saved");


        }

    }
}
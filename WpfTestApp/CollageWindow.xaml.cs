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

        public void ResizeEditorWindow(double width, double height)
        {
            // set editor size.
            // editorGrid.Resize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveCollage();
        }

        private void SaveCollage()
        {
            // UIElement target = canvasEditor;
            UIElement target = editorGrid;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            double dpi = 96d;


            int width = (int)(bounds.Width);
            int height = (int)(bounds.Height);
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                // dc.DrawRectangle(vb, new Pen(new SolidColorBrush(Colors.Red), 15), new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Collages\\a.png";
            using (Stream stm = File.Create(file))
            {
                png.Save(stm);
            }
            MessageBox.Show("Saved");


        }

        
    }
}
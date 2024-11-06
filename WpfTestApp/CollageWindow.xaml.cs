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
using WpfTestApp.Utils;

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

        private void Button_ResizeGrid(object sender, RoutedEventArgs e)
        {
            int columns = 0;
            int rows = 0;
            bool isColumnsParsed = int.TryParse(tbCollageGridColumns.Text, out columns);
            bool isRowsParsed = int.TryParse(tbCollageGridRows.Text, out rows);
            if (isColumnsParsed && isRowsParsed && columns > 0 && columns * rows >= 1) // check if column and row >= 1
                editor.ResizeGrid(columns, rows);
            else
                MessageBox.Show("Invalid parametes. Grid must have atleast one coulumn and row");
        }

        public void ResizeCollage(int width, int height)
        {
            // set editor size.
            editor.Resize(width, height);
        }

        private void SaveCollage()
        {
            UIElement target = editor;
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
                dc.DrawRectangle(vb, borderPen, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            string defaultFolder = @".\Collages";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "MyCollage";
            #if DEBUG
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            #else
                saveFileDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            #endif
            saveFileDialog.Filter = "Images|*.png;*.jpg;*.jpeg;";
            if (saveFileDialog.ShowDialog() == true) 
            {
                using (Stream stm = File.Create(saveFileDialog.FileName))
                {
                    png.Save(stm);
                }
            }
        }

    }
}
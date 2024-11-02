using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfTestApp.Interfaces;
using WpfTestApp.Utils;

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for ImageContainer.xaml
    /// </summary>
    public partial class ImageContainer : UserControl, IToolbarFunctional
    {
        public ImageContainer()
        {
            InitializeComponent();
        }


        public string? Source
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                Uri fileUri = new Uri(_imageSource!);
                _imageBitmap = new BitmapImage(fileUri);
                img.Source = _imageBitmap;
                _horizontalScale = 1;
                _verticalScale = 1;
            }
        }

        public ScrollViewer ScrollViewer => imageScroll;
        public Image Image => img;

        private string? _imageSource;
        private BitmapImage? _imageBitmap;
        private Point _lastMousePos;

        // transform properties
        private int _horizontalScale = 1;
        private int _verticalScale = 1;

        private void DropHandle(object sender, DragEventArgs e)
        {
            string? filePath = e.Data.GetData(DataFormats.StringFormat) as string;
            if (filePath == null) 
                return;

            Source = filePath;
            ResetScroll();
        }

        private void ResetScroll()
        {
            imageScroll.ScrollToHorizontalOffset(0);
            imageScroll.ScrollToVerticalOffset(0);
        }


        private void imageScroll_MouseMove(object sender, MouseEventArgs e)
        {
            if (img.IsMouseCaptured)
            {
                // TODO: sensivity as propery
                const double sensivity = 1.2d;
                double scrollH = _lastMousePos.X - e.GetPosition(this).X;
                double scrollV = _lastMousePos.Y - e.GetPosition(this).Y;
                imageScroll.ScrollToHorizontalOffset(imageScroll.HorizontalOffset + scrollH * sensivity);
                imageScroll.ScrollToVerticalOffset(imageScroll.VerticalOffset + scrollV * sensivity);
            }
            _lastMousePos = e.GetPosition(this);
        }

        private void imageScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            img.CaptureMouse();
            _lastMousePos = e.GetPosition(this);
        }

        private void imageScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            img.ReleaseMouseCapture();
        }

        // test
        private void imageScroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                img.Stretch = img.Stretch.Increment();
            }
        }

        private void Rotate()
        {
            if (_imageBitmap == null)
                return;

            Uri fileUri = new Uri(_imageSource!);
            Rotation currentRotation = _imageBitmap.Rotation;
            _imageBitmap = new BitmapImage();
            _imageBitmap.BeginInit();
            _imageBitmap.UriSource = fileUri;
            _imageBitmap.Rotation = currentRotation.Increment();
            _imageBitmap.EndInit();
            UpdateImage(_imageBitmap);
        }

        private void FlipHorizontal()
        {
            _horizontalScale *= -1;
            var transform = new ScaleTransform(_horizontalScale, _verticalScale, 0, 0);
            TransformedBitmap tb = new TransformedBitmap(_imageBitmap, transform);
            UpdateImage(tb);
        }

        private void FlipVertical()
        {
            _verticalScale *= -1;
            var transform = new ScaleTransform(_horizontalScale, _verticalScale, 0, 0);
            TransformedBitmap tb = new TransformedBitmap(_imageBitmap, transform);
            UpdateImage(tb);

            // preserved on image change. mb change implementation or restore values on image change
            /*img.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform flipTrans = new ScaleTransform();
            _verticalScale *= -1;
            flipTrans.ScaleY = _verticalScale;
            img.RenderTransform = flipTrans;*/
        }

        private void UpdateImage(BitmapSource bitmap)
        {
            img.Source = bitmap;
        }

        public void CreateToolbarElements(EditorToolbar toolbar)
        {
            // toolbar.AddBtn(Grid.GetColumn(this) + " " + Grid.GetRow(this));
            toolbar.AddBtn("Rotate", Rotate);
            toolbar.AddBtn("Flip H", FlipHorizontal);
            toolbar.AddBtn("Flip V", FlipVertical);
            toolbar.AddSlider(0, 100, "Zoom:");
        }
    }
}

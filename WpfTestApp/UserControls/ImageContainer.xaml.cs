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
            img.RenderTransformOrigin = new Point(0.5, 0.5);
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
                SetImageSource(_imageBitmap);
            }
        }

        // public ScrollViewer ScrollViewer => imageScroll;
        public Image Image => img;

        private string? _imageSource;
        private BitmapImage? _imageBitmap;
        private Point _lastMousePos;

        // transform properties
        private int _horizontalScale = 1;
        private int _verticalScale = 1;
        private double _imgScale = 1;
        private float _imgAngle = 0;
        private Slider _scaleSlider;

        private void DropHandle(object sender, DragEventArgs e)
        {
            string? filePath = e.Data.GetData(DataFormats.StringFormat) as string;
            if (filePath == null) 
                return;

            Source = filePath;
            if (img.Height < img.Width)
                img.MaxWidth = imageCanvas.ActualWidth;
            else
                img.MaxHeight = imageCanvas.ActualHeight;
            ResetImageTransform();
        }

        private void ResetImageTransform()
        {
            _horizontalScale = 1;
            _verticalScale = 1;
            ResizeImg(1);
            CenterImage();
        }

        private void CenterImage()
        {
            // recalculate ActualWidth and ActualHeight 
            img.UpdateLayout();

            Canvas.SetLeft(img, (imageCanvas.ActualWidth - img.ActualWidth) / 2);
            Canvas.SetTop(img, (imageCanvas.ActualHeight - img.ActualHeight) / 2);
        }

        public void OnContainerResized(double hOffset, double vOffset)
        {
            Canvas.SetLeft(img, Canvas.GetLeft(img) + hOffset / 2);
            Canvas.SetTop(img, Canvas.GetTop(img) + vOffset / 2);
        }

        // move to distinct usercontrol?
        #region MouseScroll
        private void imageScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            img.CaptureMouse();
            _lastMousePos = e.GetPosition(this);
        }

        private void imageScroll_MouseMove(object sender, MouseEventArgs e)
        {
            if (img.IsMouseCaptured)
            {
                // TODO: sensivity as propery
                const double sensivity = 1.2d;
                double scrollH = _lastMousePos.X - e.GetPosition(this).X;
                double scrollV = _lastMousePos.Y - e.GetPosition(this).Y;
                double currentH = Canvas.GetLeft(img);
                double currentV = Canvas.GetTop(img);
                Canvas.SetLeft(img, currentH - scrollH * sensivity);
                Canvas.SetTop(img, currentV - scrollV * sensivity);
            }
            _lastMousePos = e.GetPosition(this);
        }

        private void imageScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            img.ReleaseMouseCapture();
        }

        #endregion

        // test
        private void imageScroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                ResizeImg(1);
            }
            else if (e.Key == Key.R)
            {
                ResizeImg(2);
            }
            else if (e.Key == Key.E)
            {
                Fill();
            }
        }

        private void Rotate()
        {
            imageRotateTransform.Angle = _imgAngle;
            _imgAngle += 90;
        }

        private void FlipHorizontal()
        {
            _horizontalScale *= -1;
            var transform = new ScaleTransform(_horizontalScale, _verticalScale, 0, 0);
            TransformedBitmap tb = new TransformedBitmap(_imageBitmap, transform);
            SetImageSource(tb);
        }

        private void FlipVertical()
        {
            _verticalScale *= -1;
            var transform = new ScaleTransform(_horizontalScale, _verticalScale, 0, 0);
            TransformedBitmap tb = new TransformedBitmap(_imageBitmap, transform);
            SetImageSource(tb);
        }

        private void Fill()
        {
            img.MaxWidth = imageCanvas.ActualWidth;
            img.MaxHeight = imageCanvas.MaxHeight;
            ResizeImg(1);
            CenterImage();
        }

        private void ResizeImg(double scale)
        {
            // TODO: use pixel size instead of scale???
            _imgScale = scale;
            imageScaleTransform.ScaleX = scale;
            imageScaleTransform.ScaleY = scale;
        }

        private void SetImageSource(BitmapSource bitmap)
        {
            img.Source = bitmap;
        }

        public void CreateToolbarElements(EditorToolbar toolbar)
        {
            toolbar.AddBtn("Center", CenterImage);
            toolbar.AddBtn("Fill", Fill);
            toolbar.AddBtn("Rotate", Rotate);
            toolbar.AddBtn("Flip H", FlipHorizontal);
            toolbar.AddBtn("Flip V", FlipVertical);
            toolbar.AddSlider(0.1d, 8d, _imgScale, "Zoom:", (oldValue, newValue) => ResizeImg(newValue));
        }
    }
}

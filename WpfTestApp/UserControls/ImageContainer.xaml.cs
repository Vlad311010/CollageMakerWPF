using System;
using System.Collections.Generic;
using System.IO;
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
            Loaded += ImageContainer_Loaded;
            img.RenderTransformOrigin = new Point(0.5, 0.5);

            // set default image
            Source = null;
            // SetImageSource(new BitmapImage(AppParameters.Instance.EditorParameters.DefaultImageUri));
        }

        private void ImageContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (MaskSource == null)
                mainCanvas.OpacityMask = null;
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
                Uri fileUri = _imageSource == null ? AppParameters.Instance.EditorParameters.DefaultImageUri : new Uri(_imageSource!);
                _imageBitmap = new BitmapImage(fileUri);
                SetImageSource(_imageBitmap);
            }
        }

        public string? MaskSource
        {
            get
            {
                return _maskSource;
            }
            set
            {
                _maskSource = value;
                if (opacityMask == null || _maskSource == null)
                    return;

                // Uri fileUri = new Uri(_maskSource!);
                Uri fileUri = new Uri($"pack://application:,,,/Masks/{_maskSource!}");
                try
                {
                    _maskBitmap = new BitmapImage(fileUri);
                    opacityMask.ImageSource = _maskBitmap;
                }
                catch (IOException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public Image Image => img;
        public BitmapImage? Mask => _maskBitmap;

        private string? _imageSource;
        private string? _maskSource;
        private BitmapImage? _imageBitmap;
        private BitmapImage? _maskBitmap;
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

            Canvas.SetLeft(img, (mainCanvas.ActualWidth - img.ActualWidth) / 2);
            Canvas.SetTop(img, (mainCanvas.ActualHeight - img.ActualHeight) / 2);
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

        public void Fill()
        {
            img.MaxWidth = mainCanvas.ActualWidth;
            img.MaxHeight = mainCanvas.MaxHeight;
            ResizeImg(1);
            CenterImage();
        }

        private void Clear()
        {
            Source = null;
            Fill();
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
            toolbar.AddBtn("Clear", Clear);
        }
    }
}

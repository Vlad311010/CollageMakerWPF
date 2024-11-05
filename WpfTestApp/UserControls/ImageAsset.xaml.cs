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

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for ImageAsset.xaml
    /// </summary>
    public partial class ImageAsset : UserControl
    {
        public ImageAsset()
        {
            InitializeComponent();
        }

        public string AssetSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                Uri fileUri = new Uri(_imageSource);
                _imageBitmap = new BitmapImage(fileUri);
                imgPreview.Source = _imageBitmap;
            }
        }
        public Stretch Stretch { set => imgPreview.Stretch = value; }

        public Action<object, MouseButtonEventArgs> OnRightClick { get; set; }
        
        private string _imageSource;

        private BitmapImage _imageBitmap;

        public void imgPreview_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnRightClick?.Invoke(sender, e);
        }

        private void imgPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ImageAsset? asset = sender as ImageAsset;
                if (asset == null) return;

                DragDrop.DoDragDrop(asset, new DataObject(DataFormats.StringFormat, asset.AssetSource),  DragDropEffects.Move);
            }

        }
    }
}

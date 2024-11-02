using Microsoft.Win32;
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

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for AssetsBox.xaml
    /// </summary>
    public partial class AssetsBox : UserControl
    {
        public AssetsBox()
        {
            // TODO: unsubscribe event on destroy?
            InitializeComponent();
        }

        // button dependency property
        public static readonly DependencyProperty AddBtnProperty = DependencyProperty.Register("AddBtn", typeof(Button), typeof(AssetsBox), new PropertyMetadata(null, OnBtnChanged));

        public Button AddBtn
        {
            get => (Button)GetValue(AddBtnProperty);
            set => SetValue(AddBtnProperty, value);
        }

        private static void OnBtnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssetsBox? assetsBox = d as AssetsBox;

            if (e.OldValue is Button oldButton)
            {
                // Unsubscribe from the Click event of the old button
                oldButton.Click -= assetsBox!.btnAddImage_Click;
            }

            if (e.NewValue is Button newButton)
            {
                // Subscribe to the Click event of the new button
                newButton.Click += assetsBox!.btnAddImage_Click;
            }
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.png;*.jpg;*.jpeg;";
            openFileDialog.Multiselect = true;


            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    CreateImageControl(file);

                    // scroll to last added
                    Border border = (Border)VisualTreeHelper.GetChild(lbImages, 0);
                    ScrollViewer scroll = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scroll.ScrollToHorizontalOffset(scroll.ActualWidth);
                }
            }
        }

        private void CreateImageControl(string filePath)
        {
            // create new asset control with specified image
            ImageAsset imgAsset = new ImageAsset();
            imgAsset.AssetSource = filePath;
            imgAsset.MinWidth = 90;
            imgAsset.MaxWidth = 120;
            imgAsset.MaxHeight = 120;
            imgAsset.Stretch = Stretch.Uniform;
            imgAsset.OnRightClick = ShowContextMenu;
            lbImages.Items.Insert(lbImages.Items.Count - 1, imgAsset);
        }

        private void RemoveAsset()
        {
            lbImages.Items.Remove(lbImages.SelectedItem);
        }

        private void ShowContextMenu(object sender, MouseButtonEventArgs e)
        {
            ContextMenu? cm = lbImages.FindResource("cmAsset") as ContextMenu;
            if (cm == null) return;

            cm.PlacementTarget = sender as Image;
            cm.IsOpen = true;
        }

        private void ClickRemove(object sender, RoutedEventArgs args) { RemoveAsset(); }
    }
}

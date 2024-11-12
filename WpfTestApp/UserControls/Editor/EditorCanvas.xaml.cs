using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfTestApp.DataStructs;
using WpfTestApp.Utils;
using YamlDotNet.Core;

namespace WpfTestApp.UserControls.Editor
{
    /// <summary>
    /// Interaction logic for EditorCanvas.xaml
    /// </summary>
    public partial class EditorCanvas : EditorBase
    {
        public EditorCanvas()
        {
            InitializeComponent();
            _editorPanel = editorCanvas;
            Loaded += OnEditorLoad;
            Unloaded += OnEditorUnload;
        }

        private ImageContainer[]? _containers;


        protected override void OnEditorLoad(object sender, RoutedEventArgs e)
        {
            base.OnEditorLoad(sender, e);
            AppParameters.Instance.EditorParameters.OnTemplateChange += OnTemplateChange;
        }


        protected override void OnEditorUnload(object sender, RoutedEventArgs e)
        {
            base.OnEditorUnload(sender, e);
            AppParameters.Instance.EditorParameters.OnTemplateChange -= OnTemplateChange;
        }

        protected override void UpdateEditor()
        {
            _editorPanel.Children.Clear();
            _editorPanel.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = this });
            _editorPanel.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = this });

            if (AppParameters.Instance.EditorParameters.SelectedTemplate == null)
            {
                _containers = null!;
                return;
            }

            _containers = new ImageContainer[AppParameters.Instance.EditorParameters.SelectedTemplate.Containers.Length];
            for (int i = 0; i < AppParameters.Instance.EditorParameters.SelectedTemplate.Containers.Length; i++)
            {
                ImageContainer imgContainer = CreateImageContainer(AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
                _editorPanel.Children.Add(imgContainer);
                _containers[i] = imgContainer;

                SetContrainerTransform(imgContainer, AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
                SetContainerMaskVieport(imgContainer, AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
            }
        }


        protected override ImageContainer CreateImageContainer(ContainerData containerData)
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnEditorElementLeftClick;
            imgContainer.MaskSource = containerData.MaskSource;
            imgContainer.AllowDrop = true;
            imgContainer.Padding = new Thickness(_borderSize);

            return imgContainer;
        }

        private void SetContrainerTransform(ImageContainer container, ContainerData containerData)
        {
            editorCanvas.UpdateLayout();
            Canvas.SetLeft(container, containerData.Left * editorCanvas.ActualWidth);
            Canvas.SetTop(container, containerData.Top * editorCanvas.ActualHeight);
            container.Width = containerData.Width * editorCanvas.ActualWidth;
            container.Height = containerData.Height * editorCanvas.ActualHeight;
        }

        private void SetContainerMaskVieport(ImageContainer container, ContainerData containerData)
        {
            editorCanvas.UpdateLayout();

            if (container.Mask == null)
                return;

            // double scaleX = container.ActualWidth / container.Mask.PixelWidth;
            // double scaleY = container.ActualHeight / container.Mask.PixelHeight;

            double scaleX = AppParameters.Instance.EditorParameters.Width / AppParameters.Instance.EditorParameters.DEFAULT_WIDTH;
            double scaleY = AppParameters.Instance.EditorParameters.Height / AppParameters.Instance.EditorParameters.DEFAULT_HEIGHT;

            double maskWidth = container.Mask.PixelWidth;
            double maskHeight = container.Mask.PixelHeight;

            container.opacityMask.Viewport = new Rect(0, 0, maskWidth, maskHeight);

            double maxXOffset = container.ActualWidth - container.opacityMask.Viewport.Width;
            double maxYOffset = container.ActualHeight - container.opacityMask.Viewport.Height;

            double xOffset = maxXOffset * containerData.MaskLeft;
            double yOffset = maxYOffset * containerData.MaskTop;

            container.opacityMask.Viewport = new Rect(xOffset, yOffset, container.opacityMask.Viewport.Width, container.opacityMask.Viewport.Height);
        }

        private double MapToRange(double t, double x, double y)
        {
            return x + t * (y - x);
        }

        protected override void Resize(int width, int height)
        {
            this.Width = AppParameters.Instance.EditorParameters.Width;
            this.Height = AppParameters.Instance.EditorParameters.Height;

            if (_containers == null)
                return;

            for (int i = 0; i < AppParameters.Instance.EditorParameters.SelectedTemplate!.Containers.Length; i++)
            {
                SetContrainerTransform(_containers[i], AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
                SetContainerMaskVieport(_containers[i], AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
            }

            foreach (ImageContainer imgContainer in _containers)
                imgContainer.Fill();
        }

        private void OnTemplateChange(CollageTemplate? template)
        {
            UpdateEditor();
        }

    }
}

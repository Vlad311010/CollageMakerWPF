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
            AppParameters.Instance.EditorParameters.OnTemplateChange += OnTemplateChange;
            Loaded += OnEditorLoad;
        }

        private ImageContainer[]? _containers;
        
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
                ImageContainer imgContainer = CreateGridElement(AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
                _editorPanel.Children.Add(imgContainer);
                _containers[i] = imgContainer;

                SetContrainerTransform(imgContainer, AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
                SetContainerMaskVieport(imgContainer, AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
            }

            // foreach (ImageContainer imgContainer in _containers)
                // imgContainer.Fill();
            
        }


        protected override ImageContainer CreateGridElement(ContainerData containerData)
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnEditorElementLeftClick;
            imgContainer.MaskSource = containerData.MaskSource;
            imgContainer.AllowDrop = true;
            imgContainer.Padding = new Thickness(_borderSize);

            // SetContrainerTransform(imgContainer, containerData);
            // SetContainerMaskVieport(imgContainer, containerData);
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
            // container.opacityMask.Viewport = new Rect(150, 0, 300, 300);
            editorCanvas.UpdateLayout();
            container.UpdateLayout();
            /*double left = containerData.MaskLeft * container.ActualWidth;
            double top = containerData.MaskTop * container.ActualHeight;
            double width = containerData.MaskWidth * container.ActualWidth;
            double height = containerData.MaskHeight * container.ActualHeight;

            double? maskImageWidth = container.Mask?.Width;
            double? maskImageHeight = container.Mask?.Height;

            if (maskImageWidth != null && maskImageHeight != null && container.ActualWidth > 0 && container.ActualHeight > 0)
            {
                double widthCoeficient = (double)maskImageWidth / width;
                double heightCoeficient = (double)maskImageHeight / height;
                
                double viewportWidth = width * widthCoeficient;
                double viewportHeight = height * heightCoeficient;


                double startOffsetX = -container.ActualWidth / 2 + (double)maskImageWidth / 2;
                double startOffsetY = -container.ActualHeight / 2 + (double)maskImageHeight / 2;

                double endOffsetX = +container.ActualWidth / 2 - (double)maskImageWidth / 2;
                double endOffsetY = +container.ActualHeight / 2 - (double)maskImageHeight / 2;

                double offsetX = MapToRange(containerData.MaskLeft, startOffsetX, endOffsetX);
                double offsetY = MapToRange(containerData.MaskTop, startOffsetY, endOffsetY);

                container.opacityMask.Viewport = new Rect(offsetX, offsetY, container.ActualWidth, container.ActualHeight);
                // container.opacityMask.Viewport = new Rect(containerData.MaskLeft * viewportWidth, containerData.MaskTop * heightCoeficient, viewportWidth, viewportHeight);
                // container.opacityMask.Viewport = new Rect(0, 0, viewportWidth, viewportHeight);
                // container.opacityMask.Viewport.Offset(600, containerData.MaskTop * viewportHeight);
            }*/

            /*double left = containerData.MaskLeft * container.ActualWidth;
            double top = containerData.MaskTop * container.ActualHeight;
            double width = containerData.MaskWidth * container.ActualWidth;
            double height = containerData.MaskHeight * container.ActualHeight;

            double? maskImageWidth = container.Mask?.Width;
            double? maskImageHeight = container.Mask?.Height;

            if (maskImageWidth != null && maskImageHeight != null && width > 0 && height > 0)
            {
                double widthCoeficient = (double)maskImageWidth / width;
                double heightCoeficient = (double)maskImageHeight / height;

                double viewportWidth = width * widthCoeficient;
                double viewportHeight = height * heightCoeficient;


                double maxXOffset = container.ActualWidth - container.opacityMask.Viewport.Width;
                double maxYOffset = container.ActualHeight - container.opacityMask.Viewport.Height;

                // Calculate the actual offset based on the percentage (0 to 1 range)
                double xOffset = maxXOffset * containerData.MaskLeft;
                double yOffset = maxYOffset * containerData.MaskTop;
                // double xOffset = maxXOffset * 0;
                // double yOffset = maxYOffset * 0;

                // container.opacityMask.Viewport = new Rect(xOffset, yOffset, container.ActualWidth, container.ActualHeight);
                container.opacityMask.Viewport = new Rect(xOffset, yOffset, container.opacityMask.Viewport.Width, container.opacityMask.Viewport.Height);
                // container.opacityMask.Viewport.Scale(widthCoeficient, heightCoeficient);

                // container.opacityMask.Viewbox.Offset(container.ActualWidth * 0.5, container.ActualHeight * 0.0);
                // container.opacityMask.Viewport = new (0, 0, viewportWidth, viewportHeight);

                
            }*/

            

            if (container.Mask == null)
                return;

            double maskWidth = container.Mask.PixelWidth;
            double maskHeight = container.Mask.PixelHeight;

            double scaleX = container.ActualWidth / container.Mask.PixelWidth;
            double scaleY = container.ActualHeight / container.Mask.PixelHeight;

            container.opacityMask.Viewport = new Rect(0, 0, maskWidth, maskHeight);

            double maxXOffset = container.ActualWidth - container.opacityMask.Viewport.Width;
            double maxYOffset = container.ActualHeight - container.opacityMask.Viewport.Height;

            double xOffset = maxXOffset * containerData.MaskLeft;
            double yOffset = maxYOffset * containerData.MaskTop;

            container.opacityMask.Viewport = new Rect(xOffset, yOffset, container.opacityMask.Viewport.Width, container.opacityMask.Viewport.Height);
            container.opacityMask.Viewport.Scale(maskWidth *scaleX , maskHeight * scaleY); 
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

        public override void ResizeGrid(int columns, int rows)
        {
        }

        private void OnTemplateChange(CollageTemplate? template)
        {
            UpdateEditor();
        }

    }
}

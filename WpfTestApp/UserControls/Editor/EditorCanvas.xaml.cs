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
            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test.yaml";
            _editorPanel = editorCanvas;
            _selectedTemplate = CollageTemplate.ReadFromYaml(file);
            Loaded += OnEditorLoad;
        }

        /*private void EditorCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test.yaml";
            CollageTemplate template = CollageTemplate.ReadFromYaml(file);
            UpdateCanvas(template);
        }*/

        protected CollageTemplate _selectedTemplate;
        private ImageContainer[]? _containers;

        protected override void UpdateEditor()
        {
            _editorPanel.Children.Clear();
            _editorPanel.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = this });
            _editorPanel.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = this });

            if (_selectedTemplate == null)
            {
                _containers = null!;
                return;
            }

            _containers = new ImageContainer[_selectedTemplate.Containers.Length];
            for (int i = 0; i < _selectedTemplate.Containers.Length; i++)
            {
                ImageContainer imgContainer = CreateGridElement(_selectedTemplate.Containers[i]);
                editorCanvas.Children.Add(imgContainer);
                _containers[i] = imgContainer;
            }
        }

        protected override ImageContainer CreateGridElement(ContainerData containerData)
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnEditorElementLeftClick;
            imgContainer.Source = "D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg";
            imgContainer.MaskSource = containerData.MaskSource;
            imgContainer.AllowDrop = true;
            imgContainer.Margin = new Thickness(_borderSize);

            Canvas.SetLeft(imgContainer, containerData.Left * editorCanvas.ActualWidth);
            Canvas.SetTop(imgContainer, containerData.Top * editorCanvas.ActualHeight);
            imgContainer.Width = containerData.Width * editorCanvas.ActualWidth;
            imgContainer.Height = containerData.Height * editorCanvas.ActualHeight;
            return imgContainer;
        }

        public override void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public override void ResizeGrid(int columns, int rows)
        {
        }
    }
}

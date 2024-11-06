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

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for EditorCanvas.xaml
    /// </summary>
    public partial class EditorCanvas : UserControl
    {
        public EditorCanvas()
        {
            InitializeComponent();
            Loaded += EditorCanvas_Loaded;
        }

        private void EditorCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test.yaml";
            CollageTemplate template = CollageTemplate.ReadFromYaml(file);
            UpdateCanvas(template);
        }

        public static readonly DependencyProperty ToolbarReferenceProperty = DependencyProperty.Register("ToolbarRef", typeof(EditorToolbar), typeof(EditorCanvas), new PropertyMetadata(null));
        public EditorToolbar ToolbarRef
        {
            get => (EditorToolbar)GetValue(ToolbarReferenceProperty);
            set => SetValue(ToolbarReferenceProperty, value);
        }

        private ImageContainer[] _containers;
        private double _borderSize = 3.5d;

        private void UpdateCanvas(CollageTemplate template)
        {
            
            editorCanvas.Children.Clear();
            // editorCanvas.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = editorControl });
            // editorCanvas.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = editorControl });

            _containers = new ImageContainer[template.Containers.Length];
            for (int i = 0; i < template.Containers.Length; i++)
            {
                ImageContainer imgContainer = CreateGridElement(template.Containers[i]);
                editorCanvas.Children.Add(imgContainer);
                _containers[i] = imgContainer;
            }
        }

        private ImageContainer CreateGridElement(ContainerData containerData)
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnCanvasElementLeftClick;
            imgContainer.Source = "D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg";
            imgContainer.MaskSource = containerData.MaskSource;
            imgContainer.AllowDrop = true;
            imgContainer.Margin = new Thickness(_borderSize);
            Canvas.SetLeft(imgContainer, containerData.Left);
            Canvas.SetTop(imgContainer, containerData.Top);
            imgContainer.Width = containerData.Width;
            imgContainer.Height = containerData.Height;
            return imgContainer;
        }

        private void OnCanvasElementLeftClick(object sender, MouseButtonEventArgs e)
        {
            /*ImageContainer? newSelected = sender as ImageContainer;
            if (newSelected == null)
            {
                Deselect();
                return;
            }

            var X = Grid.GetColumn(newSelected);
            var Y = Grid.GetRow(newSelected);

            ChangeSelectedElement(_selectedContainer, newSelected);*/
        }

        public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void ResizeGrid(int columns, int rows)
        {
        }
    }
}

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
            // string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test2.yaml";
            _editorPanel = editorCanvas;
            // _selectedTemplate = CollageTemplate.ReadFromYaml(file);
            AppParameters.Instance.EditorParameters.OnTemplateChange += OnTemplateChange;
            Loaded += OnEditorLoad;
        }

        /*private void EditorCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test.yaml";
            CollageTemplate template = CollageTemplate.ReadFromYaml(file);
            UpdateCanvas(template);
        }*/

        // protected CollageTemplate? _selectedTemplate;
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
            imgContainer.Padding = new Thickness(_borderSize);

            SetContrainerTransform(imgContainer, containerData);

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

        protected override void Resize(int width, int height)
        {
            this.Width = AppParameters.Instance.EditorParameters.Width;
            this.Height = AppParameters.Instance.EditorParameters.Height;

            if (_containers == null)
                return;

            for (int i = 0; i < AppParameters.Instance.EditorParameters.SelectedTemplate!.Containers.Length; i++)
            {
                SetContrainerTransform(_containers[i], AppParameters.Instance.EditorParameters.SelectedTemplate.Containers[i]);
            }
        }

        public override void ResizeGrid(int columns, int rows)
        {
        }

        private void OnTemplateChange(CollageTemplate? template)
        {
            // _selectedTemplate = template;
            UpdateEditor();
        }

    }
}

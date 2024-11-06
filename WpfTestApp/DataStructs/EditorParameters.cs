using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTestApp.DataStructs
{
    public class EditorParameters
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double BorderThickness { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public CollageTemplate? SelectedTemplate 
        { 
            get => _selectedTemplate; 
            set { _selectedTemplate = value; OnTemplateChange?.Invoke(_selectedTemplate); } 
        }

        public event Action<int, int> OnEditorResize;
        public event Action<CollageTemplate?> OnTemplateChange;

        private CollageTemplate? _selectedTemplate;

        public EditorParameters()
        {
            BorderThickness = 2;
            BackgroundColor = Colors.White;
            BorderColor = Colors.White;
            SelectedTemplate = null;
            Resize(1278, 640);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            if (OnEditorResize != null)
                OnEditorResize(width, height);
        }

    }
}

﻿using System.Windows.Media;

namespace WpfTestApp.DataStructs
{
    public class EditorParameters
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public (int Columns, int Rows) GridSize => (_gridColumns, _gridRows);


        public double AspectRatio => Width / Height;
        public double BorderThickness { get; set; }
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; OnBackgroundColorChange?.Invoke(_backgroundColor); }
        }
        public CollageTemplate? SelectedTemplate 
        { 
            get => _selectedTemplate; 
            set { _selectedTemplate = value; OnTemplateChange?.Invoke(_selectedTemplate); } 
        }
        public Uri DefaultImageUri { get; private set; }
        public readonly double DEFAULT_WIDTH = 1000;
        public readonly double DEFAULT_HEIGHT = 1000;

        public event Action<int, int> OnEditorResize;
        public event Action<int, int> OnEditorGridResize;
        public event Action<CollageTemplate?> OnTemplateChange;
        public event Action<Color> OnBackgroundColorChange;

        private CollageTemplate? _selectedTemplate;
        private Color _backgroundColor = Colors.White;
        private int _gridColumns;
        private int _gridRows;

        public EditorParameters()
        {
            BorderThickness = 20;
            BackgroundColor = Colors.White;
            SelectedTemplate = null;
            DefaultImageUri = new Uri("pack://application:,,,/Resources/imgPlaceholder.png", UriKind.Absolute);
            Resize((int)DEFAULT_WIDTH, (int)DEFAULT_HEIGHT);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            if (OnEditorResize != null)
                OnEditorResize(width, height);
        }

        public void ResizeGrid(int columns, int rows)
        {
            _gridColumns = columns;
            _gridRows = rows;
            if (OnEditorGridResize != null)
                OnEditorGridResize(columns, rows);
        }

    }
}

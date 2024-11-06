using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for EditorGrid.xaml
    /// </summary>
    public partial class EditorGrid : UserControl
    {
        public EditorGrid()
        {
            InitializeComponent();
            Loaded += OnLoad;
            editorGrid.LostFocus += EditorGrid_LostFocus;
        }

        private void EditorGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Deselect(false);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            UpgradeGrid();
        }

        public static readonly DependencyProperty ToolbarReferenceProperty = DependencyProperty.Register("ToolbarRef", typeof(EditorToolbar), typeof(EditorGrid), new PropertyMetadata(null));
        public EditorToolbar ToolbarRef
        {
            get => (EditorToolbar)GetValue(ToolbarReferenceProperty);
            set => SetValue(ToolbarReferenceProperty, value);
        }

        private ImageContainer[,] _containers;
        private ImageContainer? _selectedContainer = null;
        private Border _selectedContainerBorder;
        private double _borderSize = 3.5d;
        private double _splitterSize = 5;

        private int _columns;
        private int _rows;
        public int Rows { get => _rows; set => _rows = value; }
        public int Columns { get => _columns; set => _columns = value; }

        private void UpgradeGrid()
        {
            if (Columns == 0 || Rows == 0) 
                return;


            Grid grid = editorGrid;
            grid.ShowGridLines = false;
            editorGrid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            grid.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = editorGridControl });
            grid.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = editorGridControl });

            for (int y = 0; y < Rows; y++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                grid.RowDefinitions.Add(row);
                if (y < Rows - 1)
                {
                    RowDefinition splitterColumn = new RowDefinition { Height = new GridLength(_splitterSize) };
                    grid.RowDefinitions.Add(splitterColumn);
                    // add horizontal splitters
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                    gridSplitter.Height = _splitterSize;
                    // gridSplitter.Background = new SolidColorBrush(Colors.Transparent);
                    Grid.SetRow(gridSplitter, y * 2 + 1);
                    Grid.SetColumnSpan(gridSplitter, Columns * 2 - 1);
                    gridSplitter.DragDelta += (sender, e) => OnCellResize(sender, e, false);
                    grid.Children.Add(gridSplitter);
                }
                    
            }


            for (int x = 0; x < Columns; x++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                grid.ColumnDefinitions.Add(column);
                if (x < Columns - 1)
                {
                    ColumnDefinition splitterColumn = new ColumnDefinition { Width = new GridLength(_splitterSize) };
                    grid.ColumnDefinitions.Add(splitterColumn);

                    // add vertical splitters
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                    gridSplitter.Width = _splitterSize;
                    Grid.SetColumn(gridSplitter, x * 2 + 1);
                    Grid.SetRowSpan(gridSplitter, Rows * 2 - 1);
                    gridSplitter.DragDelta += (sender, e) => OnCellResize(sender, e, true);
                    grid.Children.Add(gridSplitter);
                }
            }


            ImageContainer[,] previousContainers = _containers;
            _containers = new ImageContainer[Columns, Rows];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    ImageContainer imgContainer;
                    // try to reuse existing container
                    if (!TryGetContainer(previousContainers, x, y, out imgContainer)) 
                    {
                        // create and set grid position for ImageContainer
                        imgContainer = CreateGridElement();
                        Grid.SetRow(imgContainer, y * 2);
                        Grid.SetColumn(imgContainer, x * 2);
                    }

                    grid.Children.Add(imgContainer);
                    _containers[x, y] = imgContainer;
                }
            }
        }

        private bool TryGetContainer(ImageContainer[,] source, int column, int row, out ImageContainer container)
        {
            if (source != null && column >= 0 && column < source.GetLength(0) && row >= 0 && row < source.GetLength(1))
            {
                container = source[column, row];
                return true;
            }
            
            container = null!;
            return false;
        }

        
        private ImageContainer CreateGridElement()
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnGridElementLeftClick;
            imgContainer.Source = "D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg";
            // imgContainer.MaskSource = "C:\\Users\\Vlad\\Desktop\\MASKS\\11441885.png";
            imgContainer.AllowDrop = true;
            imgContainer.Margin = new Thickness(_borderSize);
            return imgContainer;
        }

        private void OnCellResize(object sender, DragDeltaEventArgs e, bool horizontal)
        {
            GridSplitter? splitter = sender as GridSplitter;
            if (splitter == null)
                return;

            int cellC = (Grid.GetColumn(splitter) - 1) / 2;
            int cellR = (Grid.GetRow(splitter) - 1) / 2;
                
            ImageContainer imgContainer = _containers[cellC, cellR];
            if (horizontal)
                imgContainer.OnContainerResized(e.HorizontalChange, 0);
            else
                imgContainer.OnContainerResized(0, e.VerticalChange);
        }



        private void OnGridElementLeftClick(object sender, MouseButtonEventArgs e)
        {
            ImageContainer? newSelected = sender as ImageContainer;
            if (newSelected == null)
            {
                Deselect();
                return;
            }

            var X = Grid.GetColumn(newSelected);
            var Y = Grid.GetRow(newSelected);
            
            ChangeSelectedElement(_selectedContainer, newSelected);
        }

        private void Deselect(bool updateToolbar=true)
        {
            if (_selectedContainer != null)
            {
                _selectedContainer.BorderThickness = new Thickness(0);
                _selectedContainer.Margin = new Thickness(_borderSize);
                _selectedContainer = null;
            }

            if (updateToolbar)
                UpdateToolbar();
        }

        private void ChangeSelectedElement(ImageContainer? prev, ImageContainer current) 
        {
            Deselect();
            current.BorderThickness = new Thickness(_borderSize);
            current.BorderBrush = Brushes.Orange;
            current.Margin = new Thickness(0);
            current.Focus();

            _selectedContainer = current;
            UpdateToolbar();
        }

        private void UpdateToolbar()
        {
            if (ToolbarRef == null)
                return;

            ToolbarRef.Clear();
            if (_selectedContainer == null)
                return;

            _selectedContainer.CreateToolbarElements(ToolbarRef);
        }

        public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void ResizeGrid(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            UpgradeGrid();
        }


    }
}

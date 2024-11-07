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
using WpfTestApp.DataStructs;
using static System.Net.WebRequestMethods;

namespace WpfTestApp.UserControls.Editor
{
    /// <summary>
    /// Interaction logic for EditorGrid.xaml
    /// </summary>
    public partial class EditorGrid : EditorBase
    {
        public EditorGrid()
        {
            InitializeComponent();
            _editorPanel = editorGrid;
            Loaded += OnEditorLoad;
        }


        private ImageContainer[,] _containers;
        private Border _selectedContainerBorder;
        private bool _newGridSize = true;
        
        private double _splitterSize = 5;

        private int _columns;
        private int _rows;
        public int Rows { get => _rows; set => _rows = value; }
        public int Columns { get => _columns; set => _columns = value; }

        protected override void UpdateEditor()
        {
            if (Columns == 0 || Rows == 0) 
                return;


            Grid grid = editorGrid;
            grid.ShowGridLines = false;
            editorGrid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            grid.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = this });
            grid.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = this });

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
                    gridSplitter.Background = new SolidColorBrush(Colors.Transparent);
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
                    gridSplitter.Background = new SolidColorBrush(Colors.Transparent);
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
                        imgContainer = CreateGridElement(new ContainerData());
                        Grid.SetRow(imgContainer, y * 2);
                        Grid.SetColumn(imgContainer, x * 2);
                    }

                    grid.Children.Add(imgContainer);
                    _containers[x, y] = imgContainer;
                }
            }

            if (_newGridSize)
            {
                _newGridSize = false;
                UpdateLayout();
                // resize and center images
                for (int y = 0; y < Rows; y++)
                {
                    for (int x = 0; x < Columns; x++)
                    {
                        _containers[x, y].Fill();
                    }
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

        
        protected override ImageContainer CreateGridElement(ContainerData containerData)
        {
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnEditorElementLeftClick;
            // imgContainer.MaskSource = "C:\\Users\\Vlad\\Desktop\\MASKS\\11441885.png";
            imgContainer.AllowDrop = true;
            imgContainer.Padding = new Thickness(_borderSize);
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

        /*protected override void ChangeSelectedElement(ImageContainer? prev, ImageContainer current)
        {
            base.ChangeSelectedElement(prev, current);
            int containerCol = Grid.GetColumn(current);
            int containerRow = Grid.GetRow(current);
            HighlightSplitter(containerCol, containerRow);
        }*/

        public override void ResizeGrid(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            _newGridSize = true;
            UpdateEditor();
        }


    }
}

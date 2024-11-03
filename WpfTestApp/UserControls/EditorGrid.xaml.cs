using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for EditorGrid.xaml
    /// </summary>
    public partial class EditorGrid : UserControl, INotifyPropertyChanged
    {
        public EditorGrid()
        {
            InitializeComponent();
            Loaded += AssetsBox_Loaded;
        }

        private void AssetsBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if TargetImage is correctly set after the UserControl is fully loaded
            /*if (ToolbarReferenceProperty != null)
            {
                MessageBox.Show("TargetImage is set!");
                // Now you can manipulate TargetImage as needed
            }
            else
            {
                MessageBox.Show("TargetImage is still null.");
            }*/
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            // TODO: refactor to use onload event
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            CreateGrid();
        }

        public static readonly DependencyProperty ToolbarReferenceProperty = DependencyProperty.Register("ToolbarRef", typeof(EditorToolbar), typeof(EditorGrid), new PropertyMetadata(null));
        public EditorToolbar ToolbarRef
        {
            get => (EditorToolbar)GetValue(ToolbarReferenceProperty);
            set => SetValue(ToolbarReferenceProperty, value);
        }


        private ImageContainer? _selectedContainer = null;
        private Border _selectedContainerBorder;
        private double _borderSize = 3.5d;

        private int _columns;
        private int _rows;
        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged();
            }
        }

        private void CreateGrid()
        {
            if (Columns == 0 || Rows == 0) 
                return;

            const double splitterSize = 5;

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
                    RowDefinition splitterColumn = new RowDefinition { Height = new GridLength(splitterSize) };
                    grid.RowDefinitions.Add(splitterColumn);

                    // add horizontal splitters
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                    gridSplitter.Height = splitterSize;
                    Grid.SetRow(gridSplitter, y * 2 + 1);
                    Grid.SetColumnSpan(gridSplitter, Columns * 2 - 1);
                    grid.Children.Add(gridSplitter);
                }
                    
            }


            for (int x = 0; x < Columns; x++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                grid.ColumnDefinitions.Add(column);
                if (x < Columns - 1)
                {
                    ColumnDefinition splitterColumn = new ColumnDefinition { Width = new GridLength(splitterSize) };
                    grid.ColumnDefinitions.Add(splitterColumn);

                    // add vertical splitters
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                    gridSplitter.Width = splitterSize;
                    Grid.SetColumn(gridSplitter, x * 2 + 1);
                    Grid.SetRowSpan(gridSplitter, Rows * 2 - 1);
                    grid.Children.Add(gridSplitter);
                }
            }

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    // create and set grid position for ImageContainer
                    UIElement imgContainer = CreateGridElement();
                    Grid.SetRow(imgContainer, y * 2);
                    Grid.SetColumn(imgContainer, x * 2);
                    grid.Children.Add(imgContainer);
                }
            }
        }

        private UIElement CreateGridElement()
        {
            // Border elementBorder = new Border { Name="border", BorderThickness = new Thickness(5), BorderBrush = Brushes.Black };
            ImageContainer imgContainer = new ImageContainer();
            imgContainer.PreviewMouseLeftButtonDown += OnGridElementLeftClick;
            imgContainer.Source = "D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg";
            imgContainer.AllowDrop = true;
            imgContainer.Margin = new Thickness(_borderSize);
            // elementBorder.Child = imgContainer;
            return imgContainer;
            
        }

        private void OnGridElementLeftClick(object sender, MouseButtonEventArgs e)
        {
            // TODO: capture mouse
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

        private void Deselect()
        {
            if (_selectedContainer != null)
            {
                _selectedContainer.BorderThickness = new Thickness(0);
                _selectedContainer.Margin = new Thickness(_borderSize);
                _selectedContainer = null;
            }

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


    }
}

using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfTestApp.UserControls;

namespace WpfTestApp
{
    public partial class CollageWindow : Window
    {
        public CollageWindow()
        {
            InitializeComponent();
            // CanvasSetGrid(2, 2);
        }

        /*private void DropHandle(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.StringFormat);
            Console.WriteLine(data);
        }*/

        public void CanvasSetGrid(int r, int c)
        {
            Grid myGrid = new Grid();
            myGrid.ShowGridLines = true;

            myGrid.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = canvasEditor });
            myGrid.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = canvasEditor });

            ColumnDefinition colDef1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition colDef2 = new ColumnDefinition { Width = new GridLength(5) };
            ColumnDefinition colDef3 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            myGrid.ColumnDefinitions.Add(colDef1);
            myGrid.ColumnDefinitions.Add(colDef2);
            myGrid.ColumnDefinitions.Add(colDef3);

            RowDefinition rowDef1 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition rowDef2 = new RowDefinition { Height = new GridLength(5) };
            RowDefinition rowDef3 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            myGrid.RowDefinitions.Add(rowDef1);
            myGrid.RowDefinitions.Add(rowDef2);
            myGrid.RowDefinitions.Add(rowDef3);

            // define elements

            ImageContainer txt1 = new ImageContainer();
            // txt1.HorizontalAlignment = HorizontalAlignment.Stretch;
            // txt1.VerticalAlignment = VerticalAlignment.Stretch;
            txt1.Source = "D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg";
            // Uri fileUri = new Uri("D:\\_Images\\Fox\\Fox-HD-Wallpaper.jpg");
            // txt1.Source = new BitmapImage(fileUri);
            txt1.AllowDrop = true;
            // txt1.Drop += DropHandle;
            Grid.SetRow(txt1, 0);
            Grid.SetColumn(txt1, 0);

            TextBlock txt2 = new TextBlock();
            txt2.Text = "Quarter 2";
            txt2.FontSize = 12;
            txt2.FontWeight = FontWeights.Bold;
            txt2.HorizontalAlignment = HorizontalAlignment.Center;
            txt2.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txt2, 2);
            Grid.SetColumn(txt2, 0);

            TextBlock txt3 = new TextBlock();
            txt3.Text = "Quarter 3";
            txt3.FontSize = 12;
            txt3.FontWeight = FontWeights.Bold;
            txt3.HorizontalAlignment = HorizontalAlignment.Center;
            txt3.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txt3, 0);
            Grid.SetColumn(txt3, 2);

            TextBlock txt4 = new TextBlock();
            txt4.Text = "Quarter 4";
            txt4.FontSize = 12;
            txt4.FontWeight = FontWeights.Bold;
            txt4.HorizontalAlignment = HorizontalAlignment.Center;
            txt4.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(txt4, 2);
            Grid.SetColumn(txt4, 2);

            GridSplitter gridSplitter1 = new GridSplitter();
            gridSplitter1.HorizontalAlignment = HorizontalAlignment.Center;
            gridSplitter1.VerticalAlignment = VerticalAlignment.Stretch;
            gridSplitter1.Width = 5;
            Grid.SetColumn(gridSplitter1, 1);
            Grid.SetRowSpan(gridSplitter1, 3);

            GridSplitter gridSplitter2 = new GridSplitter();
            gridSplitter2.HorizontalAlignment = HorizontalAlignment.Stretch;
            gridSplitter2.VerticalAlignment = VerticalAlignment.Center;
            gridSplitter2.Height = 5;
            Grid.SetRow(gridSplitter2, 1);
            Grid.SetColumnSpan(gridSplitter2, 3);


            

            // add elements
            myGrid.Children.Add(txt1);
            myGrid.Children.Add(txt2);
            myGrid.Children.Add(txt3);
            myGrid.Children.Add(txt4);
            
            myGrid.Children.Add(gridSplitter1);
            myGrid.Children.Add(gridSplitter2);

            // Canvas.SetLeft(myGrid, 0);
            // Canvas.SetTop(myGrid, 0);

            // add grid to canvas
            myGrid.Name = "GGGRID";
            canvasEditor.Children.Add(myGrid);
        }



    }
}
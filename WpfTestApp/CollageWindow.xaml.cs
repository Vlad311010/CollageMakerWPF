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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WpfTestApp.DataStructs;
using WpfTestApp.UserControls;
using WpfTestApp.UserControls.Editor;
using WpfTestApp.Utils;

namespace WpfTestApp
{
    public partial class CollageWindow : Window
    {
        public CollageWindow()
        {
            InitializeComponent();
            LoadTemplates();

            tbCollageWidthInput.Text = AppParameters.Instance.EditorParameters.Width.ToString();
            tbCollageHeightInput.Text = AppParameters.Instance.EditorParameters.Height.ToString();

            // TODO: refact?
            _editorCanvas = new EditorCanvas();
            _editorCanvas.ToolbarRef = editorToolbar;
            _editorCanvas.Background = new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor);

            _editorGrid = new EditorGrid();
            _editorGrid.ToolbarRef = editorToolbar;
            _editorGrid.Background = new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor);
            
            SwitchEditor(_editorGrid);
        }

        private CollageTemplate[] _templates;
        private EditorCanvas _editorCanvas;
        private EditorGrid _editorGrid;
        private EditorBase _activeEditor;


        private void Button_ResizeCollage(object sender, RoutedEventArgs e)
        {
            int width;
            int height;
            bool isWidthParsed = int.TryParse(tbCollageWidthInput.Text, out width);
            bool isHeightParsed = int.TryParse(tbCollageHeightInput.Text, out height);
            if (!(isWidthParsed && IsValidWindowSizeValue(width)) || !(isHeightParsed && IsValidWindowSizeValue(height)))
            {
                MessageBox.Show($"Invalid collage size: {width}x{height}");
                return;
            }

            ResizeCollage(width, height);
        }

        private bool IsValidWindowSizeValue(int value)
        {
            return value > 0 && value < 6000;
        }

        private void Button_SaveCollage(object sender, RoutedEventArgs e)
        {
            SaveCollage();
        }

        private void Button_ResizeGrid(object sender, RoutedEventArgs e)
        {
            int columns = 0;
            int rows = 0;
            bool isColumnsParsed = int.TryParse(tbCollageGridColumns.Text, out columns);
            bool isRowsParsed = int.TryParse(tbCollageGridRows.Text, out rows);
            if (isColumnsParsed && isRowsParsed && columns > 0 && columns * rows >= 1) // check if column and row >= 1
            {
                SwitchEditor(_editorGrid);
                _activeEditor.ResizeGrid(columns, rows);
            }
            else
                MessageBox.Show("Invalid parametes. Grid must have atleast one coulumn and row");
        }

        private void LoadTemplates()
        {
            string[] templateFiles = Directory.GetFiles(AppParameters.TEMPLATES_FOLDER);

            _templates = new CollageTemplate[templateFiles.Length];
            for (int i = 0; i < templateFiles.Length; i++)
                _templates[i] = CollageTemplate.ReadFromYaml(templateFiles[i]);

            List<Tuple<string, int>> templatesWithActions = _templates
                .Select((template, idx) => new Tuple<string, int>(template.Name, idx))
                .ToList();

            icTemplatesGrid.ItemsSource = templatesWithActions;
        }


        public void ResizeCollage(int width, int height)
        {
            // set editor size.
            AppParameters.Instance.EditorParameters.Resize(width, height);
        }
        
        private void OnTemplateClick(object sender, RoutedEventArgs e)
        {
            var tagValue = ((Button)sender).Tag;
            if (tagValue == null)
                return;

            SwitchEditor(_editorCanvas);
            int templateIdx = int.Parse(tagValue.ToString()!);
            ChangeCollageTemplate(_templates[templateIdx]);
        }

        private void ChangeCollageTemplate(CollageTemplate template)
        {
            AppParameters.Instance.EditorParameters.SelectedTemplate = template;
        }

        private void SwitchEditor(EditorBase editor)
        {
            if (_activeEditor == null || _activeEditor.GetType() != editor.GetType())
            {
                vbEditor.Child = null;
                _activeEditor = editor;
                vbEditor.Child = _activeEditor;
            }
        }

        private void SaveCollage()
        {
            UIElement target = _activeEditor;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            const double dpi = 96d;


            Pen borderPen = new Pen
            (
                new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor),
                AppParameters.Instance.EditorParameters.BorderThickness
            );


            int width = (int)(bounds.Width);
            int height = (int)(bounds.Height);
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                dc.DrawRectangle(vb, borderPen, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "MyCollage";
            #if DEBUG
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            #else
                saveFileDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            #endif
            saveFileDialog.Filter = "Images|*.png;*.jpg;*.jpeg;";
            if (saveFileDialog.ShowDialog() == true) 
            {
                using (Stream stm = File.Create(saveFileDialog.FileName))
                {
                    png.Save(stm);
                }
            }
        }

        private void backroundColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null)
                AppParameters.Instance.EditorParameters.BackgroundColor = (Color)e.NewValue;
        }
    }
}
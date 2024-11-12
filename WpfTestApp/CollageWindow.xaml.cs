using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfTestApp.DataStructs;
using WpfTestApp.UserControls.Editor;
using WpfTestApp.Utils;
using YamlDotNet.Core;

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
            // initialize editors
            _editorCanvas = new EditorCanvas();
            _editorCanvas.ToolbarRef = editorToolbar;
            _editorCanvas.Background = new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor);

            _editorGrid = new EditorGrid();
            _editorGrid.ToolbarRef = editorToolbar;
            _editorGrid.Background = new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor);
            
            SwitchEditor(_editorGrid);
            SwitchEditor(_editorCanvas);

            ChangeCollageTemplate(_templates[0]);
        }

        private CollageTemplate[] _templates;
        private EditorCanvas _editorCanvas;
        private EditorGrid _editorGrid;
        private EditorBase _activeEditor;
        private const int MAX_GRID_CELLS = 100;

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
            return value > 0 && value <= 6000;
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
            if (isColumnsParsed && isRowsParsed && columns > 0 && columns * rows >= 1 && columns * rows <= MAX_GRID_CELLS) // check if column and row >= 1. TODO:Move to grid class
            {
                SwitchEditor(_editorGrid);
                AppParameters.Instance.EditorParameters.ResizeGrid(columns, rows);
            }
            else
                MessageBox.Show($"Invalid parametes. Grid must have atleast one column and row and contain less the {MAX_GRID_CELLS} cells");
        }

        private void LoadTemplates()
        {
            string[] templateFiles = Directory.GetFiles(AppParameters.TEMPLATES_FOLDER);

            List<CollageTemplate> templates = new List<CollageTemplate>();
            List<string> invalidTemplates = new List<string>();
            for (int i = 0; i < templateFiles.Length; i++)
            {
                try
                {
                    CollageTemplate template = CollageTemplate.ReadFromYaml(templateFiles[i]);
                    templates.Add(template);
                }
                catch (YamlException e)
                {
                    invalidTemplates.Add(templateFiles[i]);
                }
            }
            
            _templates = templates.ToArray();
            List<Tuple<string, int>> templatesWithActions = _templates
                .Select((template, idx) => new Tuple<string, int>(template.Name, idx))
                .ToList();

            icTemplatesGrid.ItemsSource = templatesWithActions;

            if (invalidTemplates.Count > 0)
                MessageBox.Show("Some of the templates are misformatted:\n" + string.Join(", ", invalidTemplates.Select(path => Path.GetFileName(path))));
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

            int templateIdx = int.Parse(tagValue.ToString()!);
            ChangeCollageTemplate(_templates[templateIdx]);
        }

        private void ChangeCollageTemplate(CollageTemplate template)
        {
            SwitchEditor(_editorCanvas);
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
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);

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
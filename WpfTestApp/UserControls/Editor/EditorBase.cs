using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfTestApp.DataStructs;
using WpfTestApp.Utils;

namespace WpfTestApp.UserControls.Editor
{
    public abstract class EditorBase : UserControl
    {
        protected Panel _editorPanel;
        private ImageContainer? _selectedContainer = null;
        protected double _borderSize = 3.5d;

        public static readonly DependencyProperty ToolbarReferenceProperty = DependencyProperty.Register("ToolbarRef", typeof(EditorToolbar), typeof(EditorBase), new PropertyMetadata(null));
        public EditorToolbar ToolbarRef
        {
            get => (EditorToolbar)GetValue(ToolbarReferenceProperty);
            set => SetValue(ToolbarReferenceProperty, value);
        }

        protected void OnEditorLoad(object sender, RoutedEventArgs e)
        {
            AppParameters.Instance.EditorParameters.OnEditorResize += Resize;
            _editorPanel.LostFocus += OnFocusLost;
            _editorPanel.Background = new SolidColorBrush(AppParameters.Instance.EditorParameters.BackgroundColor);
            AppParameters.Instance.EditorParameters.OnBackgroundColorChange += OnBackgroundColorChange;
            UpdateEditor();
            
            Resize(AppParameters.Instance.EditorParameters.Width, AppParameters.Instance.EditorParameters.Height);
        }

        private void OnFocusLost(object sender, RoutedEventArgs e)
        {
            Deselect(false);
        }

        protected virtual void OnBackgroundColorChange(Color newColor)
        {
            _editorPanel.Background = new SolidColorBrush(newColor);
        }


        protected abstract void UpdateEditor();

        protected abstract ImageContainer CreateGridElement(ContainerData containerData);

        protected virtual void Resize(int width, int height)
        {
            this.Width = AppParameters.Instance.EditorParameters.Width;
            this.Height = AppParameters.Instance.EditorParameters.Height;
        }

        public abstract void ResizeGrid(int columns, int rows);

        protected void OnEditorElementLeftClick(object sender, MouseButtonEventArgs e)
        {
            ImageContainer? newSelected = sender as ImageContainer;
            if (newSelected == null)
            {
                Deselect();
                return;
            }

            ChangeSelectedElement(_selectedContainer, newSelected);
        }

        private void Deselect(bool updateToolbar = true)
        {
            if (_selectedContainer != null)
            {
                _selectedContainer.BorderThickness = new Thickness(0);
                // _selectedContainer.Margin = new Thickness(_borderSize);
                _selectedContainer.Padding = new Thickness(_borderSize);
                _selectedContainer = null;
            }

            if (updateToolbar)
                UpdateToolbar();
        }

        protected virtual void ChangeSelectedElement(ImageContainer? prev, ImageContainer current)
        {
            Deselect();
            current.BorderThickness = new Thickness(_borderSize);
            current.BorderBrush = Brushes.Orange;
            current.Padding = new Thickness(0);
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

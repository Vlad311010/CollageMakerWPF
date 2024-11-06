using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfTestApp.DataStructs;

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
            _editorPanel.LostFocus += OnFocusLost;
            UpdateEditor();
        }

        private void OnFocusLost(object sender, RoutedEventArgs e)
        {
            Deselect(false);
        }

        protected abstract void UpdateEditor();

        protected abstract ImageContainer CreateGridElement(ContainerData containerData);

        public virtual void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
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
    }
}

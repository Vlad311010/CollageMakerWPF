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
        }

        private double _editorWindowWidth;
        private double _editorWindowHeight;

        public void ResizeEditorWindow(double width, double height)
        {
            // set editor size.
            // editorGrid.Resize();
        }



    }
}
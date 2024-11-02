using Microsoft.Win32;
using System.IO;
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

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.png;*.jpg;*.jpeg;";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                lbFiles.Items.Clear();
                lbFiles.BorderThickness = new Thickness(2);
                foreach (string fileName in openFileDialog.FileNames)
                {
                    lbFiles.Items.Add(fileName);
                }
            }
            else
            {
                lbFiles.Items.Clear();
                lbFiles.BorderThickness = new Thickness(0);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
using WpfTestApp.Utils;

namespace WpfTestApp.UserControls
{
    /// <summary>
    /// Interaction logic for EditorToolbar.xaml
    /// </summary>
    public partial class EditorToolbar : UserControl
    {
        public EditorToolbar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BtnTemplateProperty = DependencyProperty.Register("BtnTemplate", typeof(Button), typeof(EditorToolbar), new PropertyMetadata(null));
        public static readonly DependencyProperty SliderTemplateProperty = DependencyProperty.Register("SliderTemplate", typeof(Slider), typeof(EditorToolbar), new PropertyMetadata(null));
        public Button BtnTemplate
        {
            get => (Button)GetValue(BtnTemplateProperty);
            set => SetValue(BtnTemplateProperty, value);
        }

        public Slider SliderTemplate
        {
            get => (Slider)GetValue(SliderTemplateProperty);
            set => SetValue(SliderTemplateProperty, value);
        }

        public void Clear()
        {
            stackPanel.Children.Clear();
        }

        public Button AddBtn(string text, Action btnClick)
        {
            Button newBtn = BtnTemplate.Clone();
            newBtn.Content = text;
            RoutedEventHandler clickEventHandler = new RoutedEventHandler((sendItem, args) => btnClick?.Invoke());
            newBtn.Click += clickEventHandler;
            newBtn.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.Children.Insert(0, newBtn);
            return newBtn;
        }

        public Slider AddSlider(double min, double max, double defaultValue, string label, Action<double, double> slideValueChange)
        {
            DockPanel sliderDockPanel = new DockPanel();
            Label sliderLabel = new Label { Content = label };
            sliderLabel.VerticalAlignment = VerticalAlignment.Center;
            sliderLabel.FontWeight = FontWeights.Bold;
            DockPanel.SetDock(sliderLabel, Dock.Left);
            sliderDockPanel.Children.Add(sliderLabel);

            Slider newSlider = SliderTemplate.Clone();
            newSlider.Minimum = min;
            newSlider.Maximum = max;
            newSlider.Value = defaultValue;
            newSlider.ValueChanged += (s, e) => slideValueChange?.Invoke(e.OldValue, e.NewValue);
            sliderDockPanel.Children.Add(newSlider);
            
            stackPanel.Children.Insert(0, sliderDockPanel);
            return newSlider;
        }
    }
}

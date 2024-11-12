using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfTestApp.Controls
{
    /// <summary>
    /// Adds Percentage property to Track. Percentage value used in slider styling to create filling gradient.
    /// </summary>
    public class PercentageTrack : Track
    {
        
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(
                "Percentage",          // Property name
                typeof(double),          // Property type
                typeof(PercentageTrack),    // Owner type
                new PropertyMetadata() // Default value
            );

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ValueProperty)
            {
                CalculatePercentage((double)e.OldValue, (double)e.NewValue);
            }
        }
        protected void CalculatePercentage(double oldValue, double newValue)
        {
            Percentage = (Value - Minimum) / (Maximum - Minimum);
        }

    }
}

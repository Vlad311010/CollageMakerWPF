using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace WpfTestApp.Utils
{
    public static class Extensions
    {
        public static T Clone<T>(this T originalBtn) where T : UIElement
        {
            string buttonXaml = XamlWriter.Save(originalBtn);

            // Deserialize the XAML back to a new Button instance
            using (StringReader stringReader = new StringReader(buttonXaml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)XamlReader.Load(xmlReader);
                }
            }
        }

        public static bool IsChildOf(DependencyObject? child, DependencyObject parent)
        {
            DependencyObject? current = child;

            while (current != null)
            {
                if (current == parent)
                    return true;

                current = VisualTreeHelper.GetParent(current);
            }

            return false;
        }
    }

}

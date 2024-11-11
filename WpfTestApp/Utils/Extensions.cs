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
using YamlDotNet.Core.Tokens;

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

        // def normalize(value, old_min=0.1, old_max= 8, new_min= 0, new_max= 1):
        // return ((value - old_min) / (old_max - old_min)) * (new_max - new_min) + new_min

        /// <summary>
        /// maps value from (min, max) range into (0-1) range
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double NormalizeRange(double min, double max, double value)
        {
            return ((value - min) / (max - min)) * 1 + min; 
        }


        /// <summary>
        /// maps value from (0, 1) range into (min-max) range
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double MapFromNormalRange(double min, double max, double value)
        {
            return value * (max - min) + min;
        }

    }
}

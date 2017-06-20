using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Video.Converter
{
    //Class trouvé sur le net pour pouvoir adapter le sous-titre par rapport a la taille de la video
    // (pas réussi a faire fonctionner correctement)
    public class PercentageConverter : MarkupExtension, IValueConverter
    {
        private static PercentageConverter _instance;
        
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new PercentageConverter());
        }
    }
}


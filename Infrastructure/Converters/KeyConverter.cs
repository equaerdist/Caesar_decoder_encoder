using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Caesar_decoder_encoder.Infrastructure.Converters
{
    internal class KeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (BigInteger)value;
            return number.ToString(CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (BigInteger.TryParse(stringValue, out var number))
                {
                    return number;
                }
            }

            return new BigInteger(2);
        }
    }
}

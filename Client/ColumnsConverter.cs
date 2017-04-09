using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client
{
    public class ColumnsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 1;
            if ((int)value <= 2) retVal = 1;//se il # di server    
            else if ((int)value >= 3 && (int)value <= 6) retVal = 2;
            else if ((int)value >= 6)
            {
                decimal val = (int)value / 4;
                retVal = Math.Ceiling(val);
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

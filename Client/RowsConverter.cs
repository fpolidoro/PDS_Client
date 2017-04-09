using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client
{
    public class RowsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int retVal = 2;
            if ((int)value <= 2) retVal = 2;//se il # di server    
            else if ((int)value >= 3 && (int)value <= 6) retVal = 3;
            else if ((int)value > 6) retVal = 4;
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

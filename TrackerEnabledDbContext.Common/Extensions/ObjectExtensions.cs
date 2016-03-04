using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToString(this object obj, CultureInfo cultureInfo)
        {
            return obj is IConvertible ? ((IConvertible)obj).ToString(cultureInfo)
                : obj is IFormattable ? ((IFormattable)obj).ToString(null, cultureInfo)
                : obj.ToString();
        }
    }
}

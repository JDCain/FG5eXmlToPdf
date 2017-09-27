using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FG5eXmlToPdf
{
    internal static class Helper
    {
        internal static bool StringIntToBool(string x)
        {
            var i = int.Parse(x);
            return i > 0;

        }

        internal static string BoolToYesNo(bool s)
        {
            return s ? "Yes" : "No";
        }
    }
}

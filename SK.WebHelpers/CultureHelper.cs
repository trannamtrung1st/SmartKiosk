using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SK.Business;

namespace SK.WebHelpers
{
    public static class CultureHelper
    {
        public static string CurrentLang
        {
            get
            {
                return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }
        }

        public static TimeZoneInfo DefaultTimeZone
        {
            get
            {
                return AppTimeZone.Map[CurrentLang];
            }
        }
    }
}

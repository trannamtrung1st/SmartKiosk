using System;
using System.Collections.Generic;
using System.Text;

namespace SK.WebHelpers
{
    public static class PageHelper
    {
        public static string SelectedIfActiveLang(string lang)
        {
            return CultureHelper.CurrentLang == lang ? "selected" : "";
        }
    }
}

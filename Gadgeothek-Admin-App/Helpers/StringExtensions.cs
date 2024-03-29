﻿using System;

namespace Gadgeothek_Admin_App.Helpers
{
    static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (string.IsNullOrEmpty(toCheck) || string.IsNullOrEmpty(source))
                return true;

            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

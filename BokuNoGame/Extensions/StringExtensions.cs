using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Extensions
{
    public static class StringExtensions
    {
        public static HtmlString ToHtmlString(this string value)
        {
            return new HtmlString(value);
        }
    }
}

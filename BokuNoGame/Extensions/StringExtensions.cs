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
            value = value.Replace("<script>", string.Empty).Replace("</script>", string.Empty);
            return new HtmlString(value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace YYQERP.Infrastructure.Helpers
{
    /// <summary>
    /// HtmlHelper 的摘要说明
    /// </summary>
    public class MyHtmlHelper
    {
        private static string GetDisPlayName(PropertyInfo proInfo)
        {
            object[] attrs =
                proInfo.GetCustomAttributes(
                    typeof(DisplayNameAttribute), false);

            DisplayNameAttribute[] attributes = (DisplayNameAttribute[])proInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return (attributes.Length > 0) ? attributes[0].DisplayName : "";

        }

        public static string CreateDetailHtml<T>(T objEntity) where T : class
        {
            if (objEntity == default(T))
            {
                return "";
            }
            StringBuilder sb = new StringBuilder("<div id=\"div_content\"><table id=\"table_content\" class=\"table_content\" cellpadding=\"3\" cellspacing=\"0\" border=\"0\">");
            string name;
            object value;
            string val;
            foreach (PropertyInfo info in objEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                name = GetDisPlayName(info);
                value = info.GetValue(objEntity, null);
                val = value == null ? "" : value.ToString();
                sb.Append(CreateRow(name, val));
            }
            sb.Append("</table></div>");
            return sb.ToString();
        }

        private static string CreateRow(string name, string value)
        {
            return string.Format("<tr><td class=\"td_title\">{0}</td><td>{1}</td></tr>", name, value);
        }
    }
}
using YYQERP.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure
{
    public static class CommonExtMethod
    {
        public static IList<MyListItem> GetEnumList(this Type type)
        {
            IList<MyListItem> list = new List<MyListItem>();
            foreach (var myCode in Enum.GetValues(type))
            {
                FieldInfo field = type.GetField(myCode.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                string strName = attributes[0].Description;
                int intValue = (int)myCode;
                list.Add(new MyListItem { id = intValue, name = strName });
            }
            return list;
        }
    }
}

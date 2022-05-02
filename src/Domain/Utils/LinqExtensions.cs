using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MGCap.Domain.Utils
{
    public static class LinqExtensions
    {
        // See https://stackoverflow.com/a/6200460/2146113
        // and https://www.hanselman.com/blog/?date=2010-02-04
        public static string ToCsv<T>(this IEnumerable<T> items, bool addHeader = true, params string[] toExlude)
    where T : class
        {
            var csvBuilder = new StringBuilder();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     ?.Where(x => !toExlude.Contains(x.Name));
            if (addHeader)
            {
                string head = string.Join(",", properties.Select(p => p.Name.ToCsvValue()).ToArray());
                csvBuilder.AppendLine(head);
            }
            foreach (T item in items)
            {
                string line = string.Join(",", properties.Select(p => p.GetValue(item, null).ToCsvValue()).ToArray());
                csvBuilder.AppendLine(line);
            }
            return csvBuilder.ToString();
        }

        private static string ToCsvValue<T>(this T item)
        {
            if (item == null)
            {
                return "\"\"";
            }

            if (item is string)
            {
                return string.Format("\"{0}\"", item.ToString().Trim().Replace("\"", "\\\""));
            }
            if (double.TryParse(item.ToString(), out double dummy))
            {
                return string.Format("{0}", item);
            }
            if (item is DateTime dt)
            {
                if (dt.Year < 1970)
                {
                    return "\"\"";
                }

                return string.Format("\"{0}\"", dt.ToString("MM/dd/yyyy hh:mm tt"));
            }
            return string.Format("\"{0}\"", item);
        }
    }
}

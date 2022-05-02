using System.Text;
using System.Text.RegularExpressions;

namespace MGCap.Domain.Utils
{
    public static class StringExtensions
    {
        public static string LeftSubstring(this string input, int desiredLength)
        {
            if (input.Length < desiredLength)
            {
                return input;
            }

            return string.Format("{0}...", input.Substring(0, desiredLength));
        }

        public static string EnumDisplayName(this string input)
        {
            return input.Replace("_", " ");
        }

        public static string RemoveDuplicatedSpaces(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, @"\s+", " ");
        }

        public static string RemoveExtraNewLineCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, @"\n([\n\t\r\s])*\n", "\n");
        }

        private static readonly Regex sWhitespace = new Regex(@"\s+");

        public static string ReplaceWhitespace(this string input, string replacement = "")
        {
            return sWhitespace.Replace(input, replacement);
        }

        public static string RemoveAccent(this string str)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(str);
            return Encoding.ASCII.GetString(bytes);
        }

        // See https://stackoverflow.com/a/5796793/2146113
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        /// <summary>
        /// Generates a permalink slug for passed string
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>clean slug string (ex. "some-cool-topic")</returns>
        public static string GenerateSlug(this string phrase)
        {
            var s = phrase.RemoveAccent().ToLower();
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");                      // remove invalid characters
            s = Regex.Replace(s, @"\s+", " ").Trim();                       // single space
            s = s.Substring(0, s.Length <= 45 ? s.Length : 45).Trim();      // cut and trim
            s = Regex.Replace(s, @"\s", "-");                               // insert hyphens
            return s.ToLower();
        }
    }
}

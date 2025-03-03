namespace ALife.Core.Utility.Extensions
{
    /// <summary>
    /// Extensions for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the string with the replacement string inserted at any occurrence of the replacements listed.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacement"></param>
        /// <param name="stringsToReplace"></param>
        /// <returns>The updated string.</returns>
        public static string ReplaceAny(this string str, string replacement, params string[] stringsToReplace)
        {
            string output = str;
            for(int i = 0; i < stringsToReplace.Length; i++)
            {
                output = output.Replace(stringsToReplace[i], replacement);
            }
            return output;
        }
    }
}
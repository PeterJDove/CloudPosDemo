using System;

namespace Touch.Tools
{

    /// <summary>
    /// Extension methods are public static methods distiguished by
    /// the "this" modifier on the first parameter.  The TYPE of that
    /// first parameter is the type (or class) that is extended.
    /// 
    /// For example: 
    /// <code>
    /// public static string Head(this string value) 
    /// {
    ///     // extends the string type with a Head() method.  
    /// }
    /// </code>
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// An extension method that returns a SubArray of any Array.
        /// 
        /// Call using <code>source.SubArry(int index, int count)</code>
        /// </summary>
        /// <typeparam name="T">The type of the array to be provided, and returned.</typeparam>
        /// <param name="source">The array (of type T) from which a subset is to be returned</param>
        /// <param name="index">The index of the first element within <paramref name="source"/> to be returned in the subset.</param>
        /// <param name="length">The (maximum) number of elements to be returned.</param>
        /// <returns>An array of the same type as <paramref name="source"/></returns>
        public static T[] SubArray<T>(this T[] source, int index, int length)
        {
            var result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// An extension method for Array that indicates whether it is null or, if not null, empty.
        /// 
        /// Call using <code>array.IsNullOrEmpty()</code>
        /// </summary>
        /// <param name="array">Any Array</param>
        /// <returns>Boolean</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        /// <summary>
        /// Gets the First Word of a string, ignoring any leading spaces or Control Chars.
        /// 
        /// Call using <code>string.Head()</code>
        /// </summary>
        /// <param name="value">A string, from which the first word (Head) is to be extracted.</param>
        /// <returns>first word in <paramref name="value"/></returns>
        public static string Head(this string value)
        {
            if (value == null)
                return null;

            var n1 = 0;
            while (n1 < value.Length && (value[n1] == ' ' 
                || Char.IsControl(value[n1]))) // Skip leading spaces and control chars
                n1 += 1;

            if (n1 < value.Length)
            {
                var n2 = n1 + 1;
                while (n2 < value.Length && value[n2] != ' ' 
                    && !Char.IsControl(value[n2])) // Count non-spaces
                    n2 += 1;

                return value.Substring(n1, n2 - n1);
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets that part of a string following the Head.
        /// 
        /// Call using <code>string.Tail()</code>
        /// </summary>
        /// <param name="value">a string</param>
        /// <returns>That part of <paramref name="value"/> following the first word (its <see cref="Head"/>).</returns>
        public static string Tail(this string value)
        {
            if (value == null)
                return null;

            var n = 0;
            while (n < value.Length && (value[n] == ' ' 
                || Char.IsControl(value[n]))) // Skip leading spaces and control chars
                n += 1;

            while (n < value.Length && value[n] != ' ' 
                && !Char.IsControl(value[n])) // Skip first word
                n += 1;

            return value.Substring(n);
        }

        /// <summary>
        /// Gets the string with extra spaces before each capital letter.
        /// 
        /// Call using <code>string.SplitCamelCase()</code>
        /// </summary>
        /// <param name="value">a string</param>
        /// <returns>The string with extra spaces inserted.</returns>
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", 
                " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }


    }
}

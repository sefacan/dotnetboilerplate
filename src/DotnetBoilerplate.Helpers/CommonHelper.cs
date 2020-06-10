using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace DotnetBoilerplate.Helpers
{
    public static class CommonHelper
    {
        public static string ToCapitalize(string text)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            char[] array = text.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }

            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());

            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];

            using var provider = new RNGCryptoServiceProvider();
            provider.GetBytes(randomNumberBuffer);

            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= maxLength)
                return str;

            var pLen = postfix?.Length ?? 0;

            var result = str.Substring(0, maxLength - pLen);
            if (!string.IsNullOrEmpty(postfix))
            {
                result += postfix;
            }

            return result;
        }

        public static bool SequenceEqual<T>(IEnumerable<T> a1, IEnumerable<T> a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Count() != a2.Count())
                return false;

            var orderedA1 = a1.OrderBy(a => a);
            var orderedA2 = a2.OrderBy(a => a);

            var comparer = EqualityComparer<T>.Default;
            return !orderedA1.Where((t, i) => !comparer.Equals(t, orderedA2.ElementAtOrDefault(i))).Any();
        }

        public static bool PropertyValuesAreEqual<T>(this T from, T to, out string propertyName, params string[] ignore) where T : class
        {
            propertyName = string.Empty;

            if (from == null || to == null)
                return false;

            var type = typeof(T);
            var ignoredProps = new List<string>(ignore);
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ignoredProps.Contains(property.Name))
                    continue;

                var fromValue = type.GetProperty(property.Name).GetValue(from, null);
                var toValue = type.GetProperty(property.Name).GetValue(to, null);

                if (fromValue != toValue && (fromValue == null || !fromValue.Equals(toValue)))
                {
                    propertyName = property.Name;
                    return false;
                }
            }

            return true;
        }
    }
}
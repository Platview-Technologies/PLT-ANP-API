using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Utilities
{
    /// <summary>
    /// Utility class for resolving delimiters and patterns for custom tags.
    /// </summary>
    public class DelimiterResolver
    {
        private const string startDel = @"[[";
        private const string endDel = "]]";
        private const string startval = @"\[\[";
        private const string endval = @".*";
        private const char splitCharacter = '=';

        /// <summary>
        /// The character used to split custom tag names and values in a custom tag string.
        /// </summary>
        public static char SplitCharacter
        {
            get
            {
                return splitCharacter;
            }
        }

        /// <summary>
        /// The regex pattern for identifying the start of a custom tag in the content.
        /// </summary>
        public static string StartPattern
        {
            get
            {
                return startval + endval;
            }
        }

        /// <summary>
        /// The regex pattern for identifying the end of a custom tag in the content.
        /// </summary>
        public static string EndPattern
        {
            get
            {
                return @"\]\]";
            }
        }

        /// <summary>
        /// The number of regex patterns used for custom tag identification.
        /// </summary>
        public static int PatternCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Creates a regex pattern to identify a custom tag with the given name in the content.
        /// </summary>
        /// <param name="value">The name of the custom tag.</param>
        /// <returns>The regex pattern to identify the custom tag with the given name.</returns>
        public static string StartPatternWithName(string value)
        {
            return (startval + value + endval);
        }

        /// <summary>
        /// Appends delimiters to the provided value to create a custom tag string.
        /// </summary>
        /// <param name="value">The value to be enclosed in delimiters.</param>
        /// <returns>The custom tag string with delimiters.</returns>
        public static string AppendDelimiters(string value)
        {
            return (startDel + value + endDel);
        }
    }
}

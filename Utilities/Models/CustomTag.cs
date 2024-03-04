using System;
using Utilities.Utilities;

namespace Utilities.Models
{
    public class CustomTag
    {
        public string TagName { get; set; }
        public string ContentPath { get; set; }

        /// <summary>
        /// The full tag name with the start pattern to be used for custom tag identification.
        /// </summary>
        public string TagNameWithStartPattern
        {
            get
            {
                if (string.IsNullOrEmpty(TagName)) { return string.Empty; }
                return DelimiterResolver.StartPatternWithName(TagName);
            }
        }

        /// <summary>
        /// The length of the start delimiter used in the custom tag.
        /// </summary>
        public int StartDelimiterLength
        {
            get
            {
                if (string.IsNullOrEmpty(TagName)) { return DelimiterResolver.PatternCount; }
                return TagName.Length + DelimiterResolver.PatternCount;
            }
        }
    }
}

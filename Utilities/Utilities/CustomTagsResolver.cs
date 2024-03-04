using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.Utilities
{
    /// <summary>
    /// Utility class for resolving custom tags in the content with corresponding templates.
    /// </summary>
    public class CustomTagsResolver
    {
        /// <summary>
        /// The raw content containing custom tags to be resolved.
        /// </summary>
        public string RawContent { get; set; }

        /// <summary>
        /// The regex pattern to identify the start of the custom tag.
        /// </summary>
        public string StartPattern { get; set; }

        /// <summary>
        /// The length of the start delimiter used in the custom tag.
        /// </summary>
        public int StartDelimiterLength { get; set; }

        /// <summary>
        /// The template content to be used in place of the custom tag.
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// The file path to the template content if it is loaded from a file.
        /// </summary>
        public string TemplateContentpath { get; set; }

        // Private properties for internal use.
        private string PointerName { get; set; }
        private string PointerWithDelimiter { get; set; }

        /// <summary>
        /// The resolved content with custom tags replaced by their corresponding templates.
        /// </summary>
        public string FinalContent
        {
            get
            {
                if (string.IsNullOrEmpty(RawContent))
                {
                    return string.Empty;
                }
                string finalContent = RawContent;

                // Find the custom tag start pattern in the raw content using regex.
                Match myt = Regex.Match(finalContent, StartPattern, RegexOptions.IgnoreCase);
                if (myt.Success)
                {
                    // Find the end pattern using DelimiterResolver.EndPattern to determine the end of the custom tag.
                    Match emt = Regex.Match(myt.Value, DelimiterResolver.EndPattern, RegexOptions.IgnoreCase);
                    if (emt.Success)
                    {
                        // Extract the pointer name from the custom tag.
                        PointerName = HTMLScrubber.ScrubHtml(myt.Value.Substring(StartDelimiterLength, (emt.Index - StartDelimiterLength)));
                        PointerWithDelimiter = myt.Value.Substring(0, (emt.Index + emt.Length));

                        // Load the template content from the file if the path is provided.
                        if (!string.IsNullOrEmpty(TemplateContentpath))
                        {
                            TemplateContent = System.IO.File.ReadAllText(TemplateContentpath);
                        }

                        // Replace the custom tag and its content with the template content.
                        finalContent = finalContent.Replace(PointerWithDelimiter, TemplateContent);
                    }
                }

                return finalContent;
            }
        }
    }
}




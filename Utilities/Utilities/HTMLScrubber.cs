using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Utilities.Utilities
{
    /// <summary>
    /// Utility class for scrubbing HTML content to remove unwanted tags and formatting.
    /// </summary>
    public class HTMLScrubber
    {
        /// <summary>
        /// Removes unwanted HTML tags and formatting from the input string.
        /// </summary>
        /// <param name="value">The input HTML string to be scrubbed.</param>
        /// <returns>The scrubbed HTML string with unwanted tags and formatting removed.</returns>
        public static string ScrubHtml(string value)
        {
            // Decode HTML entities like &amp;, &lt;, etc.
            value = HttpUtility.HtmlDecode(value);

            // Remove HTML comments.
            value = RemoveTag(value, "<!--", "-->");

            // Remove <script> tags and its content.
            value = RemoveTag(value, "<script", "</script>");

            // Remove <style> tags and its content.
            value = RemoveTag(value, "<style", "</style>");

            // Remove all remaining HTML tags and replace &nbsp; with a space.
            string step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();

            // Replace multiple spaces with a single space.
            string step2 = Regex.Replace(step1, @"\s{2,}", " ");

            // Trim leading and trailing spaces from each line.
            step2 = SingleSpacedTrim(step2);

            return step2;
        }

        /// <summary>
        /// Removes a specific HTML tag and its content from the input HTML string.
        /// </summary>
        /// <param name="html">The input HTML string to be processed.</param>
        /// <param name="startTag">The opening tag to be removed.</param>
        /// <param name="endTag">The closing tag to be removed.</param>
        /// <returns>The HTML string with the specified tag and its content removed.</returns>
        private static string RemoveTag(string html, string startTag, string endTag)
        {
            bool bAgain;
            do
            {
                bAgain = false;

                // Find the position of the startTag in the HTML string (ignoring case).
                int startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
                if (startTagPos < 0)
                {
                    // The startTag is not found, continue to the next iteration.
                    continue;
                }

                // Find the position of the endTag after the startTag (ignoring case).
                int endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
                if (endTagPos <= startTagPos)
                {
                    // The endTag is not found after the startTag, continue to the next iteration.
                    continue;
                }

                // Remove the entire content between the startTag and endTag, including the tags.
                html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
                bAgain = true;

            } while (bAgain);

            return html;
        }

        /// <summary>
        /// Trims leading and trailing spaces from each line of the input string.
        /// </summary>
        /// <param name="inString">The input string to be processed.</param>
        /// <returns>The input string with leading and trailing spaces trimmed from each line.</returns>
        private static string SingleSpacedTrim(string inString)
        {
            StringBuilder sb = new StringBuilder();
            bool inBlanks = false;

            foreach (char c in inString)
            {
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        if (!inBlanks)
                        {
                            // Replace consecutive spaces with a single space.
                            inBlanks = true;
                            sb.Append(' ');
                        }
                        continue;
                    default:
                        inBlanks = false;
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString().Trim();
        }

    }
}




